using Godot;
using ProjectNostalgia.item;
using ProjectNostalgia.player_inventories;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.reward.rewards;

public class ItemReward : Reward
{

    private Item _item;
    
    public ItemReward(Item item)
    {
        _item = item;
    }
    
    public override void Acquire()
    {
        PlayerItemInventory.Instance.AddItem(_item);
    }

    public override Rarity GetRarity()
    {
        return _item.Rarity;
    }

    public override MeshData GetMesh()
    {
        return new MeshData(_item.ItemData.Mesh, Vector3.Right * 90.0f, 125.0f);
    }

    public override Texture2D GetIcon()
    {
        return _item.GetIcon();
    }

    public override int GetAmount()
    {
        return _item.GetAmount();
    }
}