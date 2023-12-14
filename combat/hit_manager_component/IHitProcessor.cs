namespace ProjectNostalgia.combat.hit_manager_component;

public interface IHitProcessor
{

    public int HitProcessorPriority();

    // Override for weird Hit stuff (damage conversion etc.)
    public void ProcessIncomingHitInfo(HitInfo hitInfo)
    {
        foreach (DamageInfo info in hitInfo.DamageInfos)
        {
            ProcessIncomingDamageInfo(info);
        }
    }

    public void ProcessIncomingDamageInfo(DamageInfo info);

    public void ProcessOutgoingHitInfo(HitInfo hitInfo)
    {
        foreach (DamageInfo info in hitInfo.DamageInfos)
        {
            ProcessOutgoingDamageInfo(info);
        }
    }

    public void ProcessOutgoingDamageInfo(DamageInfo info);

    
}