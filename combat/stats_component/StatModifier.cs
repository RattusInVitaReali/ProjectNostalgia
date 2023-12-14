using System;
using Godot;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.combat.stats_component;

public class StatModifier
{

    public enum StatModifierType
    {
        Flat,
        Increased,
        More,
    }
    
    public readonly Object Source;
    
    public readonly string StatName;
    public readonly StatModifierType Type;
    public readonly float Amount;
    public readonly Rarity Rarity;

    public StatModifier(string statName, StatModifierType type, float amount, Rarity rarity, object source)
    {
        StatName = statName;
        Type = type;
        Amount = amount;
        Rarity = rarity;
        Source = source;
    }

    public string GetDescription()
    {
        string description = Type switch
        {
            StatModifierType.Flat => "+ {flat} to " + StatName,
            StatModifierType.Increased => "{percent}% Increased " + StatName,
            StatModifierType.More => "{percent}% More " + StatName,
            _ => "Unknown modifier type - if you see this you get 100$"
        };
        
        description = description.ReplaceN("{flat}", Amount.ToString("#"));
        description = description.ReplaceN("{percent}", (Amount * 100.0f).ToString("#"));

        return description;
    }
}