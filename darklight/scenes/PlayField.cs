using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayField : Node3D
{
	[Export] public int CountdownTimer = 3;
	private PackedScene _glowBallPrefab = null;
	private PackedScene _glowBrickPrefab = null;
	private PackedScene _hiddenWallPrefab = null;

	private int _startingGlowBricks = 3;
	private int _glowBrickDestoyed;
	private int _currentCountdown;
	private bool _playerDestoyed = false;

	private List<GlowBrick> _glowBricksList;
	private List<GlowBall> _glowBallsList;
	
	private Player _player;

	private Timer _gameStartTimer;
	private Timer _glowBallSpawnTimer;
	private Timer _levelEndTimer;

	private Label _countdownTimerLabel;
	private Label _glowBrickCountLabel;
	private Label _currentScoreLabel;
	private TextureProgressBar _loadingProgresBar;

	public override void _Ready()
	{
		GameData.PlayerInputDisabled = true;
		_glowBallPrefab = ResourceLoader.Load<PackedScene>("uid://dc4hr27nqx0w1");
		_glowBrickPrefab = ResourceLoader.Load<PackedScene>("uid://b5gff6pkt8hff");
		_hiddenWallPrefab = ResourceLoader.Load<PackedScene>("uid://b4oi0h36ko0wx");

		_player = GetNode<Player>("Player");

		_gameStartTimer = GetNode<Timer>("GameStartTimer");
		_glowBallSpawnTimer = GetNode<Timer>("GlowBallSpawnTimer");
		_levelEndTimer = GetNode<Timer>("LevelEndTimer");

		_countdownTimerLabel = GetNode<Label>("UI/CountdownTimerLabel");
		_glowBrickCountLabel = GetNode<Label>("UI/GlowBrickCountLabel");
		_currentScoreLabel = GetNode<Label>("UI/CurrentScoreLabel");
		_loadingProgresBar = GetNode<TextureProgressBar>("UI/LoadingProgressBar");

		_currentCountdown = CountdownTimer;
		_startingGlowBricks = GameData.CurrentLevel;

		GD.Randomize();
		InitializePlayField();

		_gameStartTimer.Start();
	}

	public override void _Process(double delta)
	{
		if (_countdownTimerLabel.Visible)
		{
			_countdownTimerLabel.Text = $"{_currentCountdown}";
		}
		_glowBrickCountLabel.Text = $"{_glowBrickDestoyed} / {_glowBricksList.Count}";
		_currentScoreLabel.Text = $"SCORE: {GameData.CurrentScore}";

		if (_loadingProgresBar.Visible)
		{
			_loadingProgresBar.Value = (_levelEndTimer.TimeLeft / _levelEndTimer.WaitTime) * 100;
		}
	}

	public void OnGameStartTimerTimeout()
	{
		_currentCountdown--;
		if (_currentCountdown > 0)
		{
			_gameStartTimer.Start();
		}
		else{
			GameData.PlayerInputDisabled = false;
			_countdownTimerLabel.Visible = false;
			_glowBallSpawnTimer.Start();
		}
	}

	public void OnGlowBallSpawnTimerTimeout()
	{
		for (var i = 0; i < _glowBricksList.Count; i++)
		{
			if (_glowBricksList[i].IsDestroyed) { continue; }
			SpawnGlowBall(_glowBricksList[i].BrickID);
		}
		_glowBallSpawnTimer.Start();
	}

	public void OnLevelEndTimerTimeout()
	{
		if (_playerDestoyed) { GameData.Reset(); }
		else { GameData.CurrentLevel++; }

		GetTree().ReloadCurrentScene();
	}

	private void InitializePlayField()
	{
		_glowBricksList = new List<GlowBrick>();
		_glowBallsList = new List<GlowBall>();
		for (var i = 0; i < _startingGlowBricks; i++)
		{
			var spawnPosition = new Vector3((int)GD.RandRange(-14f, 14f), 0f, (int)GD.RandRange(-14f, 14f));
			var newGlowBrick = _glowBrickPrefab.Instantiate<GlowBrick>();
			newGlowBrick.Init(spawnPosition, i);
			newGlowBrick.GlowBrickDestroyed += DestroyGlowBrick;
			AddChild(newGlowBrick);

			_glowBricksList.Add(newGlowBrick);
		}

		_player.PlayerDestoyed += DestroyPlayer;

		_glowBrickDestoyed = 0;
	}

	private void SpawnGlowBall(int brickID)
	{
		var glowBrick = _glowBricksList.SingleOrDefault(x => x.BrickID == brickID);

		if (glowBrick != null)
		{
			var direction = new Vector3(GD.Randf(), 0, GD.Randf()).Normalized();
			var newGlowBall = _glowBallPrefab.Instantiate<GlowBall>();
			newGlowBall.Position = glowBrick.Position + direction * 1.25F;
			AddChild(newGlowBall);
			newGlowBall.SetDirection(direction);
			_glowBallsList.Add(newGlowBall);
		}
	}

	private void DestroyGlowBrick(int brickID)
	{
		_glowBrickDestoyed++;
		GameData.CurrentScore += 50;

		if (_glowBrickDestoyed == _glowBricksList.Count)
		{
			GameData.CurrentScore += 1000;
			if (_glowBallsList.Count > GameData.CurrentLevel)
			{
				GameData.CurrentScore -= _glowBallsList.Count - GameData.CurrentLevel;
			}
			else
			{
				GameData.CurrentScore += 500;
			}

			GameData.PlayerInputDisabled = true;

			for (var i = 0; i < _glowBallsList.Count; i++)
			{
				_glowBallsList[i].DestroyGlowBall();
			}

			_loadingProgresBar.Visible = true;
			_loadingProgresBar.Value = 0;

			_levelEndTimer.Start();
		}
	}

	private void DestroyPlayer()
	{
		_playerDestoyed = true;
		_levelEndTimer.Start();
	}
}
