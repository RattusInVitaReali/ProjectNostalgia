using System;
using Godot;
using ProjectNostalgia.reward;
using ProjectNostalgia.reward.rewards;
using CurrencyData = ProjectNostalgia.currency.CurrencyData;

namespace ProjectNostalgia.loot_table.loot_table_components;

[GlobalClass]
public partial class LootTableCurrency : LootTableComponent
{
    private static Random _random = new Random();

    [Export] private CurrencyData _currency;
    [Export] private int _minAmount = 5;
    [Export] private int _minAmountPerLevel = 1;
    [Export] private int _maxAmount = 15;
    [Export] private int _maxAmountPerLevel = 3;

    public override Reward GetReward(int level)
    {
        int minAmount = _minAmount + level * _minAmountPerLevel;
        int maxAmount = _maxAmount + level * _maxAmountPerLevel;
        int amount = minAmount + _random.Next() % (maxAmount - minAmount);

        return new CurrencyReward(_currency, amount);
    }
}