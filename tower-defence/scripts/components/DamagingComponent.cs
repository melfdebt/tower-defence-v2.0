using Godot;
using System;
[GlobalClass]
public partial class DamagingComponent : Area3D
{
	[Export]
	public float Damage=10.0f;

    protected void ApplyDamage(Node target)
    {
        Node current = target;

        while (current != null)
        {
            HealthComponent health =
                current.GetNodeOrNull<HealthComponent>("HealthComponent");

            if (health != null)
            {
                health.TakeDamage(Damage);
                return;
            }

            current = current.GetParent();
        }
    }

	public virtual void ApplyEffect(Node target)
	{
    	ApplyDamage(target);
	}

}
