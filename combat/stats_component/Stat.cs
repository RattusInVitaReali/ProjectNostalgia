using System;
using System.Collections.Generic;

namespace ProjectNostalgia.combat.stats_component;

public class Stat
{

    public float Value
    {
        get
        {
            if (_dirty)
            {
                _value = CalculateValue();
                _dirty = false;
            }

            return _value;
        }
    }

    private float _value;
    public readonly string Name;
    private float _baseValue;

    private List<StatModifier> _modifiers = new();
    private bool _dirty = true;
    
    public Stat(string name, float baseValue)
    {
        Name = name;
        _baseValue = baseValue;
    }

    public void SetBaseValue(float baseValue)
    {
        _baseValue = baseValue;
    }
    
    private float CalculateValue()
    {
        float increased = 1.0f;
        float flat = 0.0f;
        List<float> more = new();

        foreach (StatModifier modifier in _modifiers)
        {
            switch (modifier.Type)
            {
                case StatModifier.StatModifierType.Flat:
                    flat += modifier.Amount;
                    break;
                case StatModifier.StatModifierType.Increased:
                    increased += modifier.Amount;
                    break;
                case StatModifier.StatModifierType.More:
                    more.Add(modifier.Amount);
                    break;
            }
        }

        float value = (_baseValue + flat) * increased;
        foreach (float multiplier in more)
        {
            value *= multiplier;
        }

        return value;
    }

    public void AddModifier(StatModifier modifier)
    {
        _dirty = true;
        _modifiers.Add(modifier);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (_modifiers.Remove(modifier))
        {
            _dirty = true;
        }
    }

    public void RemoveModifiersFromSource(Object source)
    {
        List<StatModifier> toRemove = new();
        
        foreach (StatModifier modifier in _modifiers)
        {
            if (modifier.Source == source)
            {
                toRemove.Add(modifier);
            }
        }

        foreach (StatModifier modifier in toRemove)
        {
            RemoveModifier(modifier);
        }
        {
            
        }
    }
    
}