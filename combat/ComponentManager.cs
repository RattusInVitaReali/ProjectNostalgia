using System.Collections.Generic;
using Godot;

namespace ProjectNostalgia.combat;

public class ComponentManager
{

    private List<CombatComponent> _components = new();

    public List<CombatComponent> GetComponents()
    {
        return _components;
    }

    public void RegisterComponent(CombatComponent component)
    {
        _components.Add(component);
    }

    public T GetComponent<T>() where T : Node
    {
        foreach (CombatComponent component in _components)
        {
            if (component is T tComponent) return tComponent;
        }
        return null;
    }
    
}