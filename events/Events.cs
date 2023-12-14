using Godot;
using ProjectNostalgia.combat.items_component;
using ProjectNostalgia.entity.player;

namespace ProjectNostalgia.events;

public partial class Events : Node
{

    [Signal]
    public delegate void PlayerSpawnedEventHandler(Player player);
    
    public static Events Instance;

    public override void _Ready()
    {
        Instance = this;
    }
    
    
    
}