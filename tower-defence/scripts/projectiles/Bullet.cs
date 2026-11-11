using Godot;

public partial class Bullet : Area3D
{
    [Export]
    public float Speed = 15.0f;

    private Vector3 _direction;
    private Node3D _owner;
    private DamagingComponent _damagingComponent;
	private bool _hashit=false;

    public void Setup(Vector3 direction, Node3D owner)
    {
        _direction = direction.Normalized();
        _owner = owner;
    }

    public override void _Ready()
    {
        _damagingComponent = GetNode<DamagingComponent>("DamagingComponent");

        _damagingComponent.BodyEntered += OnBodyEntered;
        _damagingComponent.AreaEntered += OnAreaEntered;

        VisibleOnScreenNotifier3D notifier = GetNode<VisibleOnScreenNotifier3D>("VisibleOnScreenNotifier3D");

        notifier.ScreenExited += () => QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += _direction * Speed * (float)delta;
    }

    private void OnBodyEntered(Node3D body)
    {
        HandleCollision(body);
    }

    private void OnAreaEntered(Area3D area)
    {
        HandleCollision(area);
    }

    private void HandleCollision(Node target)
    {
		if (_hashit) return;
        if (target == _owner || _owner.IsAncestorOf(target))
            return;
		_hashit = true;
        _damagingComponent.ApplyEffect(target);
        QueueFree();
    }
}
