
using Godot;

public partial class BuildingManager : Node3D
{
    [Export] public PackedScene PreviewScene;
    [Export] public PackedScene TowerScene;

    [ExportGroup("Tower Types")]
    [Export] public PackedScene BasicTowerScene;
    [Export] public PackedScene FreezeTowerScene;
    [Export] public PackedScene ExplosionTowerScene;

    [ExportGroup("Placement")]
    [Export] public Vector3 PlacementCheckSize = new Vector3(2, 3, 2);
    [Export] public float PlacementCheckHeight = 1.5f;
    [Export] public Vector2 ArenaHalfSize = new Vector2(17.75f, 9.75f);

    [ExportGroup("Preview Materials")]
    [Export] public Material ValidMaterial;
    [Export] public Material InvalidMaterial;

    private Node3D _player;
    private Node3D _preview;

    private MeshInstance3D _previewBase;
    private MeshInstance3D _previewTurret;

    private Label3D _towerNameLabel;

    public override void _Ready()
    {
        _player = GetNode<Node3D>("../Entities/Player");
        _towerNameLabel = GetNode<Label3D>("TowerNameLabel");

        // Обычная башня выбрана по умолчанию.
        TowerScene = BasicTowerScene;
        _towerNameLabel.Text = "Basic Tower";
        _towerNameLabel.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("build_mode"))
        {
            ToggleBuildMode();
        }

        if (Input.IsActionJustPressed("cancel_build"))
        {
            CancelBuildMode();
        }

        if (_preview == null)
            return;

        if (Input.IsActionJustPressed("select_basic_tower"))
        {
            SelectTower(BasicTowerScene, "Basic Tower");
        }
        else if (Input.IsActionJustPressed("select_freeze_tower"))
        {
            SelectTower(FreezeTowerScene, "Freeze Tower");
        }
        else if (Input.IsActionJustPressed("select_explosion_tower"))
        {
            SelectTower(ExplosionTowerScene, "Explosion Tower");
        }

        UpdatePreviewPosition();
        UpdatePreviewAppearance(CanPlaceTower());

        _towerNameLabel.GlobalPosition =
            _preview.GlobalPosition + Vector3.Up * 2.5f;
    }

    private void ToggleBuildMode()
    {
        if (_preview == null)
        {
            if (PreviewScene == null)
            {
                GD.PrintErr("PreviewScene не назначена!");
                return;
            }

            _preview = PreviewScene.Instantiate<Node3D>();
            AddChild(_preview);

            GetPreviewMeshes();

            _towerNameLabel.Visible = true;
            UpdatePreviewPosition();
            UpdatePreviewAppearance(CanPlaceTower());
        }
        else
        {
            PlaceTower();
        }
    }

    private void SelectTower(PackedScene towerScene, string towerName)
    {
        if (towerScene == null)
        {
            GD.PrintErr("Сцена выбранной башни не назначена!");
            return;
        }

        TowerScene = towerScene;
        _towerNameLabel.Text = towerName;
    }

    private void GetPreviewMeshes()
    {
        _previewBase =
            _preview.GetNode<MeshInstance3D>("MeshInstance3D");

        _previewTurret =
            _preview.GetNode<MeshInstance3D>("Turret/MeshInstance3D");
    }

    private void UpdatePreviewAppearance(bool canPlace)
    {
        Material material = canPlace ? ValidMaterial : InvalidMaterial;

        _previewBase.MaterialOverride = material;
        _previewTurret.MaterialOverride = material;
    }

    private void PlaceTower()
    {
        if (!CanPlaceTower())
        {
            GD.Print("Здесь нельзя установить башню!");
            return;
        }

        if (TowerScene == null)
        {
            GD.PrintErr("TowerScene не назначена!");
            return;
        }

        Node3D tower = TowerScene.Instantiate<Node3D>();

		tower.AddToGroup("towers");

		Node3D entities = GetNode<Node3D>("../Entities");
		entities.AddChild(tower);
        tower.GlobalPosition = _preview.GlobalPosition;

        _preview.QueueFree();
        _preview = null;

        _towerNameLabel.Visible = false;
    }

    private void CancelBuildMode()
    {
        if (_preview == null)
            return;

        _preview.QueueFree();
        _preview = null;

        _towerNameLabel.Visible = false;
    }

    private void UpdatePreviewPosition()
    {
        Vector3 forward = -_player.GlobalBasis.Z;
        forward.Y = 0;

        _preview.GlobalPosition =
            _player.GlobalPosition + forward.Normalized() * 3.0f;
    }

    private bool CanPlaceTower()
    {
        Vector3 position = _preview.GlobalPosition;

        float halfWidth = PlacementCheckSize.X / 2;
        float halfDepth = PlacementCheckSize.Z / 2;

        // Проверка границ арены.
        if (Mathf.Abs(position.X) + halfWidth > ArenaHalfSize.X ||
            Mathf.Abs(position.Z) + halfDepth > ArenaHalfSize.Y)
        {
            return false;
        }

        // Проверка физических пересечений.
        BoxShape3D shape = new BoxShape3D();
        shape.Size = PlacementCheckSize;

        PhysicsShapeQueryParameters3D query =
            new PhysicsShapeQueryParameters3D();

        query.Shape = shape;

        query.Transform = new Transform3D(
            Basis.Identity,
            _preview.GlobalPosition + Vector3.Up * PlacementCheckHeight
        );

        // Player (2), Enemies (3), Towers (4), Core (6).
        query.CollisionMask =
            (1u << 1) |
            (1u << 2) |
            (1u << 3) |
            (1u << 5);

        var spaceState = GetWorld3D().DirectSpaceState;
        var results = spaceState.IntersectShape(query, 32);

        return results.Count == 0;
    }
}
