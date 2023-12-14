using System;
using System.Collections.Generic;
using Godot;

namespace ProjectNostalgia.utils;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic
}

public static class RarityUtils
{

    private static Dictionary<Rarity, Color> _rarityColors = new()
    {
        { Rarity.Common, Colors.White },
        { Rarity.Uncommon, Colors.Green },
        { Rarity.Rare, Colors.Blue },
        { Rarity.Epic, Colors.Purple },
        { Rarity.Legendary, Colors.Red },
        { Rarity.Mythic, Colors.Orange }
    };

    public static Color GetColor(Rarity rarity)
    {
        return _rarityColors[rarity];
    }

    public static Color GetLightenedColor(Rarity rarity)
    {
        return GetColor(rarity).Lightened(0.3f);
    }

    public static int GetAffixCount(Rarity rarity)
    {
        return Math.Min((int)rarity, 4);
    }
    
}