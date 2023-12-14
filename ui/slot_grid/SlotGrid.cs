using Godot;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.item;
using ProjectNostalgia.slottable_inventory;
using ProjectNostalgia.ui.slot;

namespace ProjectNostalgia.ui.slot_grid;

public partial class SlotGrid : GridContainer
{
    [Signal]
    public delegate void ItemSelectedEventHandler(Item item);

    private static PackedScene _slotScene = GD.Load<PackedScene>("res://ui/slot/slot.tscn");

    private SlottableInventory _inventory;

    public void SetInventory(SlottableInventory inventory)
    {
        _inventory = inventory;
        _inventory.SlottableAdded += _OnSlottableAdded;
        _inventory.SlottableRemoved += _OnSlottableRemoved;
    }

    public void AddSlottableToInventory(ISlottable slottable)
    {
        _inventory?.AddSlottable(slottable);
    }

    public void RemoveSlottableFromInventory(ISlottable slottable)
    {
        _inventory?.RemoveSlottable(slottable);
    }
    
    private void _OnSlottableAdded(ISlottable slottable)
    {
        Slot slot = _slotScene.Instantiate<Slot>();
        AddChild(slot);
        slot.SetSlottable(slottable);
        slot.SlottableSelected += _OnSlottableSelected;
    }

    private void _OnSlottableRemoved(ISlottable slottable)
    {
        foreach (Node child in GetChildren())
        {
            Slot slot = (Slot)child;
            if (slot.Slottable == slottable)
            {
                RemoveChild(child);
                return;
            }
        }
    }

    private void _OnSlottableSelected(ISlottable slottable)
    {
        EmitSignal(SignalName.ItemSelected, (Item)slottable);
    }
}