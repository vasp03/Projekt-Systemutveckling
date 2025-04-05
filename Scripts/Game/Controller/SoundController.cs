using System;
using System.IO;

namespace Goodot15.Scripts.Game.Controller;

using Godot;
using System.Collections.Generic;

public partial class SoundController : Node {
	private AudioStreamPlayer musicPlayer;
	private string currentMusicPath;
	private float musicVolume = 1.0f;
	private bool isMusicMuted = false;
	
	private readonly Dictionary<string, AudioStream> sfx = new();
	private float _volume = 1.0f;
	private bool isSfxMuted = false;

	public override void _Ready() {
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
		PlayMusic("Music/main_menu.ogg");
	}

	public void PlayGameMusic() {
		PlayMusic("Music/gameplay.ogg");
	}

	private void PlayMusic(string musicPath) {
		if (currentMusicPath == musicPath && musicPlayer.Playing) {
			musicPlayer.Stop();
		}
		
		AudioStream musicStream = GD.Load<AudioStream>("res://" + musicPath);
		
		// Ensures looping of .ogg files
		if (musicStream is AudioStreamOggVorbis oggStream) {
			oggStream.Loop = true;
		}
		
		musicPlayer.Stream = musicStream;
		musicPlayer.VolumeDb = isMusicMuted ? -80 : Mathf.LinearToDb(musicVolume);
		musicPlayer.Play();
		currentMusicPath = musicPath;
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

	public void ToggleMusicMuted() {
		isMusicMuted = !isMusicMuted;
		musicPlayer.VolumeDb = isMusicMuted ? -80 : Mathf.LinearToDb(musicVolume);
	}
	
	// SFX Setup

	private void LoadSounds() {
		sfx["Combine"] = GD.Load<AudioStream>("res://Sounds/Combine.wav");
		sfx["Stack"] = GD.Load<AudioStream>("res://Sounds/Stack.wav");
		sfx["Pickup"] = GD.Load<AudioStream>("res://Sounds/Pickup.wav");
		sfx["Drop"] =  GD.Load<AudioStream>("res://Sounds/Drop.wav");
		sfx["Hover"] = GD.Load<AudioStream>("res://Sounds/Hover.wav");
		sfx["Click"] = GD.Load<AudioStream>("res://Sounds/Click.wav");
	}

	public void PlaySound(string soundName) {
		if (isSfxMuted || !sfx.ContainsKey(soundName)) {
			GD.PushWarning($"Sound '{soundName}' not found or muted.");
			return;
		}

		AudioStreamPlayer player = new AudioStreamPlayer();
		player.Stream = sfx[soundName];
		player.VolumeDb = Mathf.LinearToDb(_volume);
		AddChild(player);
		//Queues the node to be deleted when player.Finished emits.
		player.Finished += () => player.QueueFree();
		player.Play();
	}
	
	public void SetVolume(float volume) {
		_volume = Mathf.Clamp(volume, 0.0f, 1.0f);
	}

	public void ToggleSfxMuted() {
		isSfxMuted = !isSfxMuted;
	}
}
