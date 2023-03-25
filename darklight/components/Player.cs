using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Signal] public delegate void PlayerDestoyedEventHandler();
	public const float Speed = 10.0f;
	public const float JumpVelocity = 4.5f;

	private Node3D _playerModel;
	private Vector3 _facingDirection;
	private CollisionShape3D _collisionShape;

	private CpuParticles3D _deathParticlesRed;
	private CpuParticles3D _deathParticlesGreen;
	private CpuParticles3D _deathParticlesBlue;

	public override void _Ready()
	{
		_playerModel = GetNode<Node3D>("PlayerModel");
		_collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");

		_deathParticlesRed = GetNode<CpuParticles3D>("DeathParticlesRed");
		_deathParticlesGreen = GetNode<CpuParticles3D>("DeathParticlesGreen");
		_deathParticlesBlue = GetNode<CpuParticles3D>("DeathParticlesBlue");

		_facingDirection = Vector3.Up;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GameData.PlayerInputDisabled) { return; }
		Vector3 velocity = Velocity;

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;

			_facingDirection = direction.Normalized();
			_playerModel.LookAt(Position + _facingDirection);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		var collisionsCount = GetSlideCollisionCount();
		for (var i = 0; i < collisionsCount; i++)
		{
			var collision = GetSlideCollision(i);
			if (collision != null)
			{
				var collider = collision.GetCollider();
				if (collider is GlowBall)
				{
					_collisionShape.Disabled = true;
					_playerModel.Visible = false;
					_deathParticlesRed.Restart();
					_deathParticlesGreen.Restart();
					_deathParticlesBlue.Restart();
					AudioManager.PlaySoundEffect(AudioManager.SoundEffects.PlayerDeath);

					EmitSignal(nameof(PlayerDestoyed));
				}
				else if (collider is HiddenWall)
				{
					((HiddenWall)collider).ShowWall();
				}
			}
		}

	}
}
