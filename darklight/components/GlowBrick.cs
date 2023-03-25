using Godot;
using System;

public partial class GlowBrick : CharacterBody3D
{
	[Signal] public delegate void GlowBrickDestroyedEventHandler(int brickID);

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	
	public int BrickID { get; private set; }
	public bool IsDestroyed {get; private set; } = false;

	private CollisionShape3D _collisionShape;
	private MeshInstance3D _brickMesh;

	public override void _Ready()
	{
		_collisionShape = GetNode<CollisionShape3D>("CollisionShape3D"); 
		_brickMesh = GetNode<MeshInstance3D>("BrickMesh");
	}

	public void Init(Vector3 position, int brickID)
	{
		Position = position;
		BrickID = brickID;
	}

	public override void _PhysicsProcess(double delta)
	{
		_brickMesh.Rotation += Vector3.Up * (float)delta;
		var collision = MoveAndCollide(Vector3.Zero);
		if (collision != null)
		{
			var collider = collision.GetCollider();
			if (collider is Player)
			{
				IsDestroyed = true;
				_collisionShape.Disabled = true;
				Visible = false;
				EmitSignal(nameof(GlowBrickDestroyed), BrickID);
				AudioManager.PlaySoundEffect(AudioManager.SoundEffects.GlowBrickPickup);
			}
		}
	}
}
