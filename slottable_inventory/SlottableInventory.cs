using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectNostalgia.interfaces;

namespace ProjectNostalgia.slottable_inventory;

public partial class SlottableInventory : Node3D
{

    public event Action<ISlottable> SlottableAdded;
    public event Action<ISlottable> SlottableRemoved;
    
    public void AddSlottable(ISlottable slottable)
    {
        AddChild(slottable as Node);
        SlottableAdded?.Invoke(slottable);
    }

    public void RemoveSlottable(ISlottable slottable)
    {
        RemoveChild(slottable as Node);
        SlottableRemoved?.Invoke(slottable);
    }
    
    public IEnumerable<ISlottable> GetSlottables()
    {
        return GetChildren().Cast<ISlottable>();
    }
    
}