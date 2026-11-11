using Godot;
using System;

[GlobalClass]
public partial class MoveComponent : Node
{
	[Export]
	public float speed = 3.0f;
	[Export]
	public float stopDistance = 1.5f;
	[Export]
	public NodePath targetPath  = "../../Core";
	public bool IsMovementDisabled { get; set; } = false;
	private CharacterBody3D _body;
	private Node3D _target;
	private double _freezeTimer = 0;

    public override void _Ready()
    {
        _body = GetParent<CharacterBody3D>();

		_target = GetNodeOrNull<Node3D>(targetPath);

		if (_target == null)
		{
			GD.Print("core isnt found");
		}
    }

	public void Freeze(float duration)
	{
    	_freezeTimer = Mathf.Max((float)_freezeTimer, duration);
    	IsMovementDisabled = true;
	}

    public override void _PhysicsProcess(double delta)
    {
		if (_freezeTimer > 0)
		{
    		_freezeTimer -= delta;

    		if (_freezeTimer <= 0)
    		{
        		_freezeTimer = 0;
        		IsMovementDisabled = false;
    		}
		}

		if (IsMovementDisabled)
		{
    		_body.Velocity = Vector3.Zero;
    		_body.MoveAndSlide();
    		return;
		}	
        if (_target==null) return ;
		Vector3 direction = _target.GlobalPosition - _body.GlobalPosition;
		direction.Y=0;
		float distance = direction.Length();
		if (distance <= stopDistance)
		{
			_body.Velocity = Vector3.Zero;
		}
		else
		{
			direction = direction.Normalized();
			_body.Velocity = direction * speed;
			_body.MoveAndSlide();
		}
    }


}
