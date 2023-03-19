using Godot;
using System;

public partial class GlowBall : CharacterBody3D
{
	[Export] public float BallSpeed = 10f;

	public bool IsDestroyed { get; private set; } = false;

	private Node3D _glowBallMeshA;
	[Export] private StandardMaterial3D[] _materialsList;
	private StandardMaterial3D _originalMaterial;

	private Node3D _meshesNode;

	[Export] private Vector3 _ballDirection = Vector3.Zero;
	private CpuParticles3D _collisionParticles;

	public override void _Ready()
	{
		_meshesNode = GetNode<Node3D>("Meshes");
		_glowBallMeshA = _meshesNode.GetNode<Node3D>("GlowBallMeshA");
		_collisionParticles = GetNode<CpuParticles3D>("CollisionParticles");

		_originalMaterial = _materialsList[0];
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsDestroyed) { return; }
		_meshesNode.Rotation += _ballDirection  * 5f * (float)delta;

		var moveVelocity = _ballDirection * BallSpeed * (float)delta;
		
		var collision = MoveAndCollide(moveVelocity);
		if (collision != null)
		{
			_ballDirection = _ballDirection.Bounce(collision.GetNormal());
			_collisionParticles.Direction = collision.GetNormal();
			_collisionParticles.Restart();
			BallSpeed += 1f;
		}
	}

	public void SetDirection(Vector3 direction)
	{
		_ballDirection = direction.Normalized();
	}

	public void DestroyGlowBall()
	{
		IsDestroyed = true;
	}
}
