using Godot;
using ProjectNostalgia.entity;
using ProjectNostalgia.interfaces;

namespace ProjectNostalgia.combat.action_queue_component;

public class CombatAction
{

    public readonly IComponentContainer Source;
    public readonly IComponentContainer Target;

    public CombatAction(IComponentContainer source, IComponentContainer target)
    {
        Source = source;
        Target = target;
    }

    public virtual void Execute()
    {
        
    }
    
    
}