using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectNostalgia.item;
using ProjectNostalgia.slottable_inventory;

namespace ProjectNostalgia.player_inventories;

public partial class PlayerItemInventory : Node
{

    [Signal]
    public delegate void ItemAddedEventHandler(Item item);
    
    private static PackedScene _slottableInventoryScene = GD.Load<PackedScene>("res://slottable_inventory/slottable_inventory.tscn");
    
    public static PlayerItemInventory Instance;
    
    private SlottableInventory _inventory;
    
    
    public override void _Ready()
    {
        Instance = this;
        _inventory = _slottableInventoryScene.Instantiate<SlottableInventory>();
        AddChild(_inventory);
    }

    public void AddItem(Item item)
    {
        _inventory.AddSlottable(item);
        EmitSignal(SignalName.ItemAdded, item);
    }

    public SlottableInventory GetInventory()
    {
        return _inventory;
    }
}