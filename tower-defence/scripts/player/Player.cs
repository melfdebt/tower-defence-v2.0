
using Godot;

public partial class Player : CharacterBody3D
{
    [Export]
    public float MoveSpeed = 5.0f;

    [Export]
    public float BulletSpawnOffset = 1.2f;

    private ShootComponent _shootComponent;

    public override void _Ready()
    {
        _shootComponent = GetNode<ShootComponent>("ShootComponent");
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement();
        RotateTowardsMouse();

        if (Input.IsActionPressed("shoot"))
            Shoot();
    }

    private void HandleMovement()
    {
        Vector2 input = Input.GetVector(
            "move_left",
            "move_right",
            "move_forward",
            "move_back"
        );

        Vector3 direction = new Vector3(input.X, 0, input.Y);

        Velocity = direction.Normalized() * MoveSpeed;

        MoveAndSlide();
    }

    private void RotateTowardsMouse()
    {
        Vector3? target = GetMouseTarget();

        if (target == null)
            return;

        Vector3 direction = target.Value - GlobalPosition;
        direction.Y = 0;

        if (direction.LengthSquared() < 0.001f)
            return;

        LookAt(GlobalPosition + direction, Vector3.Up);
    }

    private void Shoot()
    {
        Vector3? target = GetMouseTarget();

        if (target == null)
            return;

        Vector3 direction = target.Value - GlobalPosition;
        direction.Y = 0;

        if (direction.LengthSquared() < 0.001f)
            return;

        direction = direction.Normalized();

        Vector3 origin =
            GlobalPosition
            + Vector3.Up * 0.9f
            + direction * BulletSpawnOffset;

        _shootComponent.TryShoot(origin, direction, this);
    }

    private Vector3? GetMouseTarget()
    {
        Camera3D camera = GetViewport().GetCamera3D();

        Vector2 mousePosition = GetViewport().GetMousePosition();

        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePosition);

        Plane groundPlane = new Plane(Vector3.Up, GlobalPosition.Y);

        return groundPlane.IntersectsRay(
            rayOrigin,
            rayDirection
        );
    }
}
