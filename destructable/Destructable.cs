using System.Collections.Generic;
using Godot;
using ProjectNostalgia.combat;
using ProjectNostalgia.combat.hurtbox_component;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.destructable;

public partial class Destructable : StaticBody3D, IComponentContainer
{

    private ComponentManager _manager = new();

    public ComponentManager GetManager()
    {
        return _manager;
    }

    public override void _Ready()
    {
        GetNode<HurtboxComponent>("HurtboxComponent").Init(CollisionUtils.OwnerType.Destructible);
    }

    private void _OnHealthComponentDied()
    {
        QueueFree();
    }
    
}