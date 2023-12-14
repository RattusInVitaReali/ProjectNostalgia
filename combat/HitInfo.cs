using System.Collections.Generic;
using Godot;
using ProjectNostalgia.entity;
using ProjectNostalgia.interfaces;

namespace ProjectNostalgia.combat;

public class HitInfo
{
    
    public readonly List<DamageInfo> DamageInfos = new();
    public IComponentContainer Source;
    public IComponentContainer Target;
    
    public HitInfo(IComponentContainer source, IComponentContainer target, IEnumerable<DamageInfo> damageInfos)
    {
        Source = source;
        Target = target;
        foreach (DamageInfo info in damageInfos)
        {
            DamageInfos.Add(info);
        }
    }

    public void AddDamageInfo(DamageInfo info)
    {
        DamageInfos.Add(info);
    }
    
}