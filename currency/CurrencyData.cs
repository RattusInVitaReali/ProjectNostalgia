using System;
using Godot;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.currency;

[GlobalClass]
public partial class CurrencyData : Resource
{

    [Export] public string Name;
    [Export] public Rarity Rarity;
    [Export] public Texture2D Icon;
    [Export] public Mesh Mesh;
    
    public CurrencyType Type => Enum.Parse<CurrencyType>(Name);

}