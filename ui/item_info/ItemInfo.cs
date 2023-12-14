using Godot;
using ProjectNostalgia.combat.stats_component;
using ProjectNostalgia.item;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.ui.item_info;

public partial class ItemInfo : Panel
{

    private static PackedScene _statModifierLabelScene = GD.Load<PackedScene>("res://ui/item_info/stat_modifier_label.tscn");

    private Label _name;
    private Label _level;
    private TextureRect _icon;
    private VBoxContainer _statsContainer;
    
    public override void _Ready()
    {
        _name = GetNode<Label>("%Name");
        _level = GetNode<Label>("%Level");
        _icon = GetNode<TextureRect>("%Icon");
        _statsContainer = GetNode<VBoxContainer>("%StatsContainer");
        
        // Testing:
        // SetItem(Item.GenerateTestItem());
    }

    public void SetItem(Item item)
    {
        if (item == null) return;
        _name.Text = item.ItemName;
        _level.Text = "Level " + item.Level;
        _icon.Texture = item.ItemData.Icon;
        SetStats(item);
        SetColors(item.Rarity);
    }

    private void SetStats(Item item)
    {
        foreach (Node stat in _statsContainer.GetChildren())
        {
            _statsContainer.RemoveChild(stat);
        }

        foreach (StatModifier stat in item.GetModifiers())
        {
            Label statLabel = _statModifierLabelScene.Instantiate<Label>();
            statLabel.Text = stat.GetDescription();
            statLabel.LabelSettings.FontColor = RarityUtils.GetLightenedColor(stat.Rarity);
            _statsContainer.AddChild(statLabel);
        }
    }

    private void SetColors(Rarity rarity)
    {
        Color color = RarityUtils.GetLightenedColor(rarity);
        SetColors(color);
    }
    
    private void SetColors(Color color)
    {
        _name.LabelSettings.FontColor = color;
        StyleBoxFlat styleBox = (StyleBoxFlat)Get("theme_override_styles/panel");
        styleBox.BorderColor = color;
    }
    
}
