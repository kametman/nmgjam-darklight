using Godot;
using System;

public partial class HiddenWall : StaticBody3D
{
	public bool IsHidden { get; private set; }
	[Export] private Material _normalWallMaterial;
	[Export] private Material[] _visibleWallMaterials;

	private MeshInstance3D _wallMesh;
	private Timer _visibilityTimer;
	private CollisionShape3D _collision;

	[Export] private int _maxDurability = 10;
	private int _durability;

	public override void _Ready()
	{
		_wallMesh = GetNode<MeshInstance3D>("WallMesh");
		_visibilityTimer = GetNode<Timer>("VisibilityTimer");
		_collision = GetNode<CollisionShape3D>("CollisionShape3D");

		_durability = _maxDurability;

		HideWall();
	}

	public void Init(Vector3 spawnPosition)
	{
		Position = spawnPosition;
		if (GD.Randf() > 0.5f)
		{
			RotateY(Mathf.Pi * 0.5f);
		}
	}

	public override void _Process(double delta)
	{
		if (!IsHidden)
		{
			_wallMesh.MaterialOverride = _visibleWallMaterials[GD.Randi() % _visibleWallMaterials.Length];
		}
	}

	public void OnVisibilityTimerTimeout()
	{
		HideWall();
	}

	public void ShowWall(bool causeDamage = false)
	{
		if (causeDamage) { _durability--; }
		if (_durability <= 0)
		{
			_collision.Disabled = true;
			_wallMesh.Visible = false;
		}
		else
		{
			IsHidden = false;
			AudioManager.PlaySoundEffect(AudioManager.SoundEffects.WallBump);
			_wallMesh.Visible = true;
			_visibilityTimer.Start();
		}
	}

	public void HideWall()
	{
		_wallMesh.MaterialOverride = _normalWallMaterial;
		_wallMesh.Visible = false;
		IsHidden = true;
	}
}
