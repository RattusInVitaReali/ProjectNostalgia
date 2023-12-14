using System;
using System.Collections.Generic;
using Godot;
using ProjectNostalgia.entity.player;
using ProjectNostalgia.room;

namespace ProjectNostalgia.level_generator;

public partial class LevelGenerator : Node3D
{
	[Export] public int Size = 200;
	[Export] public int Octaves = 5;
	[Export] public float Frequency = 0.015f;
	[Export] public float TerrainMaxHeight = 15.0f;
	[Export] public float TreeMaxHeight = 6.0f;
	[Export] public float DungeonHeight = 10.0f;
	[Export] public int RoomSizeMin = 11;
	[Export] public int RoomSizeMax = 15;
	[Export] public int RoomTries = 100;

	private static PackedScene _playerScene = GD.Load<PackedScene>("res://entity/player/player.tscn");
	private static PackedScene _monsterPackScene = GD.Load<PackedScene>("res://monster_pack/monster_pack.tscn");
	private static PackedScene _roomScene = GD.Load<PackedScene>("res://room/room.tscn");
	private Node3D _roomsNode;

	public override void _Ready()
	{
		_roomsNode = GetNode("Rooms") as Node3D;
		
		int canvasSize = Size / 2 + 1;

		RoomData startRoom = GenerateRoom(11, 11, canvasSize, false);
		List<RoomData> rooms = GenerateRooms(canvasSize, RoomTries, startRoom);
		List<int> pathIndices = GeneratePaths(rooms, canvasSize);

		Mesh mesh = GenerateMesh(Size, Octaves, Frequency, TerrainMaxHeight, rooms, pathIndices);
		MeshInstance3D meshInstance = GetNode("NavigationRegion3D/MeshInstance3D") as MeshInstance3D;
		meshInstance!.Mesh = mesh;
		meshInstance.CreateTrimeshCollision();

		foreach (RoomData roomData in rooms)
		{
			Room room = roomData.GetRoomNode();
			_roomsNode!.AddChild(room);
			int roomIndex = VectorToInt(roomData.Position + new Vector2(roomData.Width - 2, roomData.Height - 2), canvasSize);
			Vector3 roomPosition = new Vector3(0.0f, DungeonHeight + 0.05f, 0.0f);
			roomPosition.X = ((float)canvasSize / 2 - roomIndex % canvasSize) * 2 - 1f;
			roomPosition.Z = ((float)canvasSize / 2 - roomIndex / canvasSize) * 2 - 1f;
			room.Position = roomPosition;
		}
		
		NavigationRegion3D navigationRegion = GetNode("NavigationRegion3D") as NavigationRegion3D;
		navigationRegion!.BakeNavigationMesh(false);

		RayCast3D ray = GetNode("RayCast3D") as RayCast3D;
		MultiMesh trees = GenerateTrees(meshInstance, ray, Size);
		MultiMeshInstance3D multiMeshInstance = GetNode("MultiMeshInstance3D") as MultiMeshInstance3D;
		multiMeshInstance!.Multimesh = trees;

		int playerPosIndex = startRoom.RoomIndices[startRoom.RoomIndices.Count / 2];
		Vector3 playerPosition = new Vector3(0.0f, 12.0f, 0.0f);
		playerPosition.X = (canvasSize / 2 - playerPosIndex % canvasSize) * 2;
		playerPosition.Z = (canvasSize / 2 - playerPosIndex / canvasSize) * 2;
		GD.Print("Player pos: ", playerPosition);
		GD.Print("Start room pos: ", startRoom.CenterPosition);
		Player player = _playerScene.Instantiate() as Player;
		AddChild(player);
		player!.GlobalPosition = playerPosition;
	}

	private class RoomData
	{
		public List<int> RoomIndices;
		public List<int> BorderIndices;
		public Vector2 Position;
		public Vector2 CenterPosition;
		public int Height;
		public int Width;
		public int CanvasSize;
		public Vector2 TopDoor;
		public Vector2 BottomDoor;
		public Vector2 LeftDoor;
		public Vector2 RightDoor;

		public RoomData(List<int> roomIndices, List<int> borderIndices, Vector2 position, int width, int height,
			int canvasSize)
		{
			RoomIndices = roomIndices;
			BorderIndices = borderIndices;
			Position = position;
			Height = height;
			Width = width;
			CanvasSize = canvasSize;
			CenterPosition = Position + new Vector2(Width / 2, Height / 2);
			TopDoor = new Vector2(position.X + width / 2, position.Y); // top
			BottomDoor = new Vector2(position.X + width / 2, position.Y + height); // bottom
			LeftDoor = new Vector2(position.X, position.Y + height / 2); // left
			RightDoor = new Vector2(position.X + width, position.Y + height / 2); // right
		}

		public Room GetRoomNode()
		{
			Room room = _roomScene.Instantiate() as Room;
			room!.PreInit(Width - 3, Height - 3);
			return room;
		}

	}

	private Mesh GenerateMesh(int size, int octaves, float frequency, float height, List<RoomData> rooms,
		List<int> pathIndices)
	{
		FastNoiseLite noise = new FastNoiseLite();
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
		noise.FractalOctaves = octaves;
		noise.Frequency = frequency;

		PlaneMesh mesh = new PlaneMesh();
		mesh.Size = new Vector2(size, size);
		mesh.SubdivideDepth = size / 2 - 1;
		mesh.SubdivideWidth = size / 2 - 1;

		SurfaceTool surfaceTool = new SurfaceTool();
		surfaceTool.CreateFrom(mesh, 0);

		ArrayMesh arrayPlane = surfaceTool.Commit();

		MeshDataTool dataTool = new MeshDataTool();
		dataTool.CreateFromSurface(arrayPlane, 0);

		GD.Print("Vertices: ", dataTool.GetVertexCount());
		for (int i = 0; i < dataTool.GetVertexCount(); i++)
		{
			Vector3 vertex = dataTool.GetVertex(i);
			vertex.Y = noise.GetNoise3D(vertex.X, vertex.Y, vertex.Z) * height;
			dataTool.SetVertex(i, vertex);
		}

		int canvasSize = size / 2 + 1;
		RoomData startRoom = GenerateRoom(11, 11, canvasSize);

		foreach (int idx in GetDungeonIndices(rooms, pathIndices, startRoom))
		{
			Vector3 vertex = dataTool.GetVertex(idx);
			vertex.Y = DungeonHeight;
			dataTool.SetVertex(idx, vertex);
		}

		arrayPlane.ClearSurfaces();

		dataTool.CommitToSurface(arrayPlane);
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
		surfaceTool.CreateFrom(arrayPlane, 0);
		surfaceTool.GenerateNormals();

		return surfaceTool.Commit();
	}

	private List<int> GetDungeonIndices(List<RoomData> rooms, List<int> pathIndices, RoomData startRoom)
	{
		List<int> dungeonIndices = new List<int>();
		foreach (RoomData room in rooms)
		{
			dungeonIndices.AddRange(room.RoomIndices);
		}

		dungeonIndices.AddRange(pathIndices);
		return dungeonIndices;
	}

	private List<RoomData> GenerateRooms(int canvasSize, int roomAttempts, RoomData startRoom)
	{
		List<RoomData> rooms = new List<RoomData>();
		rooms.Add(startRoom);

		for (int i = 0; i < roomAttempts; i++)
		{
			bool collision = false;
			RoomData room = GenerateRoom(RoomSizeMin, RoomSizeMax, canvasSize);
			foreach (RoomData roomData in rooms)
			{
				if (AreRoomsColliding(roomData, room))
				{
					collision = true;
					break;
				}
			}

			if (!collision)
			{
				rooms.Add(room);
			}
		}

		return rooms;
	}

	private RoomData GenerateRoom(int roomSizeMin, int roomSizeMax, int canvasSize, bool spawnMonsters = true)
	{
		List<int> roomIndices = new List<int>();
		List<int> borderIndices = new List<int>();
		Random random = new Random();
		int width = 0;
		while (width % 2 == 0) width = random.Next(roomSizeMin, roomSizeMax + 1);
		int height = 0;
		while (height % 2 == 0) height = random.Next(roomSizeMin, roomSizeMax + 1);
		int x = random.Next(0, canvasSize - width);
		int y = random.Next(0, canvasSize - height);

		for (int i = 1; i < width - 1; i++)
		{
			for (int j = 1; j < height - 1; j++)
			{
				roomIndices.Add(x + i + canvasSize * (y + j));
			}
		}

		for (int i = 0; i < width; i++)
		{
			borderIndices.Add(x + i + canvasSize * y); // Top border
			borderIndices.Add(x + i + canvasSize * (y + height - 1)); // Bottom border
		}

		for (int j = 0; j < height; j++)
		{
			borderIndices.Add(x + canvasSize * (y + j)); // Left border
			borderIndices.Add(x + width - 1 + canvasSize * (y + j)); // Right border
		}

		return new RoomData(roomIndices, borderIndices, new Vector2(x, y), width, height, canvasSize);
	}

	private bool AreRoomsColliding(RoomData firstRoom, RoomData secondRoom)
	{
		List<int> first = new List<int>();
		first.AddRange(firstRoom.RoomIndices);
		first.AddRange(firstRoom.BorderIndices);
		List<int> second = new List<int>();
		second.AddRange(secondRoom.RoomIndices);
		second.AddRange(secondRoom.BorderIndices);
		foreach (int pos in first)
		{
			if (second.Contains(pos))
			{
				return true;
			}
		}

		return false;
	}

	private List<int> GeneratePaths(List<RoomData> rooms, int canvasSize)
	{
		Dictionary<double, RoomData> roomsDict = new Dictionary<double, RoomData>();

		rooms = new List<RoomData>(rooms);
		AStar2D aStar = new AStar2D();
		RoomData firstRoom = rooms[0];
		rooms.RemoveAt(0);
		long firstId = aStar.GetAvailablePointId();
		aStar.AddPoint(firstId, firstRoom.CenterPosition);
		roomsDict.Add(firstId, firstRoom);

		while (rooms.Count > 0)
		{
			float minDistance = Single.PositiveInfinity;
			Vector2 minPosition = new Vector2();
			RoomData minRoom = null;
			Vector2 pos = new Vector2();
			foreach (long pointId in aStar.GetPointIds())
			{
				Vector2 pos1 = aStar.GetPointPosition(pointId);
				foreach (RoomData room in rooms)
				{
					Vector2 pos2 = room.CenterPosition;
					if (pos1.DistanceTo(pos2) < minDistance)
					{
						minDistance = pos1.DistanceTo(pos2);
						minRoom = room;
						minPosition = pos2;
						pos = pos1;
					}
				}
			}

			long newId = aStar.GetAvailablePointId();
			aStar.AddPoint(newId, minPosition);
			aStar.ConnectPoints(aStar.GetClosestPoint(pos), newId);
			roomsDict.Add(newId, minRoom);
			rooms.Remove(minRoom);
		}

		HashSet<string> visited = new();
		List<int> indices = new List<int>();
		foreach (long pointId in aStar.GetPointIds())
		{
			foreach (long connectionId in aStar.GetPointConnections(pointId))
			{
				if (visited.Contains(pointId.ToString() + connectionId) ||
					visited.Contains(connectionId.ToString() + pointId)) continue;
				RoomData first = roomsDict[pointId];
				RoomData second = roomsDict[connectionId];
				List<int> connectionIndices = GetPathIndices(first, second, canvasSize);
				indices.AddRange(connectionIndices);
				visited.Add(pointId.ToString() + connectionId);
			}
		}

		return indices;
	}

	private List<int> GetPathIndices(RoomData first, RoomData second, int canvasSize)
	{
		List<int> indices = new List<int>();
		Vector2 pos1 = Vector2.Zero;
		Vector2 pos2 = Vector2.Zero;
		float distance = int.MaxValue;

		Tuple<RoomData, RoomData>[] roomDataArray =
		{
			new(first, second),
			new(second, first)
		};

		float xy = pos2.Y;
		float yx = pos1.X;

		foreach (Tuple<RoomData, RoomData> tuple in roomDataArray)
		{
			RoomData firstRoom = tuple.Item1;
			RoomData secondRoom = tuple.Item2;

			if (firstRoom.TopDoor.DistanceTo(secondRoom.LeftDoor) < distance)
			{
				if (firstRoom.TopDoor.X <= secondRoom.LeftDoor.X &&
					firstRoom.TopDoor.Y >= secondRoom.LeftDoor.Y)
				{
					pos1 = firstRoom.TopDoor;
					pos2 = secondRoom.LeftDoor;
					distance = pos1.DistanceTo(pos2);
					xy = pos2.Y;
					yx = pos1.X;
				}
			}

			if (firstRoom.TopDoor.DistanceTo(secondRoom.RightDoor) < distance)
			{
				if (firstRoom.TopDoor.X >= secondRoom.RightDoor.X &&
					firstRoom.TopDoor.Y >= secondRoom.RightDoor.Y)
				{
					pos1 = firstRoom.TopDoor;
					pos2 = secondRoom.RightDoor;
					distance = pos1.DistanceTo(pos2);
					xy = pos2.Y;
					yx = pos1.X;
				}
			}

			if (firstRoom.BottomDoor.DistanceTo(secondRoom.LeftDoor) < distance)
			{
				if (firstRoom.BottomDoor.X <= secondRoom.LeftDoor.X &&
					firstRoom.BottomDoor.Y <= secondRoom.LeftDoor.Y)
				{
					pos1 = firstRoom.BottomDoor;
					pos2 = secondRoom.LeftDoor;
					distance = pos1.DistanceTo(pos2);
					xy = pos2.Y;
					yx = pos1.X;
				}
			}

			if (firstRoom.BottomDoor.DistanceTo(secondRoom.RightDoor) < distance)
			{
				if (firstRoom.BottomDoor.X >= secondRoom.RightDoor.X &&
					firstRoom.BottomDoor.Y <= secondRoom.RightDoor.Y)
				{
					pos1 = firstRoom.BottomDoor;
					pos2 = secondRoom.RightDoor;
					distance = pos1.DistanceTo(pos2);
					xy = pos2.Y;
					yx = pos1.X;
				}
			}
		}

		if (pos1 == pos2)
		{
			foreach (Tuple<RoomData, RoomData> tuple in roomDataArray)
			{
				RoomData firstRoom = tuple.Item1;
				RoomData secondRoom = tuple.Item2;

				if (firstRoom.TopDoor.DistanceTo(secondRoom.BottomDoor) < distance)
				{
					if (firstRoom.TopDoor.Y >= secondRoom.BottomDoor.Y)
					{
						pos1 = firstRoom.TopDoor;
						pos2 = secondRoom.BottomDoor;
						distance = pos1.DistanceTo(pos2);
						xy = pos2.Y;
						yx = pos1.X;
					}
				}

				if (firstRoom.BottomDoor.DistanceTo(secondRoom.TopDoor) < distance)
				{
					if (firstRoom.BottomDoor.Y <= secondRoom.TopDoor.Y)
					{
						pos1 = firstRoom.BottomDoor;
						pos2 = secondRoom.TopDoor;
						distance = pos1.DistanceTo(pos2);
						xy = pos2.Y;
						yx = pos1.X;
					}
				}

				if (firstRoom.LeftDoor.DistanceTo(secondRoom.RightDoor) < distance)
				{
					if (firstRoom.LeftDoor.X <= secondRoom.RightDoor.X)
					{
						pos1 = firstRoom.LeftDoor;
						pos2 = secondRoom.RightDoor;
						distance = pos1.DistanceTo(pos2);
						xy = pos2.Y;
						yx = pos1.X;
					}
				}

				if (firstRoom.RightDoor.DistanceTo(secondRoom.LeftDoor) < distance)
				{
					if (firstRoom.RightDoor.Y >= secondRoom.LeftDoor.Y)
					{
						pos1 = firstRoom.RightDoor;
						pos2 = secondRoom.LeftDoor;
						distance = pos1.DistanceTo(pos2);
						xy = pos2.Y;
						yx = pos1.X;
					}
				}
			}
		}

		Vector2 startX = pos1.X < pos2.X ? pos1 : pos2;
		Vector2 endX = pos1.X > pos2.X ? pos1 : pos2;

		for (float i = startX.X - 1; i < endX.X + 1; i++)
		{
			indices.Add(VectorToInt(new Vector2(i, xy + 1), canvasSize));
			indices.Add(VectorToInt(new Vector2(i, xy), canvasSize));
			indices.Add(VectorToInt(new Vector2(i, xy - 1), canvasSize));
		}

		Vector2 startY = pos1.Y < pos2.Y ? pos1 : pos2;
		Vector2 endY = pos1.Y > pos2.Y ? pos1 : pos2;

		for (float i = startY.Y - 1; i < endY.Y + 1; i++)
		{
			indices.Add(VectorToInt(new Vector2(yx + 1, i), canvasSize));
			indices.Add(VectorToInt(new Vector2(yx, i), canvasSize));
			indices.Add(VectorToInt(new Vector2(yx - 1, i), canvasSize));
		}

		return indices;
	}

	private static int VectorToInt(Vector2 pos, int canvasSize)
	{
		return (int)(pos.X + pos.Y * canvasSize);
	}

	private MultiMesh GenerateTrees(MeshInstance3D meshInstance, RayCast3D ray, int canvasLength)
	{
		MultiMesh trees = new MultiMesh();
		trees.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		trees.Mesh = GD.Load("res://resources/objects/Tree.res") as Mesh;
		trees.InstanceCount = canvasLength * canvasLength / 25;
		Random random = new Random();
		for (int i = 0; i < trees.InstanceCount; i++)
		{
			float height = 30.0f;
			while (height >= TreeMaxHeight)
			{
				float x = random.NextSingle() * canvasLength - canvasLength / 2;
				float z = random.NextSingle() * canvasLength - canvasLength / 2;
				ray.GlobalPosition = new Vector3(x, 30.0f, z);
				ray.ForceRaycastUpdate();
				height = ray.GetCollisionPoint().Y;
			}

			Transform3D transform = new Transform3D(meshInstance.Basis,
				new Vector3(ray.GlobalPosition.X, height + 1.5f, ray.GlobalPosition.Z));
			float scale = random.NextSingle() * 0.5f + 2.0f;
			transform = transform.ScaledLocal(new Vector3(scale, scale, scale));
			transform = transform.RotatedLocal(Vector3.Up, random.NextSingle() * 2 * (float)Math.PI);
			trees.SetInstanceTransform(i, transform);
		}

		return trees;
	}
}
