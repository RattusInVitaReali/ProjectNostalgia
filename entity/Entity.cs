using System;
using System.Collections.Generic;
using Godot;
using ProjectNostalgia.combat;
using ProjectNostalgia.combat.stats_component;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.skill;

namespace ProjectNostalgia.entity;

public partial class Entity : CharacterBody3D, IComponentContainer
{
	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	protected List<Skill> Skills = new();

	private ComponentManager _manager = new();

	public ComponentManager GetManager()
	{
		return _manager;
	}
	
	public override void _Ready()
	{
		foreach (Node node in GetNode<Node3D>("Skills").GetChildren())
		{
			Skills.Add((Skill) node);
		}
		InitSkills();
	}

	public float GetStat(string statName)
	{
		return GetManager().GetComponent<StatsComponent>().GetStat(statName);
	}
	
	protected virtual void InitSkills()
	{
		throw new NotImplementedException();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity.Y -= _gravity * (float)delta;
			Velocity = velocity;
		}

		MoveAndSlide();
	}

	protected void RotateSmoothly(double delta)
	{
		if (Velocity != Vector3.Zero)
		{
			double newY = Mathf.LerpAngle(Rotation.Y, Math.Atan2(Velocity.X, Velocity.Z), delta * 20);
			Vector3 newRotation = Rotation;
			newRotation.Y = (float)newY;
			Rotation = newRotation;
		}
	}

	protected void _OnHealthComponentDied()
	{
		StartDying();
	}

	protected virtual void StartDying()
	{
		
	}
}
