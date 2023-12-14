using System.Collections.Generic;

namespace ProjectNostalgia.utils;

public static class CollisionUtils
{
    
    public enum CollisionLayer
    {
        Ground,
        PlayerCollision,
        MonsterCollision,
        DestructibleCollision,
        PlayerHurtbox,
        MonsterHurtbox,
        DestructibleHurtbox
    }
    
    public enum OwnerType
    {
        Player,
        Monster,
        Destructible,
        Other
    }

    private static readonly Dictionary<CollisionLayer, int> CollisionLayerDict = new()
    {
        { CollisionLayer.Ground, 1 },
        { CollisionLayer.PlayerCollision, 2 },
        { CollisionLayer.MonsterCollision, 3 },
        { CollisionLayer.DestructibleCollision, 4 },
        { CollisionLayer.PlayerHurtbox, 5 },
        { CollisionLayer.MonsterHurtbox, 6 },
        { CollisionLayer.DestructibleHurtbox, 7 }
    };

    public static int GetLayer(CollisionLayer layer)
    {
        return CollisionLayerDict[layer];
    }
    
}