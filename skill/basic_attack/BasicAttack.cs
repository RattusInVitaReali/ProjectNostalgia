using System.Collections.Generic;
using Godot;
using Godot.NativeInterop;
using ProjectNostalgia.combat;
using ProjectNostalgia.combat.hitbox_component;
using ProjectNostalgia.combat.hurtbox_component;
using ProjectNostalgia.entity.player;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.skill.basic_attack;

public partial class BasicAttack : Skill, IComponentContainer
{
	private HitboxComponent _hitbox;
	private ComponentManager _manager = new();
	
	public IComponentContainer Target;

	public ComponentManager GetManager()
	{
		return _manager;
	}
	
	public override void _Ready()
	{
		base._Ready();
		_hitbox = GetNode("HitboxComponent") as HitboxComponent;
	}

	protected override void SpecialInit(IComponentContainer owner, CollisionUtils.OwnerType type)
	{
		_hitbox.AddTargetLayer(CollisionUtils.CollisionLayer.DestructibleHurtbox);
		if (type == CollisionUtils.OwnerType.Monster)
		{
			_hitbox.AddTargetLayer(CollisionUtils.CollisionLayer.PlayerHurtbox);
		}
		if (type == CollisionUtils.OwnerType.Player)
		{
			_hitbox.AddTargetLayer(CollisionUtils.CollisionLayer.MonsterHurtbox);
		}
	}

	public override bool HasTarget()
	{
		return _hitbox.HasTarget();
	}

	protected override void SkillEffect()
	{
		foreach (HurtboxComponent target in _hitbox.GetTargets())
		{
			DamageInfo damage = new DamageInfo(SkillOwner, target.ComponentOwner, DamageInfo.DamageType.Physical, 5.0f);
			HitInfo hit = new HitInfo(SkillOwner, target.ComponentOwner, new List<DamageInfo> { damage });
			HitAction action = new HitAction(SkillOwner, target.ComponentOwner, hit);
			target.AddCombatAction(action);
		}
	}
}
