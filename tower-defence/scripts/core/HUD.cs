
using Godot;

public partial class HUD : CanvasLayer
{
    private Label _waveLabel;
    private Label _enemiesLabel;
    private Label _playerHealthLabel;
    private Label _coreHealthLabel;

    private WaveManager _waveManager;
    private HealthComponent _playerHealth;
    private HealthComponent _coreHealth;

    public override void _Ready()
    {
        // Получаем надписи
        _waveLabel = GetNode<Label>("UI/WaveLabel");
        _enemiesLabel = GetNode<Label>("UI/EnemiesLabel");
        _playerHealthLabel = GetNode<Label>("UI/HPLabel");
        _coreHealthLabel = GetNode<Label>("UI/CoreHealthLabel");

        // Получаем менеджер волн и компоненты здоровья
        _waveManager = GetNode<WaveManager>("../WaveManager");

        _playerHealth = GetNode<HealthComponent>(
            "../Entities/Player/HealthComponent"
        );

        _coreHealth = GetNode<HealthComponent>(
            "../Entities/Core/HealthComponent"
        );

        // Подписываемся на события волн
        _waveManager.WaveChanged += OnWaveChanged;
        _waveManager.EnemiesCountChanged += OnEnemiesCountChanged;

        // Подписываемся на изменение здоровья
        _playerHealth.HealthChanged += OnPlayerHealthChanged;
        _coreHealth.HealthChanged += OnCoreHealthChanged;

        // Показываем начальные значения
        OnWaveChanged(1);
        OnPlayerHealthChanged(
            _playerHealth.CurrentHealth,
            _playerHealth.MaxHealth
        );
        OnCoreHealthChanged(
            _coreHealth.CurrentHealth,
            _coreHealth.MaxHealth
        );
    }

    private void OnWaveChanged(int wave)
    {
        _waveLabel.Text = $"ВОЛНА: {wave}";
    }

    private void OnEnemiesCountChanged(int count)
    {
        _enemiesLabel.Text = $"ВРАГИ: {count}";
    }

    private void OnPlayerHealthChanged(float current, float max)
    {
        _playerHealthLabel.Text =
            $"ИГРОК: {current:0} / {max:0}";
    }

    private void OnCoreHealthChanged(float current, float max)
    {
        _coreHealthLabel.Text =
            $"ЯДРО: {current:0} / {max:0}";
    }
}

