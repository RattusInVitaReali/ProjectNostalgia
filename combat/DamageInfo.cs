using Godot;
using ProjectNostalgia.entity;
using ProjectNostalgia.interfaces;

namespace ProjectNostalgia.combat;

public class DamageInfo
{
    
    public enum DamageType
    {
        Physical,
        Arcane,
        Fire,
        Ice,
        Lightning,
        Poison,
        Bleeding
    }

    public IComponentContainer Source;
    public IComponentContainer Target;

    public DamageType Type;
    public float Amount;

    public DamageInfo(IComponentContainer source, IComponentContainer target, DamageType type, float amount)
    {
        Type = type;
        Amount = amount;
        Target = target;
        Source = source;
    }
}