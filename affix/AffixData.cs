using System;
using System.Collections.Generic;
using Godot;
using ProjectNostalgia.combat.stats_component;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.affix;

[GlobalClass]
public partial class AffixData : Resource
{
    private static Random _random = new();

    [Export] public string Name;
    [Export] public int Weight = 1000;
    [Export] public int MinLevel = 1;
    [Export] public int MaxLevel = 100;
    [Export] public float MinValue;
    [Export] public float MinValuePerLevel;
    [Export] public float MaxValue;
    [Export] public float MaxValuePerLevel;
    [Export] public string StatName;
    [Export] public StatModifier.StatModifierType ModifierType;
    [Export] private string _description;

    private Dictionary<Rarity, float> _rarityMultipliers = new()
    {
        { Rarity.Common, 1.0f },
        { Rarity.Uncommon, 1.2f },
        { Rarity.Rare, 1.5f },
        { Rarity.Epic, 2.0f },
        { Rarity.Legendary, 3.0f },
        { Rarity.Mythic, 5.0f }
    };

    public StatModifier GetModifier(int level, Rarity rarity, object source)
    {
        float rarityMultiplier = _rarityMultipliers[rarity];
        float maxValue = (MaxValue + level * MaxValuePerLevel) * rarityMultiplier;
        float minValue = (MinValue + level * MinValuePerLevel) * rarityMultiplier;
        float value = minValue + _random.NextSingle() * (maxValue - minValue);

        int roundingPrecision = ModifierType == StatModifier.StatModifierType.Flat ? 0 : 2;
        value = (float)Math.Round(value, roundingPrecision);

        return new StatModifier(StatName, ModifierType, value, rarity, source);
    }
}