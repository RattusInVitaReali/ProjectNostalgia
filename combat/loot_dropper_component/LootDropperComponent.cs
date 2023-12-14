using System;
using Godot;
using Godot.Collections;
using ProjectNostalgia.ground_object.ground_reward;
using ProjectNostalgia.loot_table;
using ProjectNostalgia.reward;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.combat.loot_dropper_component;

public partial class LootDropperComponent : CombatComponent
{

    [Export] private Array<LootTable> _lootTables; 
    
    public void DropLoot(int level)
    {
        foreach (LootTable lootTable in _lootTables)
        {
            GroundReward groundReward = GetGroundRewardFromTable(lootTable, level);
            if (groundReward == null) continue;
            SpawnGroundReward(groundReward);
        }
    }

    private GroundReward GetGroundRewardFromTable(LootTable lootTable, int level)
    {
        Reward reward = lootTable.RollLoot(level);
        if (reward == null) return null;
        GroundReward groundReward = GroundReward.FromReward(reward);
        return groundReward;
    }

    // TODO: Use ApplyImpulse() to throw it around a bit
    private void SpawnGroundReward(GroundReward groundReward)
    {
        GetTree().Root.AddChild(groundReward);
        groundReward.GlobalPosition = GlobalPosition + new Vector3(0.0f, 0.5f, 0.0f);
        Vector2 xz = new Vector2(RandomUtils.RNG.NextSingle() - 0.5f, RandomUtils.RNG.NextSingle() - 0.5f).Normalized();
        xz = xz * 3.0f;
        Vector3 impulse = new Vector3(xz.X, 0.5f, xz.Y);
        groundReward.ApplyImpulse(impulse);
    }
    
}