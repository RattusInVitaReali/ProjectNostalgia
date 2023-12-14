using System;
using System.Collections.Generic;
using Godot;
using ProjectNostalgia.destructable;
using ProjectNostalgia.monster_pack;

namespace ProjectNostalgia.room;

public partial class Room : Node3D
{
    private static PackedScene _monsterPackScene = GD.Load<PackedScene>("res://monster_pack/monster_pack.tscn");
    private static List<PackedScene> _destructables = new();
    private static int[] _tileIds = { 101, 102, 103, 106, 107, };
    private static Random _random = new();

    private GridMap _floor;

    [Export] public float DestructableChance = 1.0f;
    
    public int Width;
    public int Height;

    public override void _Ready()
    {
        _destructables.Add(GD.Load<PackedScene>("res://destructable/destructables/chest.tscn"));
        _destructables.Add(GD.Load<PackedScene>("res://destructable/destructables/barrels.tscn"));
        _floor = GetNode("Floor") as GridMap;
        Init();
    }

    public void PreInit(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public void Init()
    {
        // for (int i = 0; i < Width; i++)
        // {
        //     for (int j = 0; j < Height; j++)
        //     {
        //         _floor.SetCellItem(new Vector3I(i, 0, j), _tileIds[_random.Next(0, _tileIds.Length)]);
        //     }
        // }

        // for (int i = 0; i < Width; i++)
        // {
        //     if (_random.NextSingle() < DestructableChance)
        //     {
        //         Vector3 position = new Vector3(i * 2 + 1.0f, 0.0f, 1.0f);
        //         AddDestructable(position);
        //     }
        //
        //     if (_random.NextSingle() < DestructableChance)
        //     {
        //         Vector3 position = new Vector3(i * 2 + 1.0f, 0.0f, Height * 2 - 1);
        //         AddDestructable(position);
        //     }
        // }
        //
        // for (int i = 1; i < Height - 1; i++)
        // {
        //     if (_random.NextSingle() < DestructableChance)
        //     {
        //         Vector3 position = new Vector3(1.0f, 0.0f, i * 2+ 0.5f);
        //         AddDestructable(position);
        //     }
        //
        //     if (_random.NextSingle() < DestructableChance)
        //     {
        //         Vector3 position = new Vector3(Width * 2 - 1, 0.0f, i * 2 + 0.5f);
        //         AddDestructable(position);
        //     }
        // }

        MonsterPack pack = _monsterPackScene.Instantiate() as MonsterPack;
        AddChild(pack);
        pack!.Position = new Vector3(_random.NextSingle() * Width * 2, 0, _random.NextSingle() * Height * 2);
        pack.SpawnMonsters();
    }

    private void AddDestructable(Vector3 position)
    {
        Destructable destructable =
            _destructables[_random.Next(0, _destructables.Count)].Instantiate() as Destructable;
        AddChild(destructable);
        destructable!.Position = position;
        destructable!.Rotation = new Vector3(0.0f, _random.NextSingle() * 2 * (float)Math.PI, 0.0f);
    }
}