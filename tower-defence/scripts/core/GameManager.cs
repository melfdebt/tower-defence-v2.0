using Godot;
using System;

public partial class GameManager : Node
{
	private HealthComponent _coreHealth;
	private HealthComponent _playerHealth;

	private GameOver _gameover;
	private bool _isGameOver=false;

    public override void _Ready()
    {
        _coreHealth = GetNode<HealthComponent>("../Entities/Core/HealthComponent");
		_coreHealth.Died+=OnGameOver;
		_playerHealth = GetNode<HealthComponent>("../Entities/Player/HealthComponent");

		_playerHealth.Died += OnGameOver;
		_gameover = GetNode<GameOver>("../GameOver");
		GD.Print("GM initialized");
    }

	private void OnGameOver()
	{
		if (_isGameOver) return;

		_isGameOver = true;

		GD.Print("gameover");
		_gameover.ShowGameover();

        GetTree().Paused = true;
	}

}
