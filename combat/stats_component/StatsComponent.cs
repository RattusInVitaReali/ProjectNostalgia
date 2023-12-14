using System;
using System.Collections.Generic;
using Godot;
using ProjectNostalgia.combat.hit_manager_component;

namespace ProjectNostalgia.combat.stats_component;

public partial class StatsComponent : CombatComponent, IHitProcessor
{
    
    [Export]
    private Godot.Collections.Dictionary<string, float> BaseStats { get; set; } = new()
    {
        ["MaxHP"] = 10,
        ["PhysicalDamage"] = 5,
        ["Armor"] = 0,
        ["FlatReduction"] = 0,
        ["MovementSpeed"] = 3,
        ["CriticalStrikeChance"] = 0.1f,
        ["CriticalStrikeMultiplier"] = 1.5f
    };

    private Dictionary<string, Stat> _stats = new();

    public override void _Ready()
    {
        base._Ready();
        foreach (string statName in BaseStats.Keys)
        {
            Stat stat = new Stat(statName, BaseStats[statName]);
            _stats.Add(statName, stat);
        }
    }

    public void SetStatBaseValue(string statName, float baseValue)
    {
        _stats[statName].SetBaseValue(baseValue);
    }
    
    public float GetStat(string statName)
    {
        return _stats[statName].Value;
    }

    public void AddModifier(StatModifier modifier)
    {
        _stats[modifier.StatName].AddModifier(modifier);
    }

    public void AddModifiers(IEnumerable<StatModifier> modifiers)
    {
        foreach (StatModifier modifier in modifiers)
        {
            AddModifier(modifier);
        }
    }

    public void RemoveModifier(StatModifier modifier)
    {
        _stats[modifier.StatName].RemoveModifier(modifier);
    }

    public void RemoveModifiersFromSource(Object source)
    {
        foreach (Stat stat in _stats.Values)
        {
            stat.RemoveModifiersFromSource(source);
        }
    }
    
    public int HitProcessorPriority()
    {
        return 100;
    }

    public void ProcessIncomingDamageInfo(DamageInfo info)
    {
        // Insert Armor/Resists formula here;
        info.Amount -= _stats["FlatReduction"].Value;
    }

    public void ProcessOutgoingDamageInfo(DamageInfo info)
    {
        
    }
}