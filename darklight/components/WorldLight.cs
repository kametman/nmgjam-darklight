using Godot;

public partial class WorldLight : Node3D
{
	private const float ROTATION_ANGLE = Mathf.Pi;

	public bool IsLightBright { get { return _bright == 0; } }

	private int _bright = 0;
	
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Rotation = new Vector3(ROTATION_ANGLE * _bright, 0f, 0f);
	}
	
	public void ToggleLight()
	{
		_bright = (++_bright) % 2;
	}

	public void TurnLightOn()
	{
		_bright = 1;
	}

	public void TurnLightOff()
	{
		_bright = 0;
	}
}

