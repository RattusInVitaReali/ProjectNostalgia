using System.Collections.Generic;
using Godot;
using ProjectNostalgia.combat.hurtbox_component;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.combat.hitbox_component;

public partial class HitboxComponent : CombatComponent
{

    [Signal]
    public delegate void TargetFoundEventHandler(HurtboxComponent target);

    private Area3D _area;
    
    public override void _Ready()
    {
        base._Ready();
        _area = GetNode<Area3D>("Area3D");

    }

    public Area3D GetArea()
    {
        return _area;
    }

    public void Init(CollisionUtils.OwnerType type)
    {
        AddTargetLayer(CollisionUtils.CollisionLayer.DestructibleHurtbox);
        if (type == CollisionUtils.OwnerType.Player)
        {
            AddTargetLayer(CollisionUtils.CollisionLayer.MonsterHurtbox);
        }

        if (type == CollisionUtils.OwnerType.Monster)
        {
            AddTargetLayer(CollisionUtils.CollisionLayer.PlayerHurtbox);
        }
    }
    
    public void AddTargetLayer(CollisionUtils.CollisionLayer layer)
    {
        _area.SetCollisionMaskValue(CollisionUtils.GetLayer(layer), true);
    }

    public void RemoveTargetLayer(CollisionUtils.CollisionLayer layer)
    {
        _area.SetCollisionMaskValue(CollisionUtils.GetLayer(layer), false);
    }
    
    public bool HasTarget()
    {
        return _area.HasOverlappingAreas();
    }

    public List<HurtboxComponent> GetTargets()
    {
        List<HurtboxComponent> targets = new();
        foreach (Area3D area in _area.GetOverlappingAreas())
        {
            targets.Add(area.GetParent<HurtboxComponent>());
        }

        return targets;
    }

    private void _OnArea3DAreaEntered(Area3D area)
    {
        EmitSignal("TargetFound", area.GetParent<HurtboxComponent>());
    }

    public bool IsInRange(HurtboxComponent hurtbox)
    {
        return _area.OverlapsArea(hurtbox.GetArea());
    }

}