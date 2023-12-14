using System;
using System.Collections.Generic;
using Godot;
using ProjectNostalgia.currency;
using ProjectNostalgia.utils;
using CurrencyData = ProjectNostalgia.currency.CurrencyData;

namespace ProjectNostalgia.player_inventories;

public partial class PlayerCurrencyInventory : Node
{
    public static PlayerCurrencyInventory Instance;

    public event Action<CurrencyType, int> CurrencyChanged;

    private Dictionary<CurrencyType, Currency> _currencies = new();

    public override void _Ready()
    {
        Instance = this;

        List<string> currencyDataPaths = DirectoryUtils.GetFilesInDirectory("res://currency/currencies");
        foreach (string path in currencyDataPaths)
        {
            CurrencyData currencyData = GD.Load<CurrencyData>(path);
            Currency currency = new Currency(currencyData);
            _currencies.Add(currencyData.Type, currency);
            GD.Print("Registered currency: ", currencyData.Type.ToString());
        }
    }

    public int GetCurrencyAmount(CurrencyType currency)
    {
        return _currencies[currency].Amount;
    }

    public CurrencyData GetCurrencyData(CurrencyType currency)
    {
        return _currencies[currency].CurrencyData;
    }

    private void SetCurrencyAmount(CurrencyType currency, int amount)
    {
        _currencies[currency].Amount = amount;
        CurrencyChanged?.Invoke(currency, amount);
        GD.Print(currency.ToString(), ": ", amount);
    }

    public void AddCurrency(Currency currency)
    {
        AddCurrency(currency.CurrencyData.Type, currency.Amount);
    }
    
    public void AddCurrency(CurrencyType currency, int amount)
    {
        SetCurrencyAmount(currency, GetCurrencyAmount(currency) + amount);
    }

    public void RemoveCurrency(Currency currency)
    {
        RemoveCurrency(currency.CurrencyData.Type, currency.Amount);
    }
    
    public bool RemoveCurrency(CurrencyType currency, int amount)
    {
        if (GetCurrencyAmount(currency) < amount) return false;
        SetCurrencyAmount(currency, GetCurrencyAmount(currency) - amount);
        return true;
    }
}