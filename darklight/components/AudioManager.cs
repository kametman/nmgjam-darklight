using Godot;
using System.Collections.Generic;

public partial class AudioManager : Node3D
{
	public enum SoundEffects
	{
		PlayerDeath,
		Countdown,
		GameStart,
		FireGlowBall,
		GlowBallBounce,
		GlowBrickPickup,
		WallBump,
	}
	public enum MusicLoops
	{
		MainGame,
		StageClear,
	}

	private static Dictionary<SoundEffects, AudioStreamPlayer3D> _sfxLibrary;
	private static Dictionary<MusicLoops, AudioStreamPlayer3D> _musicLibrary;

	public override void _Ready()
	{
		_sfxLibrary = new Dictionary<SoundEffects, AudioStreamPlayer3D>();
		_sfxLibrary.Add(SoundEffects.PlayerDeath, GetNode<AudioStreamPlayer3D>("PlayerDeathSfx"));
		_sfxLibrary.Add(SoundEffects.Countdown, GetNode<AudioStreamPlayer3D>("CountdownSfx"));
		_sfxLibrary.Add(SoundEffects.GameStart, GetNode<AudioStreamPlayer3D>("GameStartSfx"));
		_sfxLibrary.Add(SoundEffects.FireGlowBall, GetNode<AudioStreamPlayer3D>("FireGlowBallSfx"));
		_sfxLibrary.Add(SoundEffects.GlowBallBounce, GetNode<AudioStreamPlayer3D>("GlowBallBounceSfx"));
		_sfxLibrary.Add(SoundEffects.GlowBrickPickup, GetNode<AudioStreamPlayer3D>("GlowBrickPickupSfx"));
		_sfxLibrary.Add(SoundEffects.WallBump, GetNode<AudioStreamPlayer3D>("WallBumpSfx"));

		_musicLibrary = new Dictionary<MusicLoops, AudioStreamPlayer3D>();
		_musicLibrary.Add(MusicLoops.MainGame, GetNode<AudioStreamPlayer3D>("MainGameMusic"));
		_musicLibrary.Add(MusicLoops.StageClear, GetNode<AudioStreamPlayer3D>("StageClearMusic"));
	}

	public override void _Process(double delta)
	{
	}

	public static void PlaySoundEffect(SoundEffects sfxEnum)
	{
		var sfx = _sfxLibrary[sfxEnum];
		if (sfx != null) { sfx.Play(); }
	}

	public static void PlayMusicLoop(MusicLoops musicEnum)
	{
		var musicLoop = _musicLibrary[musicEnum];
		if (musicLoop != null) { musicLoop.Play(); }
	}

	public static void StopAllMusicLoops()
	{
		foreach (var musicLoop in _musicLibrary)
		{
			musicLoop.Value.Stop();
		}
	}

	public static void StopAllSoundEffects()
	{
		foreach (var sfx in _sfxLibrary)
		{
			sfx.Value.Stop();
		}
	}
}
