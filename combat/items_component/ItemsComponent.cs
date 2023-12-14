using System;
using Godot;
using Godot.Collections;
using ProjectNostalgia.combat.stats_component;
using ProjectNostalgia.item;
using ProjectNostalgia.player_inventories;

namespace ProjectNostalgia.combat.items_component;

public enum EquipmentSlot
{
    Null,
    Weapon1,
    Weapon2,
    BodyArmor,
    Helmet,
    Boots,
    Gloves
}

public partial class ItemsComponent : CombatComponent
{
    [Signal]
    public delegate void ItemsUpdatedEventHandler();

    [Export] private StatsComponent _stats;

    [Export] private Dictionary<EquipmentSlot, Item> _slots = new();

    public override void _Ready()
    {
        base._Ready();
        foreach (EquipmentSlot slot in Enum.GetValues<EquipmentSlot>())
        {
            if (slot == EquipmentSlot.Null) continue;
            _slots.Add(slot, null);
        }
    }

    public Item GetItem(EquipmentSlot slot)
    {
        return _slots[slot];
    }

    public void Equip(Item item)
    {
        EquipmentSlot slot = FindSlot(item, true);
        slot = slot == EquipmentSlot.Null ? FindSlot(item, false) : slot;
        GD.Print("Found slot: ", slot.ToString());
        Unequip(slot);
        AddChild(item);
        _slots[slot] = item;
        _stats.AddModifiers(item.GetModifiers());
        EmitSignal(SignalName.ItemsUpdated);
    }

    private EquipmentSlot FindSlot(Item item, bool mustBeEmpty)
    {
        foreach (EquipmentSlot slot in _slots.Keys)
        {
            if (mustBeEmpty && _slots[slot] != null) continue;
            if (item.ItemData.GetSlots().Contains(slot))
            {
                return slot;
            }
        }

        return EquipmentSlot.Null;
    }

    public void Unequip(Item item)
    {
        if (item == null) return;
        foreach (EquipmentSlot slot in item.ItemData.GetSlots())
        {
            if (GetItem(slot) == item)
            {
                Unequip(slot);
                return;
            }
        }
    }

    public void Unequip(EquipmentSlot slot)
    {
        Item item = _slots[slot];
        if (item == null) return;
        _slots[slot] = null;
        RemoveItem(item);
        EmitSignal(SignalName.ItemsUpdated);
    }

    private void RemoveItem(Item item)
    {
        RemoveChild(item);
        PlayerItemInventory.Instance.AddItem(item);
        _stats.RemoveModifiersFromSource(item);
    }
}