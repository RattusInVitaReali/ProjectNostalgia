using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using ProjectNostalgia.affix;
using ProjectNostalgia.combat.items_component;
using ProjectNostalgia.combat.stats_component;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.item;

public partial class Item : Node3D, ISlottable
{
    private static Random _random = new();

    private static PackedScene _itemScene = GD.Load<PackedScene>("res://item/item.tscn");

    public enum Type
    {
        Axe,
        Sword,
        Dagger,
        Chestplate
    }

    public ItemData ItemData { get; private set; }
    public int Level { get; private set; }
    public Rarity Rarity { get; private set; }
    public string ItemName { get; private set; }
    
    private List<Affix> _affixes = new();
    private List<StatModifier> _statModifiers = new();
    private ItemsComponent _owner;

    public IEnumerable<StatModifier> GetModifiers()
    {
        return _statModifiers;
    }

    public IEnumerable<Affix> GetAffixes()
    {
        return _affixes;
    }

    public static Item FromItemData(ItemData itemData, int level, Rarity rarity)
    {
        Item item = _itemScene.Instantiate<Item>();
        item.ItemData = itemData;
        item.Rarity = rarity;
        item.Level = level;
        item.ItemName = item.Rarity + " " + item.ItemData.Type;
        
        List<AffixData> validAffixes = itemData.Affixes.Where(affix => affix.MinLevel <= level && affix.MaxLevel >= level).ToList();
        int affixCount = RarityUtils.GetAffixCount(rarity);
        for (int i = 0; i < affixCount; i++)
        {
            AffixData newAffixData = GetValidAffixData(validAffixes);
            if (newAffixData == null) break;
            Affix newAffix = GenerateAffix(newAffixData, item);
            item._affixes.Add(newAffix);
            validAffixes.Remove(newAffixData);
        }

        foreach (Affix affix in item._affixes)
        {
            item._statModifiers.Add(affix.StatModifier);
        }
        
        return item;
    }

    public static Item GenerateTestItem()
    {
        Random random = new Random();
        
        List<ItemData> itemDatas = new();
        foreach (string itemDataPath in DirectoryUtils.GetFilesInDirectory("res://item/items"))
        {
            itemDatas.Add(GD.Load<ItemData>(itemDataPath));
        }

        ItemData itemData = itemDatas[random.Next() % itemDatas.Count];
        
        int level = random.Next() % 100 + 1;
        
        var rarities = Enum.GetValues<Rarity>();
        Rarity rarity = rarities[random.Next(rarities.Length)];

        return FromItemData(itemData, level, rarity);
    }

    private static AffixData GetValidAffixData(List<AffixData> validAffixes)
    {
        if (validAffixes.Count < 1) return null;
        int totalWeight = validAffixes.Sum(affix => affix.Weight);
        int roll = _random.Next() % totalWeight;
        foreach (AffixData affixData in validAffixes)
        {
            roll -= affixData.Weight;
            if (roll < 0) return affixData;
        }

        return null;
    }

    private static Affix GenerateAffix(AffixData affixData, Item item)
    {

        int[] weights = { 20, 30, 50 };
        List<Tuple<Rarity, int>> rarityWeights = new();
        Rarity currentRarity = item.GetRarity();
        for (int i = 0; i < 3; i++)
        {
            rarityWeights.Add(new Tuple<Rarity, int>(currentRarity, weights[i]));
            int currentRarityIndex = (int)currentRarity;
            int nextRarityIndex = Math.Max(0, currentRarityIndex - 1);
            currentRarity = (Rarity)nextRarityIndex;
        }

        Rarity rarity = Rarity.Common;
        int roll = RandomUtils.RNG.Next(weights.Sum());
        foreach (var pair in rarityWeights)
        {
            roll -= pair.Item2;
            if (roll < 0)
            {
                rarity = pair.Item1;
                break;
            }
        }

        return new Affix(affixData, rarity, item.Level, item);

    }

    public Texture2D GetIcon()
    {
        return ItemData.Icon;
    }

    public Rarity GetRarity()
    {
        return Rarity;
    }
    
    public int GetAmount()
    {
        return 1;
    }
}