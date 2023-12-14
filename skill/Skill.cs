using System;
using Godot;
using ProjectNostalgia.entity;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.skill;

public partial class Skill : Node3D
{

	[Signal]
	public delegate void CastTimeFinishedEventHandler();

	protected IComponentContainer SkillOwner;
	protected IComponentContainer SkillTarget;
	protected float Cooldown;
	private Timer _cooldownTimer;
	private Timer _castTimeTimer;

	[Export] public float BaseCooldown = 1.0f;
	[Export] public float CastTime = 0.5f;
	[Export] public string AnimationName;

	public void Init(IComponentContainer owner, CollisionUtils.OwnerType type)
	{
		SkillOwner = owner;
		SpecialInit(owner, type);
	}

	protected virtual void SpecialInit(IComponentContainer owner, CollisionUtils.OwnerType type)
	{
		
	}

	public override void _Ready()
	{
		_cooldownTimer = GetNode("CooldownTimer") as Timer;
		_castTimeTimer = GetNode("CastTimeTimer") as Timer;
		_castTimeTimer!.Timeout += _OnCastTimeFinished;
		Cooldown = BaseCooldown;
	}

	public void Use()
	{
		_cooldownTimer.Start(Cooldown);
		_castTimeTimer.Start(CastTime);
	}

	public void Cancel()
	{
		_castTimeTimer.Stop();
	}

	public bool CanUse()
	{
		return _cooldownTimer.TimeLeft == 0 && SpecialCanUse();
	}

	protected virtual bool SpecialCanUse()
	{
		return true;
	}

	public virtual bool HasTarget()
	{
		throw new NotImplementedException();
	}

	private void _OnCastTimeFinished()
	{
		EmitSignal(SignalName.CastTimeFinished);
		SkillEffect();
	}
	
	protected virtual void SkillEffect()
	{
		
	}
}
