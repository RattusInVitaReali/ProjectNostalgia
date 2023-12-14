using Godot;
using ProjectNostalgia.combat.action_queue_component;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.combat.hurtbox_component;

public partial class HurtboxComponent : CombatComponent
{
    
    [Export] private ActionQueueComponent _actionQueue;
    
    private Area3D _area;

    public override void _Ready()
    {
        base._Ready();
        _area = GetNode<Area3D>("Area3D");
    }

    public void Init(CollisionUtils.OwnerType type)
    {
        if (type == CollisionUtils.OwnerType.Player)
        {
            AddLayer(CollisionUtils.CollisionLayer.PlayerHurtbox);
        }

        if (type == CollisionUtils.OwnerType.Monster)
        {
            AddLayer(CollisionUtils.CollisionLayer.MonsterHurtbox);
        }

        if (type == CollisionUtils.OwnerType.Destructible)
        {
            AddLayer(CollisionUtils.CollisionLayer.DestructibleHurtbox);
        }
    }

    public Area3D GetArea()
    {
        return _area;
    }
    
    public void AddCombatAction(CombatAction action)
    {
        _actionQueue.AddToBot(action);
    }

    public void AddLayer(CollisionUtils.CollisionLayer layer)
    {
        _area.SetCollisionLayerValue(CollisionUtils.GetLayer(layer), true);
    }
    
    public void RemoveLayer(CollisionUtils.CollisionLayer layer)
    {
        _area.SetCollisionLayerValue(CollisionUtils.GetLayer(layer), false);
    }
    
}