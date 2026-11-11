
using Godot;
using System.Collections.Generic;

public partial class Enemy : CharacterBody3D
{
    private Area3D _detectionArea;
    private Node3D _turret;
    private Marker3D _muzzle;
    private ShootComponent _shootComponent;

    private readonly List<Node3D> _targets = new();
    private Node3D _currentTarget;

    public override void _Ready()
    {
        _detectionArea = GetNode<Area3D>("DetectionArea");
        _turret = GetNode<Node3D>("Turret");
        _muzzle = GetNode<Marker3D>("Turret/Muzzle");
        _shootComponent = GetNode<ShootComponent>("ShootComponent");

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

        _turret.LookAt(
            _turret.GlobalPosition + direction,
            Vector3.Up
        );

        _shootComponent.TryShoot(
            _muzzle.GlobalPosition,
            direction,
            this
        );
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!IsValidTarget(body))
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

    private bool IsValidTarget(Node3D body)
    {
        return body.IsInGroup("player")
            || body.IsInGroup("towers")
            || body.IsInGroup("core");
    }

    private void UpdateTarget()
    {
        _targets.RemoveAll(target =>
            !GodotObject.IsInstanceValid(target));

        Node3D closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Node3D target in _targets)
        {
            float distance = GlobalPosition.DistanceSquaredTo(
                target.GlobalPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        _currentTarget = closestTarget;
    }
}


