using Godot;
using System;

public partial class GlowBall : CharacterBody3D
{
	[Export] public float BallSpeed = 10f;

	public bool IsDestroyed { get; private set; } = false;

	private Node3D _glowBallMeshA;
	[Export] private StandardMaterial3D[] _materialsList;
	private StandardMaterial3D _originalMaterial;
	private OmniLight3D _omniLight3D;

	private Node3D _meshesNode;

	[Export] private Vector3 _ballDirection = Vector3.Zero;
	private CpuParticles3D _collisionParticles;
	private CpuParticles3D _collisionParticlesR;
	private CpuParticles3D _collisionParticlesG;
	private CpuParticles3D _collisionParticlesB;
	private CpuParticles3D[] _particlesList;

	public override void _Ready()
	{
		_meshesNode = GetNode<Node3D>("Meshes");
		_glowBallMeshA = _meshesNode.GetNode<Node3D>("GlowBallMeshA");
		_omniLight3D = GetNode<OmniLight3D>("OmniLight3D");
		_collisionParticles = GetNode<CpuParticles3D>("CollisionParticles");
		_collisionParticlesR = GetNode<CpuParticles3D>("CollisionParticlesR");
		_collisionParticlesG = GetNode<CpuParticles3D>("CollisionParticlesG");
		_collisionParticlesB = GetNode<CpuParticles3D>("CollisionParticlesB");

		_particlesList = new CpuParticles3D[]
		{
			_collisionParticles, _collisionParticlesR, _collisionParticlesG, _collisionParticlesB, 
		};

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
			for (var i = 0; i < _particlesList.Length; i++)
			{
				_particlesList[i].Direction = collision.GetNormal();
				_particlesList[i].Restart();
			}
			BallSpeed += 0.5f;
			AudioManager.PlaySoundEffect(AudioManager.SoundEffects.GlowBallBounce);

			var collider = collision.GetCollider();
			if (collider is HiddenWall)
			{
				((HiddenWall)collider).ShowWall(true);
			}
		}
	}

	public void SetDirection(Vector3 direction)
	{
		_ballDirection = direction.Normalized();
	}

	public void DestroyGlowBall()
	{
		IsDestroyed = true;
		_meshesNode.Visible = false;
		_omniLight3D.Visible = false;
		for (var i = 0; i < _particlesList.Length; i++)
		{
			_particlesList[i].Spread = 180f;
			_particlesList[i].Restart();
		}
	}
}
