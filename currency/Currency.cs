using Godot;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.currency;

public enum CurrencyType
{
    Gold,
    Silver, // Temp
    Bronze // Temp
}

public class Currency : ISlottable
{
    public currency.CurrencyData CurrencyData { get; private set; }
    public int Amount;

    public Currency(currency.CurrencyData currencyData, int amount = 0)
    {
        CurrencyData = currencyData;
        Amount = amount;
    }

    public Texture2D GetIcon()
    {
        return CurrencyData.Icon;
    }

    public Rarity GetRarity()
    {
        return CurrencyData.Rarity;
    }

    public int GetAmount()
    {
        return Amount;
    }
}