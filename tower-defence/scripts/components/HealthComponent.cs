using Godot;
using System;
[GlobalClass]
public partial class HealthComponent : Node
{
	[Export]
	public float MaxHealth=100.0f;

	[Export]
	public bool DestroyOwner=true;


	[Signal]
	public delegate void DiedEventHandler();

	[Signal]
	public delegate void HealthChangedEventHandler(	
    	float currentHealth,
    	float maxHealth
	);
	public float CurrentHealth {get;private set;}

	public bool isDied = false;

    public override void _Ready()
    {
		CurrentHealth=MaxHealth;
    }

	public void TakeDamage(float damage)
	{
		CurrentHealth-=damage;
		EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
		GD.Print($"{CurrentHealth}/{MaxHealth}");
		if (CurrentHealth <= 0)
		{
			Die();
		}

	}

	private void Die()
	{
		GD.Print($"{GetParent().Name} died");
		EmitSignal(SignalName.Died);
		isDied = true;
		if (DestroyOwner)
		{
			GetParent().QueueFree();
		}

	}


}
