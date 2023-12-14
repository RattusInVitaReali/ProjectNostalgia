using Godot;
using ProjectNostalgia.combat.action_queue_component;
using ProjectNostalgia.interfaces;
using HitManagerComponent = ProjectNostalgia.combat.hit_manager_component.HitManagerComponent;

namespace ProjectNostalgia.combat;

public class HitAction : CombatAction
{

    public HitInfo HitInfo;
    
    public HitAction(IComponentContainer source, IComponentContainer target, HitInfo info) : base(source, target)
    {
        HitInfo = info;
    }

    public override void Execute()
    {
        HitManagerComponent hitManager = Target.GetManager().GetComponent<HitManagerComponent>();
        hitManager!.TakeHit(HitInfo);
    }
    
}