using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MainGame : Node3D
{
	[Export] public int CountdownTimer = 3;
	[Export] public int RoomWidth = 48;
	[Export] public int RoomLength = 48;
	[Export] public int DeadZoneSize = 2;

	private PackedScene _glowBallPrefab = null;
	private PackedScene _glowBrickPrefab = null;
	private PackedScene _hiddenWallPrefab = null;

	private int _startingGlowBricks = 3;
	private int _startingHiddenBlocks;
	private int _glowBrickDestoyed;
	private int _currentCountdown;
	private bool _playerDestoyed = false;
	private string _stageClearText = "";
	private string _gameOverText = "GAME OVER";
	private List<int> _bricksXRange, _bricksZRange, _wallsXRange, _wallsZRange;

	private List<GlowBrick> _glowBricksList;
	private List<GlowBall> _glowBallsList;
	private List<HiddenWall> _hiddenWallsList;
	
	private Player _player;

	private Timer _gameStartTimer;
	private Timer _glowBallSpawnTimer;
	private Timer _levelEndTimer;

	private Label _countdownTimerLabel;
	private Label _glowBrickCountLabel;
	private Label _currentScoreLabel;
	private TextureProgressBar _loadingProgresBar;
	private Label _stageClearLabel;
	private Label _gameOverLabel;

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
		_stageClearLabel = GetNode<Label>("UI/StageClearLabel");
		_gameOverLabel = GetNode<Label>("UI/GameOverLabel");

		_countdownTimerLabel.Visible = true;
		_stageClearLabel.Visible = false;
		_gameOverLabel.Visible = false;

		_currentCountdown = CountdownTimer;
		_startingGlowBricks = GameData.CurrentLevel;
		_startingHiddenBlocks = _startingGlowBricks * 2;

		InitializeObjectPositionRanges();
		GD.Randomize();
		InitializePlayField();

		AudioManager.PlaySoundEffect(AudioManager.SoundEffects.Countdown);
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
			AudioManager.PlaySoundEffect(AudioManager.SoundEffects.Countdown);
			_gameStartTimer.Start();
		}
		else{
			AudioManager.PlaySoundEffect(AudioManager.SoundEffects.GameStart);
			GameData.PlayerInputDisabled = false;
			_countdownTimerLabel.Visible = false;
			_glowBallSpawnTimer.Start();
			AudioManager.PlayMusicLoop(AudioManager.MusicLoops.MainGame);
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
		if (_playerDestoyed) 
		{
			AudioManager.StopAllSoundEffects();
			GetTree().ChangeSceneToFile("uid://45lv0wd6ottn");
		}
		else 
		{ 
			GameData.CurrentLevel++; 
			GetTree().ReloadCurrentScene();
		}

	}

	private void InitializeObjectPositionRanges()
	{
		_bricksXRange = new List<int>();
		_wallsXRange = new List<int>();
		_bricksZRange = new List<int>();
		_wallsZRange = new List<int>();

		for (var i = (-RoomWidth / 2) + DeadZoneSize; i < (RoomWidth / 2) - DeadZoneSize; i++)
		{
			if (i >= -DeadZoneSize && i <= DeadZoneSize)
			{
				continue;
			}
			else if (i % 2 == 0)
			{
				_bricksXRange.Add(i);
			}
			else 
			{
				_wallsXRange.Add(i);
			}
		}

		for (var i = (-RoomLength / 2) + DeadZoneSize; i < (RoomLength / 2) - DeadZoneSize; i++)
		{
			if (i >= -DeadZoneSize && i <= DeadZoneSize)
			{
				continue;
			}
			else if (i % 2 == 0)
			{
				_bricksZRange.Add(i);
			}
			else 
			{
				_wallsZRange.Add(i);
			}
		}
	}

	private void InitializePlayField()
	{
		_glowBricksList = new List<GlowBrick>();
		_glowBallsList = new List<GlowBall>();
		_hiddenWallsList = new List<HiddenWall>();

		SpawnGlowBricks();
		SpawnHiddenWalls();

		_player.PlayerDestoyed += DestroyPlayer;

		_glowBrickDestoyed = 0;
	}

	private void SpawnGlowBricks()
	{
		for (var i = 0; i < _startingGlowBricks; i++)
		{
			var brickXPos = _bricksXRange[(int)(GD.Randi() % _bricksXRange.Count)];
			var brickZPos = _bricksXRange[(int)(GD.Randi() % _bricksZRange.Count)];

			var spawnPosition = new Vector3(brickXPos, 0f, brickZPos);
			spawnPosition += new Vector3(1f, 0f, 1f);
			var newGlowBrick = _glowBrickPrefab.Instantiate<GlowBrick>();
			newGlowBrick.Init(spawnPosition, i);
			newGlowBrick.GlowBrickDestroyed += DestroyGlowBrick;
			AddChild(newGlowBrick);

			_glowBricksList.Add(newGlowBrick);
		}
	}

	private void SpawnHiddenWalls()
	{
		for(var i = 0; i < _startingHiddenBlocks; i++)
		{
			var wallXPos = _wallsXRange[(int)(GD.Randi() % _wallsXRange.Count)];
			var wallZPos = _wallsXRange[(int)(GD.Randi() % _wallsZRange.Count)];

			var spawnPosition = new Vector3(wallXPos, 0f, wallZPos);
			if (spawnPosition == Vector3.Zero) { continue; }

			var newWall = _hiddenWallPrefab.Instantiate<HiddenWall>();
			newWall.Init(spawnPosition);
			AddChild(newWall);

			_hiddenWallsList.Add(newWall);
		}
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

			AudioManager.PlaySoundEffect(AudioManager.SoundEffects.FireGlowBall);
		}
	}

	private void DestroyGlowBrick(int brickID)
	{
		_glowBrickDestoyed++;
		GameData.CurrentScore += 50;

		if (_glowBrickDestoyed == _glowBricksList.Count)
		{
			_stageClearText = "!!! STAGE CLEAR !!!";

			GameData.CurrentScore += 1000;
			_stageClearText += "\n+1000";

			if (_glowBallsList.Count > GameData.CurrentLevel)
			{
				var speedPenalty = 10 * (_glowBallsList.Count - GameData.CurrentLevel);
				GameData.CurrentScore -= speedPenalty;
				_stageClearText += $"\nHazard Penalty -{speedPenalty}";
			}
			else 
			{ 
				GameData.CurrentScore += 500; 
				_stageClearText += "\nSpeed Bonus +500";
			}

			GameData.PlayerInputDisabled = true;

			for (var i = 0; i < _glowBallsList.Count; i++)
			{
				_glowBallsList[i].DestroyGlowBall();
			}

			_loadingProgresBar.Visible = true;
			_loadingProgresBar.Value = 0;

			_stageClearLabel.Text = _stageClearText;
			_stageClearLabel.Visible = true;
			_levelEndTimer.Start();
			AudioManager.StopAllMusicLoops();
			AudioManager.PlayMusicLoop(AudioManager.MusicLoops.StageClear);
		}
	}

	private void DestroyPlayer()
	{
		_playerDestoyed = true;
		_levelEndTimer.WaitTime = 5f;
		_levelEndTimer.Start();
		_gameOverText = $"GAME OVER\nTOTAL SCORE: {GameData.CurrentScore}";
		_gameOverLabel.Text = _gameOverText;
		_gameOverLabel.Visible = true;
		AudioManager.StopAllMusicLoops();
	}
}
