using Godot;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.interfaces;

public interface ISlottable
{

    public Texture2D GetIcon();

    public Rarity GetRarity();
    
    public int GetAmount();

}