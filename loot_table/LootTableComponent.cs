using Godot;
using ProjectNostalgia.reward;

namespace ProjectNostalgia.loot_table;

[GlobalClass]
public abstract partial class LootTableComponent : Resource
{

    [Export] public int MinLevel = 1;
    [Export] public int MaxLevel = 100;
    [Export] public int Weight = 1000;

    public abstract Reward GetReward(int level);

}