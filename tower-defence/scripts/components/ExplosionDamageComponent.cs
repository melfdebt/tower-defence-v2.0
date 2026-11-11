
using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class ExplosionDamageComponent : DamagingComponent
{
    [Export]
    public float ExplosionRadius = 3.0f;

    public override void ApplyEffect(Node target)
    {
        // Точка взрыва — место попадания снаряда
        Vector3 explosionPosition = GlobalPosition;

        // Создаём сферу для поиска объектов
        SphereShape3D sphere = new SphereShape3D();
        sphere.Radius = ExplosionRadius;

        // Настраиваем параметры поиска
        PhysicsShapeQueryParameters3D query =
            new PhysicsShapeQueryParameters3D();

        query.Shape = sphere;
        query.Transform = new Transform3D(
            Basis.Identity,
            explosionPosition
        );

        // Ищем физические тела в радиусе взрыва
        var spaceState = GetWorld3D().DirectSpaceState;
        var results = spaceState.IntersectShape(query, 32);

        HashSet<Node> damagedTargets = new HashSet<Node>();

        foreach (var result in results)
        {
            Node collider = result["collider"].AsGodotObject() as Node;

            if (collider == null)
                continue;

            // Ищем HealthComponent у объекта или его родителей
            Node current = collider;

            while (current != null)
            {
                HealthComponent health =
                    current.GetNodeOrNull<HealthComponent>("HealthComponent");

                if (health != null)
                {
                    if (damagedTargets.Add(current))
                        ApplyDamage(current);

                    break;
                }

                current = current.GetParent();
            }
        }
    }
}

