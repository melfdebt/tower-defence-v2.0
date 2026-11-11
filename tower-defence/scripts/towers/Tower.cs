using Godot;
using System.Collections.Generic;

public partial class Tower : StaticBody3D
{
    private ShootComponent _shootComponent;
    private Area3D _detectionArea;
    private Node3D _turret;
    private Marker3D _muzzle;

    private readonly List<Node3D> _targets = new();
    private Node3D _currentTarget;

    public override void _Ready()
    {
        _shootComponent = GetNode<ShootComponent>("ShootComponent");
        _detectionArea = GetNode<Area3D>("DetectionArea");
        _turret = GetNode<Node3D>("Turret");
        _muzzle = GetNode<Marker3D>("Turret/Muzzle");

        _detectionArea.BodyEntered += OnBodyEntered;
        _detectionArea.BodyExited += OnBodyExited;
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateTarget();

        if (_currentTarget == null)
            return;

        Vector3 direction =
            _currentTarget.GlobalPosition - _turret.GlobalPosition;

        direction.Y = 0;

        if (direction.LengthSquared() < 0.001f)
            return;

        direction = direction.Normalized();

        // Поворачиваем только пушку.
        _turret.LookAt(
            _turret.GlobalPosition + direction,
            Vector3.Up
        );

        // Выстрел из конца ствола.
        _shootComponent.TryShoot(
            _muzzle.GlobalPosition,
            direction,
            this
        );
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup("enemies"))
            return;

        if (!_targets.Contains(body))
            _targets.Add(body);
    }

    private void OnBodyExited(Node3D body)
    {
        _targets.Remove(body);

        if (_currentTarget == body)
            _currentTarget = null;
    }

    private void UpdateTarget()
    {
        _targets.RemoveAll(target =>
            !GodotObject.IsInstanceValid(target)
        );

        if (_targets.Count == 0)
        {
            _currentTarget = null;
            return;
        }

        _currentTarget = _targets[0];
    }
}
