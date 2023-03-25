using Godot;
using System;

public partial class TitleScreen : Node3D
{
	private Timer _lightTimer;
	private OmniLight3D _titlePlateLight;
	private Control _uiControl;

	private bool _moveLight = true;

	public override void _Ready()
	{
		_lightTimer = GetNode<Timer>("LightTimer");
		_titlePlateLight = GetNode<OmniLight3D>("TitlePlateLight");
		_uiControl = GetNode<Control>("UI");

		_moveLight = GameData.InitialLoad;
		_uiControl.Visible = !GameData.InitialLoad;
	}


	public override void _Process(double delta)
	{
		_titlePlateLight.Position = _moveLight ? new Vector3(0f, 5f * (float)_lightTimer.TimeLeft, 0f)
			: new Vector3(0f, 0f, 0.5f);
	}

	public void OnLightTimerTimeout()
	{
		_moveLight = false;
		_uiControl.Visible = true;
		GameData.InitialLoad = false;
	}

	public void OnStartButtonPressed()
	{
		GameData.Reset();
		GetTree().ChangeSceneToFile("uid://dh25ngn5r7g60");
	}

	public void OnChallengeButtonPressed()
	{
		GameData.Reset();
		GameData.CurrentLevel = 20;
		GetTree().ChangeSceneToFile("uid://dh25ngn5r7g60");
	}
}
