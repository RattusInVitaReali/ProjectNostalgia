using Godot;
using ProjectNostalgia.ground_object;

namespace ProjectNostalgia.combat.pick_up_aura_component;

public partial class PickUpAuraComponent : CombatComponent
{

    [Export] private float _radius = 2.0f;

    public override void _Ready()
    {
        base._Ready();
        CylinderShape3D shape = (CylinderShape3D)GetNode<Area3D>("Area3D").GetNode<CollisionShape3D>("CollisionShape3D").Shape;
        shape.Radius = _radius;
    }

    private void _OnAreaBodyEntered(Node3D body)
    {
        GroundObject groundObject = (GroundObject)body;
        groundObject.PickUp();
    }

}