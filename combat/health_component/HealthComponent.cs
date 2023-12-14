using System;
using Godot;
using ProjectNostalgia.combat.stats_component;

namespace ProjectNostalgia.combat.health_component;

public partial class HealthComponent : CombatComponent
{

    [Signal]
    public delegate void DiedEventHandler();

    [Export] public float MaxHealth;
    [Export] private StatsComponent _statsComponent;
    
    private float _health;
    private bool _dead = false;

    public override void _Ready()
    {
        _health = MaxHealth;
    }

    public override void _Process(double delta)
    {
        if (_statsComponent != null)
        {
            SetMaxHealth(_statsComponent.GetStat("MaxHP"));
        }
    }
    
    public void SetMaxHealth(float maxHealth)
    {
        float diff = maxHealth - MaxHealth;
        MaxHealth = maxHealth;
        if (diff > 0) Heal(diff);
    }
    
    public void TakeDamage(float amount)
    {
        if (_dead) return;
        _health -= amount;
        if (_health <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (_dead) return;
        _health = _health + amount > MaxHealth ? MaxHealth : _health + amount;
    }

    private void Die()
    {
        _dead = true;
        EmitSignal("Died");
    }

}