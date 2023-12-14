using System.Collections.Generic;
using ProjectNostalgia.combat.stats_component;
using ProjectNostalgia.item;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.affix;

public class Affix
{
    
    public AffixData AffixData { get; }
    public Rarity Rarity { get; }
    public StatModifier StatModifier { get; }
    public int Level { get; }
    
    private Item _item;
    
    public Affix(AffixData affixData, Rarity rarity, int level, Item item)
    {
        AffixData = affixData;
        Rarity = rarity;
        Level = level;
        _item = item;
        StatModifier = AffixData.GetModifier(level, rarity, item);
    }


}