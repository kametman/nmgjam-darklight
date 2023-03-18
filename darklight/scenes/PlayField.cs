using Godot;
using System;

public partial class PlayField : Node3D
{
	private PackedScene _glowBallPrefab = null;
	private PackedScene _glowBrickPrefab = null;

	private int _startingGlowBricks = 3;

	public override void _Ready()
	{
		_glowBallPrefab = ResourceLoader.Load<PackedScene>("uid://dc4hr27nqx0w1");
		_glowBrickPrefab = ResourceLoader.Load<PackedScene>("uid://b5gff6pkt8hff");

		GD.Randomize();
		InitializePlayField();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void InitializePlayField()
	{
		for (var i = 0; i < _startingGlowBricks; i++)
		{
			var spawnPosition = new Vector3((int)GD.RandRange(-15f, 15f), 0f, (int)GD.RandRange(-15f, 15f));
			var newGlowBrick = _glowBrickPrefab.Instantiate<GlowBrick>();
			newGlowBrick.Position = spawnPosition;
			newGlowBrick.GlowBallSpawned += SpawnGlowBall;
			AddChild(newGlowBrick);
		}
	}

	private void SpawnGlowBall(Vector3 position)
	{
		var direction = new Vector3(GD.Randf(), 0, GD.Randf()).Normalized();
		var newGlowBall = _glowBallPrefab.Instantiate<GlowBall>();
		newGlowBall.Position = position + direction * 1.25F;
		AddChild(newGlowBall);
		newGlowBall.SetDirection(direction);
	}
}
