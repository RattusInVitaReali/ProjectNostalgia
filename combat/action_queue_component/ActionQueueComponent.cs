using System.Collections.Generic;
using Godot;

namespace ProjectNostalgia.combat.action_queue_component;

public partial class ActionQueueComponent : CombatComponent
{
	private List<CombatAction> _actions = new();

	public void AddToBot(CombatAction action)
	{
		_actions.Add(action);
	}

	public void AddToTop(CombatAction action)
	{
		_actions.Insert(0, action);
	}

	public bool NextAction()
	{
		CombatAction nextAction = _actions.Count > 0 ? _actions[0] : null;
		if (nextAction == null) return false;
		_actions.Remove(nextAction);
		nextAction.Execute();
		return true;
	}

	public override void _Process(double delta)
	{
		while (NextAction())
		{
			
		}
	}
}
