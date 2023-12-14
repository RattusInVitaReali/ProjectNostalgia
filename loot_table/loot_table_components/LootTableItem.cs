using Godot;
using ProjectNostalgia.item;
using ProjectNostalgia.reward;
using ProjectNostalgia.reward.rewards;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.loot_table.loot_table_components;

[GlobalClass]
public partial class LootTableItem : LootTableComponent
{
    
    [Export] private ItemData _item;
    
    public override Reward GetReward(int level)
    {
        Item item = Item.FromItemData(_item, level, Rarity.Legendary);
        return new ItemReward(item);
    }
}