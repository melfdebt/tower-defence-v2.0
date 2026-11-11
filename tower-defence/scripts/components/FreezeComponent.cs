using Godot;
using System;

[GlobalClass]
public partial class FreezeComponent : DamagingComponent
{
	[Export]
	public float FreezeDuration = 3.0f;

    public override void ApplyEffect(Node target)
    {
		base.ApplyEffect(target);
        Node current = target;
		while (current != null)
		{
			MoveComponent move  = current.GetNodeOrNull<MoveComponent>("MoveComponent");
			if (move != null)
			{
				move.Freeze(FreezeDuration);
				return;
			}
			current = current.GetParent();
		}
    }


}
