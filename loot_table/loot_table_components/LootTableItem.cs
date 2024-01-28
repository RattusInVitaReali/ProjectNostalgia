using System;
using Godot;
using ProjectNostalgia.item;
using ProjectNostalgia.reward;
using ProjectNostalgia.reward.rewards;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.loot_table.loot_table_components;

[GlobalClass]
public partial class LootTableItem : LootTableComponent
{
    
    [Export] private ItemData _item;
    
    public override Reward GetReward(int level)
    {
        Rarity rarity = GetRarity(level);
        int itemLevel = Math.Max(1, RandomUtils.RNG.Next((int)(level * 0.91) - 1, level + 1));
        Item item = Item.FromItemData(_item, itemLevel, rarity);
        return new ItemReward(item);
    }

    private Rarity GetRarity(int level)
    {
        if (RandomUtils.RNG.NextSingle() > 0.25 + 0.010 * level) return Rarity.Common;
        if (RandomUtils.RNG.NextSingle() > 0.15 + 0.007 * level) return Rarity.Uncommon;
        if (RandomUtils.RNG.NextSingle() > 0.10 + 0.005 * level) return Rarity.Rare;
        if (RandomUtils.RNG.NextSingle() > 0.05 + 0.003 * level) return Rarity.Epic;
        if (RandomUtils.RNG.NextSingle() > 0.05 + 0.002 * level) return Rarity.Legendary;
        return Rarity.Mythic;
    }
    
    
}