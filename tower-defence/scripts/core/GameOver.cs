using Godot;
using System;

public partial class GameOver : CanvasLayer
{
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
		Hide();
		GetNode<Button>("Panel/VBoxContainer/Button").Pressed += OnRestartPressed;
    }

	public void ShowGameover()
	{
		Show();
	}

	private void OnRestartPressed()
	{
		GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
	}

}
