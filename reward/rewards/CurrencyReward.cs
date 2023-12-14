using System;
using Godot;
using ProjectNostalgia.currency;
using ProjectNostalgia.player_inventories;
using ProjectNostalgia.utils;
using CurrencyData = ProjectNostalgia.currency.CurrencyData;

namespace ProjectNostalgia.reward.rewards;

public class CurrencyReward : Reward
{
    private Currency _currency;

    public CurrencyReward(CurrencyData currencyData, int amount)
    {
        _currency = new Currency(currencyData, amount);
    }

    public override void Acquire()
    {
        PlayerCurrencyInventory.Instance.AddCurrency(_currency);
    }

    public override Rarity GetRarity()
    {
        return _currency.CurrencyData.Rarity;
    }

    public override MeshData GetMesh()
    {
        return new MeshData(_currency.CurrencyData.Mesh, Vector3.Zero, 0.75f);
    }

    public override Texture2D GetIcon()
    {
        return _currency.GetIcon();
    }

    public override int GetAmount()
    {
        return _currency.GetAmount();
    }
}