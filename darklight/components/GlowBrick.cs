using Godot;
using System;

public partial class GlowBrick : CharacterBody3D
{
	[Signal] public delegate void GlowBallSpawnedEventHandler(Vector3 position);

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{

	}

	public void OnGlowBallTimerTimeout()
	{
		EmitSignal(nameof(GlowBallSpawned), GlobalPosition);
	}
}
