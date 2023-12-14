using System;
using System.Collections.Generic;
using Godot;
using ProjectNostalgia.combat.items_component;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.item;
using ProjectNostalgia.ui.slot;

namespace ProjectNostalgia.ui.equipment;

public partial class Equipment : GridContainer
{
    [Signal]
    public delegate void ItemSelectedEventHandler(Item item);

    private ItemsComponent _itemsComponent;
    private Dictionary<EquipmentSlot, Slot> _slots = new();

    public override void _Ready()
    {
        foreach (EquipmentSlot slot in Enum.GetValues<EquipmentSlot>())
        {
            Slot slotNode = GetNodeOrNull<Slot>("%" + slot);
            if (slotNode == null) continue;
            GD.Print("Registered slot: ", slot.ToString());
            slotNode.SlottableSelected += _OnItemSelected;
            _slots.Add(slot, slotNode);
        }
    }

    public void SetItemsComponent(ItemsComponent itemsComponent)
    {
        _itemsComponent = itemsComponent;
        _itemsComponent.ItemsUpdated += UpdateItems;
    }

    public void Equip(Item item)
    {
        _itemsComponent?.Equip(item);
    }

    public void Unequip(Item item)
    {
        _itemsComponent?.Unequip(item);
    }

    private void UpdateItems()
    {
        foreach (EquipmentSlot equipmentSlot in _slots.Keys)
        {
            _slots[equipmentSlot].SetSlottable(_itemsComponent.GetItem(equipmentSlot));
        }
    }

    private void _OnItemSelected(ISlottable slottable)
    {
        EmitSignal(SignalName.ItemSelected, (Item)slottable);
    }
}