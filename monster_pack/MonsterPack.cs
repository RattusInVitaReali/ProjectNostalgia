using System;
using Godot;
using ProjectNostalgia.entity.monster;

namespace ProjectNostalgia.monster_pack;

public partial class MonsterPack : Node3D
{
	private PackedScene _monster = GD.Load<PackedScene>("res://entity/monster/monsters/dino.tscn");
	[Export] public int MonsterCountMin = 2;
	[Export] public int MonsterCountMax = 4;
	
	public void SpawnMonsters()
	{
		Random random = new Random();
		for (int i = 0; i < random.Next(MonsterCountMin, MonsterCountMax + 1); i++)
		{
			Monster monster = Monster.CreateMonsterAndAddChild(_monster, this, 5);
			monster!.Position = new Vector3(random.NextSingle() * 2 - 4, 0.5f, random.NextSingle() * 2 - 4);
		}
	}
	
}
