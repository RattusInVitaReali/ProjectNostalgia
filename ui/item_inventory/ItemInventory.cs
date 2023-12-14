using Godot;
using ProjectNostalgia.combat.items_component;
using ProjectNostalgia.entity.player;
using ProjectNostalgia.events;
using ProjectNostalgia.item;
using ProjectNostalgia.player_inventories;
using ProjectNostalgia.ui.equipment;
using ProjectNostalgia.ui.item_info;
using ProjectNostalgia.ui.slot_grid;

namespace ProjectNostalgia.ui.item_inventory;

public partial class ItemInventory : Control
{
    private Equipment _equipment;
    private Button _equipmentButton;
    private SlotGrid _slotGrid;
    private ItemInfo _itemInfo;

    private Item _selectedItem;

    private ButtonState _buttonState;

    public override void _Ready()
    {
        _equipment = GetNode<Equipment>("%Equipment");
        _equipmentButton = GetNode<Button>("%EquipmentButton");
        _slotGrid = GetNode<SlotGrid>("%SlotGrid");
        _itemInfo = GetNode<ItemInfo>("%ItemInfo");
        
        _slotGrid.SetInventory(PlayerItemInventory.Instance.GetInventory());
        Events.Instance.PlayerSpawned += _SetPlayer;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_inventory"))
        {
            Visible = !Visible;
        }

        // Testing
        // if (@event.IsActionPressed("ui_attack"))
        // {
        //     PlayerItemInventory.Instance.AddItem(Item.GenerateTestItem());
        // }
    }

    private void ChangeButtonState(ButtonState newState)
    {
        _buttonState = newState;
        _buttonState.SetButtonParams();
    }

    private void _OnEquipmentItemSelected(Item item)
    {
        ChangeButtonState(new UnequipState(this));
        SelectItem(item);
    }

    private void _OnInventoryItemSelected(Item item)
    {
        ChangeButtonState(new EquipState(this));
        SelectItem(item);
    }

    private void SelectItem(Item item)
    {
        _selectedItem = item;
        _itemInfo.SetItem(item);
    }

    private void _OnEquipmentButtonPressed()
    {
        _buttonState.ButtonAction();
    }

    private void _SetPlayer(Player player)
    {
        _equipment.SetItemsComponent(player.GetManager().GetComponent<ItemsComponent>());
    }

    private abstract class ButtonState
    {
        protected ItemInventory ItemInventory;

        public ButtonState(ItemInventory itemInventory)
        {
            ItemInventory = itemInventory;
        }

        public abstract void ButtonAction();
        public abstract void SetButtonParams();
    }

    private class EquipState : ButtonState
    {
        public override void ButtonAction()
        {
            if (ItemInventory._selectedItem == null) return;
            ItemInventory._slotGrid.RemoveSlottableFromInventory(ItemInventory._selectedItem);
            ItemInventory._equipment.Equip(ItemInventory._selectedItem);
        }

        public override void SetButtonParams()
        {
            ItemInventory._equipmentButton.Text = "Equip";
        }

        public EquipState(ItemInventory itemInventory) : base(itemInventory)
        {
        }
    }

    private class UnequipState : ButtonState
    {
        public override void ButtonAction()
        {
            if (ItemInventory._selectedItem == null) return;
            ItemInventory._equipment.Unequip(ItemInventory._selectedItem);
        }

        public override void SetButtonParams()
        {
            ItemInventory._equipmentButton.Text = "Unequip";
        }

        public UnequipState(ItemInventory itemInventory) : base(itemInventory)
        {
        }
    }
} 