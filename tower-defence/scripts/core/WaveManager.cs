
using Godot;
[GlobalClass]
public partial class WaveManager : Node
{
	[Signal]
	public delegate void WaveChangedEventHandler(int wave);

	[Signal]
	public delegate void EnemiesCountChangedEventHandler(int count);

    [ExportGroup("Enemies")]
    [Export] public Godot.Collections.Array<PackedScene> EnemyScenes = new();

    [ExportGroup("Spawn Points")]
    [Export] public Godot.Collections.Array<Marker3D> SpawnPoints = new();

    [ExportGroup("Wave Settings")]
    [Export] public int EnemiesPerWave = 3;
    [Export] public int EnemyIncreasePerWave = 2;
    [Export] public float TimeBetweenWaves = 5.0f;

    private int _currentWave = 0;
    private int _aliveEnemies = 0;

    private Node3D _entities;
    private RandomNumberGenerator _random = new();

    private bool _isWaitingForNextWave = false;

    public override void _Ready()
    {
        _entities = GetNode<Node3D>("../Entities");

        _random.Randomize();

        if (EnemyScenes.Count == 0 || SpawnPoints.Count == 0)
        {
            GD.PrintErr("WaveManager: не назначены сцены врагов или точки появления!");
            return;
        }

        StartNextWave();
    }

    private void StartNextWave()
    {
        if (GetTree().Paused)
            return;

        _currentWave++;
		EmitSignal(SignalName.WaveChanged, _currentWave);

        int enemiesToSpawn =
            EnemiesPerWave + (_currentWave - 1) * EnemyIncreasePerWave;

        GD.Print($"Началась волна {_currentWave}. Врагов: {enemiesToSpawn}");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        int enemyIndex = _random.RandiRange(0, EnemyScenes.Count - 1);
        int spawnIndex = _random.RandiRange(0, SpawnPoints.Count - 1);

        PackedScene enemyScene = EnemyScenes[enemyIndex];
        Marker3D spawnPoint = SpawnPoints[spawnIndex];

        if (enemyScene == null || !GodotObject.IsInstanceValid(spawnPoint))
            return;

        Node3D enemy = enemyScene.Instantiate<Node3D>();

		enemy.AddToGroup("enemies");

		_entities.AddChild(enemy);
		enemy.GlobalPosition = spawnPoint.GlobalPosition;

        HealthComponent health =
            enemy.GetNodeOrNull<HealthComponent>("HealthComponent");

        if (health == null)
        {
            GD.PrintErr("У врага отсутствует HealthComponent!");
            enemy.QueueFree();
            return;
        }

        _aliveEnemies++;
		EmitSignal(SignalName.EnemiesCountChanged, _aliveEnemies);
        health.Died += OnEnemyDied;
    }

    private void OnEnemyDied()
    {
        _aliveEnemies--;
		EmitSignal(SignalName.EnemiesCountChanged, _aliveEnemies);

        GD.Print($"Осталось врагов: {_aliveEnemies}");

        if (_aliveEnemies <= 0 && !_isWaitingForNextWave)
        {
            _aliveEnemies = 0;
            _isWaitingForNextWave = true;
            WaitForNextWave();
        }
    }

    private async void WaitForNextWave()
    {
        GD.Print($"Следующая волна через {TimeBetweenWaves} секунд");

        await ToSignal(
            GetTree().CreateTimer(TimeBetweenWaves),
            SceneTreeTimer.SignalName.Timeout
        );

        _isWaitingForNextWave = false;

        if (!IsInsideTree() || GetTree().Paused)
            return;

        StartNextWave();
    }
}
