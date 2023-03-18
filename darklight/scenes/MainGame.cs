using Godot;

public partial class MainGame : Node3D
{
	private WorldLight _mainLight;
	private Timer _toggleTimer;

	private Label _timerLabel;
	private TextureProgressBar _lightMeter;

	public override void _Ready()
	{
		_mainLight = GetNode<WorldLight>("MainLight");
		_toggleTimer = GetNode<Timer>("MainLight/ToggleTimer");

		_timerLabel = GetNode<Label>("UI/TimerLabel");
		_lightMeter = GetNode<TextureProgressBar>("UI/LightMeter");


		_lightMeter.MinValue = 0;
		_lightMeter.MaxValue = _toggleTimer.WaitTime * 100;

		// _toggleTimer.Start();
	}

	public override void _Process(double delta)
	{
		_timerLabel.Text = $"{_toggleTimer.TimeLeft}";

		if (_mainLight.IsLightBright)
		{
			_lightMeter.Value = _toggleTimer.TimeLeft * 100;
		}
		else
		{
			_lightMeter.Value = (_toggleTimer.WaitTime - _toggleTimer.TimeLeft) * 100;
		}
	}

	private void OnToggleTimerTimeout()
	{
		_mainLight.ToggleLight();

		if (_mainLight.IsLightBright)
		{
			_lightMeter.FillMode = (int)TextureProgressBar.FillModeEnum.Clockwise;
			_lightMeter.MaxValue = 500;
			_toggleTimer.WaitTime = 5;
		}
		else
		{
			_lightMeter.FillMode = (int)TextureProgressBar.FillModeEnum.CounterClockwise;
			_lightMeter.MaxValue = 1000;
			_toggleTimer.WaitTime = 10;
		}

		_toggleTimer.Start();
	}
}

