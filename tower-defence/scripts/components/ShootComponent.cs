using Godot;
using System;
[GlobalClass]
public partial class ShootComponent : Node
{
	[Export]
	public PackedScene BulletScene;

	[Export]
	public float FireRate=2.0f;

	private double _timeSinceLastShot;
	private bool _hasShot;

    public override void _Process(double delta)
    {
        _timeSinceLastShot+=delta;
    }

	public void TryShoot(Vector3 origin,Vector3 direction,Node3D owner)
	{
		if (FireRate<=0 || BulletScene==null) return ;

		if (_hasShot && _timeSinceLastShot < 1.0 / FireRate) return ;

		Bullet bullet = BulletScene.Instantiate<Bullet>();

		bullet.Setup(direction, owner);

		GetTree().CurrentScene.AddChild(bullet);

		bullet.GlobalPosition = origin;

		_timeSinceLastShot=0;
		_hasShot=true;
	}

}
