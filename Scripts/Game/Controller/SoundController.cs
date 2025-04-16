using System;
using System.IO;

namespace Goodot15.Scripts.Game.Controller;

using Godot;
using System.Collections.Generic;

public partial class SoundController : Node {
	private AudioStreamPlayer musicPlayer;
	private SettingsManager settingsManager;
	private string currentMusicPath;
	private float musicVolume;
	private bool isMusicMuted = false;
	
	private readonly Dictionary<string, AudioStream> sfx = new();
	private float sfxVolume;
	private bool isSfxMuted = false;

	public override void _Ready() {
		settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		musicVolume = settingsManager.MusicVolume;
		sfxVolume = settingsManager.SfxVolume;
		LoadMusicPlayer();
		LoadSounds();
	}
	
	// Music Setup

	private void LoadMusicPlayer() {
		musicPlayer = new AudioStreamPlayer();
		musicPlayer.Bus = "Music";
		AddChild(musicPlayer);
	}

	public void PlayMenuMusic() {
		PlayMusic("Assets/Music/xDeviruchi/02 - Title Theme.wav");
	}

	public void PlayGameMusic() {
		PlayMusic("Music/xDeviruchi/Definitely Our Town.wav");
	}

	public void PlayShopMusic() {
		PlayMusic("Music/xDeviruchi/Shop.wav");
	}

	private void PlayMusic(string musicPath) {
		if (currentMusicPath == musicPath && musicPlayer.Playing) {
			return;
		}
		
		AudioStream musicStream = GD.Load<AudioStream>("res://" + musicPath);
			
		
		// Enable looping based on file type. Either .wav or .ogg
		if (musicStream is AudioStreamOggVorbis oggStream) {
			oggStream.Loop = true;
		} else if (musicStream is AudioStreamWav wavStream) {
			wavStream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;

			wavStream.LoopEnd = wavStream.Data.Length;
		}
		
		currentMusicPath = musicPath;
		musicPlayer.Stream = musicStream;
		musicPlayer.VolumeDb = isMusicMuted ? -80 : Mathf.LinearToDb(musicVolume);
		musicPlayer.Play();
	}

	public void StopMusic() {
		musicPlayer?.Stop();
		currentMusicPath = "";
	}

	public void SetMusicVolume(float volume) {
		musicVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
		if (!isMusicMuted) {
			musicPlayer.VolumeDb = Mathf.LinearToDb(musicVolume);
		}
	}
	
	public float GetMusicVolume() => musicVolume;

	public void ToggleMusicMuted() {
		isMusicMuted = !isMusicMuted;
		musicPlayer.VolumeDb = isMusicMuted ? -80 : Mathf.LinearToDb(musicVolume);
	}
	
	// SFX Setup

	private void LoadSounds() {
		/*
		sfx["Combine"] = GD.Load<AudioStream>("res://Sounds/Combine.wav");
		sfx["Stack"] = GD.Load<AudioStream>("res://Sounds/Stack.wav");
		sfx["Pickup"] = GD.Load<AudioStream>("res://Sounds/Pickup.wav");
		sfx["Drop"] =  GD.Load<AudioStream>("res://Sounds/Drop.wav");
		sfx["Hover"] = GD.Load<AudioStream>("res://Sounds/Hover.wav");
		sfx["Click"] = GD.Load<AudioStream>("res://Sounds/Click.wav");
		*/
	}

	public void PlaySound(string soundName) {
		if (isSfxMuted || !sfx.ContainsKey(soundName)) {
			GD.PushWarning($"Sound '{soundName}' not found or muted.");
			return;
		}

		AudioStreamPlayer player = new AudioStreamPlayer();
		player.Stream = sfx[soundName];
		player.VolumeDb = Mathf.LinearToDb(sfxVolume);
		AddChild(player);
		// Queues the node to be deleted when player.Finished emits.
		player.Finished += () => player.QueueFree();
		player.Play();
	}
	
	public void SetSfxVolume(float volume) {
		sfxVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
	}
	
	public float GetSfxVolume() => sfxVolume;

	public void ToggleSfxMuted() {
		isSfxMuted = !isSfxMuted;
	}
	
	public override void _ExitTree() {
		StopMusic();
		QueueFree();
	}
}
