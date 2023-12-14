using System;
using System.Linq;
using Godot;
using Godot.Collections;
using ProjectNostalgia.reward;

namespace ProjectNostalgia.loot_table;

[GlobalClass]
public partial class LootTable : Resource
{
    private static Random _random = new Random();

    [Export] public float DropChance = 0.16f;
    [Export] public Array<LootTableComponent> Pool { get; set; }
    [Export] public int RollCount = 1;
    
    public Reward GetLoot(int level)
    {
        int totalWeight = Pool.Sum(ltc => ltc.Weight);
        int roll = _random.Next() % totalWeight;
        foreach (LootTableComponent lootTableComponent in Pool)
        {
            roll -= lootTableComponent.Weight;
            if (roll < 0)
            {
                return lootTableComponent.GetReward(level);
            }
        }

        return null;
    }

    public Reward RollLoot(int level)
    {
        return _random.NextSingle() < DropChance ? GetLoot(level) : null;
    }
}