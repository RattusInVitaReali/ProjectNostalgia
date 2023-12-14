using System;
using Godot;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.ui.slot;

public partial class Slot : Panel
{

    // Cannot be [Signal] because fuck you if you think you can use interfaces with Godot
    public event Action<ISlottable> SlottableSelected;
    
    private static readonly Color EmptyColor = new(230, 166, 71);

    [Export] private Texture2D _emptyTextureOverride;
    
    private TextureButton _icon;

    public ISlottable Slottable { get; private set; }

    public override void _Ready()
    {
        _icon = GetNode<TextureButton>("%Icon");
        SetSlottable(null);

        // Testing
        // SetSlottable(Item.GenerateTestItem());
    }

    public void SetSlottable(ISlottable slottable)
    {
        if (slottable == Slottable && Slottable != null) return;
        Slottable = slottable;
        if (Slottable == null)
        {
            SetTexture(_emptyTextureOverride);
            SetBackgroundColor(EmptyColor);
        }
        else
        {
            SetTexture(Slottable.GetIcon());
            SetBackgroundColor(Slottable.GetRarity());
        }
    }

    private void SetTexture(Texture2D texture)
    {
        _icon.TextureNormal = texture;
    }

    private void SetBackgroundColor(Rarity rarity)
    {
        Color color = RarityUtils.GetLightenedColor(rarity);
        SetBackgroundColor(color);
    }

    private void SetBackgroundColor(Color color)
    {
        StyleBoxFlat styleBox = (StyleBoxFlat)Get("theme_override_styles/panel");
        styleBox.BorderColor = color;
        styleBox.BgColor = color with { A = 0.5f };
    }

    private void _OnIconPressed()
    {
        if (Slottable == null) return;
        SlottableSelected?.Invoke(Slottable);
    }
}