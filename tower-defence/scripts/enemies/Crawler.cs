
using Godot;

public partial class Crawler : CharacterBody3D
{
    private Timer _damageTimer;
    private DamagingComponent _damageComponent;

    private Node3D _target;
    private HealthComponent _targetHealth;

    public override void _Ready()
    {
        _damageComponent =
            GetNode<DamagingComponent>("DamagingComponent");

        _damageTimer = GetNode<Timer>("DamageTimer");

        _damageComponent.BodyEntered += OnBodyEntered;
        _damageComponent.BodyExited += OnBodyExited;

        _damageTimer.Timeout += OnDamageTimerTimeout;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup("core"))
            return;

        if (_target == body)
            return;

        _target = body;
        _targetHealth = body.GetNode<HealthComponent>("HealthComponent");

        _targetHealth.Died += OnTargetDied;
        _damageComponent.ApplyEffect(_target);
        _damageTimer.Start();
    }

    private void OnBodyExited(Node3D body)
    {
        if (body != _target)
            return;

        StopAttacking();
    }

    private void OnDamageTimerTimeout()
    {
        if (_target == null ||
            !GodotObject.IsInstanceValid(_target) ||
            _targetHealth == null ||
            !GodotObject.IsInstanceValid(_targetHealth) ||
            _targetHealth.isDied)
        {
            StopAttacking();
            return;
        }

        _damageComponent.ApplyEffect(_target);
    }

    private void OnTargetDied()
    {
        StopAttacking();
    }

    private void StopAttacking()
    {
        _damageTimer.Stop();

        if (_targetHealth != null && GodotObject.IsInstanceValid(_targetHealth))
        {
            _targetHealth.Died -= OnTargetDied;
        }

        _target = null;
        _targetHealth = null;
    }
}

