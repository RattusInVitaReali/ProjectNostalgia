using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using ProjectNostalgia.combat.items_component;

namespace ProjectNostalgia.item;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public Mesh Mesh;
    [Export] public Array<string> Slots = new();
    [Export] public Item.Type Type;
    [Export] public Texture2D Icon;
    [Export] public Array<affix.AffixData> Affixes = new();

    private List<EquipmentSlot> _slots = null;

    public List<EquipmentSlot> GetSlots()
    {
        if (_slots == null)
        {
            _slots = new();
            foreach (string slot in Slots)
            {
                _slots.Add(Enum.Parse<EquipmentSlot>(slot));
            }
        }

        return _slots;
    }
}