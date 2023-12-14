using System.Collections.Generic;
using Godot;
using ProjectNostalgia.combat.health_component;

namespace ProjectNostalgia.combat.hit_manager_component;

public partial class HitManagerComponent : CombatComponent
{
	
	[Export] public HealthComponent HealthComponent;
	[Export] private Godot.Collections.Array<Node> _hitProcessorComponents = new();

	public void TakeHit(HitInfo info)
	{
		foreach (Node node in _hitProcessorComponents)
		{
			IHitProcessor hitProcessor = (IHitProcessor)node;
			hitProcessor.ProcessIncomingHitInfo(info);
		}

		foreach (DamageInfo damageInfo in info.DamageInfos)
		{
			HealthComponent.TakeDamage(damageInfo.Amount);
		}
	}
	
}