using Godot;
using System;

public partial class GameData : Node
{
	public static bool PlayerInputDisabled { get; set; }
	public static int CurrentLevel { get; set; } = 1;
	public static int CurrentScore { get; set; } = 0;
	public static bool InitialLoad { get; set; } = true;

	public static void Reset()
	{
		CurrentLevel = 1;
		CurrentScore = 0;
	}
}
