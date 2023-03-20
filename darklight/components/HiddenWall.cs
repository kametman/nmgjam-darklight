using Godot;
using System;

public partial class HiddenWall : StaticBody3D
{
	public bool IsHidden { get; private set; }
	[Export] private Material _normalWallMaterial;
	[Export] private Material[] _visibleWallMaterials;

	private MeshInstance3D _wallMesh;
	private Timer _visibilityTimer;

	public override void _Ready()
	{
		_wallMesh = GetNode<MeshInstance3D>("WallMesh");
		_visibilityTimer = GetNode<Timer>("VisibilityTimer");

		HideWall();
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

	public void ShowWall()
	{
		IsHidden = false;
		_visibilityTimer.Start();
	}

	public void HideWall()
	{
		_wallMesh.MaterialOverride = _normalWallMaterial;
		IsHidden = true;
	}
}
