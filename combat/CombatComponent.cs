using System;
using Godot;
using ProjectNostalgia.interfaces;

namespace ProjectNostalgia.combat;

public partial class CombatComponent : Node3D
{

	[Export] private Node3D _overrideOwner;
	public IComponentContainer ComponentOwner = null;
	
	public override void _Ready()
	{
		ComponentOwner = _overrideOwner == null ? GetParentOrNull<IComponentContainer>() : (IComponentContainer)_overrideOwner; 
		if (ComponentOwner == null) throw new Exception("Component has no owner or parent!");
		ComponentOwner.GetManager().RegisterComponent(this);
	}
	
}