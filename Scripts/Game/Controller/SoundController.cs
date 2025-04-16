using System;
using System.IO;
using System.Linq;

namespace Goodot15.Scripts.Game.Controller;

using Godot;
using System.Collections.Generic;

public partial class SoundController : Node {

	private void DebugLog(string message) {
		GD.Print($"[{GetType().FullName}] {message}");
	}
	
	private AudioStreamPlayer musicPlayer;
	private SettingsManager SettingsManager => GetNode<SettingsManager>("/root/SettingsManager");
	private string currentPlayingMusicPath;
	
	private readonly IDictionary<string, AudioStream> _cachedSounds = new Dictionary<string, AudioStream>();
	private readonly IDictionary<string, AudioStream> _cachedMusic = new Dictionary<string, AudioStream>();


	public override void _Ready() {
		SetupMusicPlayer();
		LoadSounds();
		
		MusicVolume = SettingsManager.MusicVolume;
		SfxVolume = SettingsManager.SfxVolume;
	}
	
	#region Music-related

	private const string BASE_MUSIC_PATH = "res://Assets/Music/xDeviruchi";

	private void SetupMusicPlayer() {
		musicPlayer = new AudioStreamPlayer();
		musicPlayer.Bus = "Music";
		AddChild(musicPlayer);
	}

	public void PlayMenuMusic() {
		PlayMusic("02 - Title Theme.wav");
	}

	public void PlayGameMusic() {
		PlayMusic("Definitely Our Town.wav");
	}

	public void PlayShopMusic() {
		PlayMusic("Shop.wav");
	}

	private void PlayMusic(string musicPath) {
		if (currentPlayingMusicPath == musicPath) {
			musicPlayer.Play();
			return;
		}
		
		currentPlayingMusicPath = musicPath;
		musicPlayer.Stream = LoadMusic(musicPath);
		musicPlayer.VolumeDb = MusicMuted ? -80 : Mathf.LinearToDb(MusicVolume);
		musicPlayer.Play();
	}

	private AudioStream LoadMusic(string soundAssetName) {
		if (_cachedMusic.TryGetValue(soundAssetName, out AudioStream audioStream)) {
			// Return already loaded asset
			return audioStream;
		}
		else {
			// Music not loaded, first time setup
			DebugLog($"First time loading audio stream: {soundAssetName}");
			audioStream = GD.Load<AudioStream>($"{BASE_MUSIC_PATH}/{soundAssetName}");
			ConfigureLoopingSound(audioStream);
			_cachedMusic.Add(soundAssetName, audioStream);
		}
		
		return audioStream;
	}
	
	public void StopMusic() {
		musicPlayer?.Stop();
		currentPlayingMusicPath = "";
	}
	
	#endregion Music-related

	public static void ConfigureLoopingSound(AudioStream audioStreamAsset) {
		if (audioStreamAsset is AudioStreamOggVorbis oggStream) {
			oggStream.Loop = true;
		} else if (audioStreamAsset is AudioStreamWav wavStream) {
			wavStream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;

			wavStream.LoopEnd = wavStream.Data.Length;
		}
	}




	

	
	#region SFX-related

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
		if (SfxMuted || !_cachedSounds.TryGetValue(soundName, out AudioStream sfxAudioStream)) {
			GD.PushWarning($"Sound '{soundName}' not found or muted.");
			return;
		}

		AudioStreamPlayer player = new AudioStreamPlayer();
		player.Stream = sfxAudioStream;
		player.VolumeDb = Mathf.LinearToDb(SfxVolume);
		AddChild(player);
		// Queues the node to be deleted when player.Finished emits.
		player.Finished += () => player.QueueFree();
		player.Play();
	}
	
	#endregion SFX-related
	
	#region Settings & configurability related

	private float _musicVolume;
	public float MusicVolume {
		get => _musicVolume;
		set { _musicVolume = Mathf.Clamp(value, 0.0f, 1.0f); UpdateMusicVolume(); }

	}
	
	private void UpdateMusicVolume() {
		if (!MusicMuted) {
			musicPlayer.VolumeDb = Mathf.LinearToDb(MusicVolume);
		}
	}
	private bool _musicMuted = false;
	public bool MusicMuted {
		get => _musicMuted;
		set {
			_musicMuted = value;
			UpdateMusicMuted();
		}
	}
	public bool ToggleMusicMuted() {
		MusicMuted = !MusicMuted;
		UpdateMusicMuted();
		return MusicMuted;
	}

	private void UpdateMusicMuted() {
		musicPlayer.VolumeDb = MusicMuted ? -80 : Mathf.LinearToDb(MusicVolume);
	}
	
	private float _sfxVolume;
	public float SfxVolume {
		get => _sfxVolume;
		set => _sfxVolume = Mathf.Clamp(value, 0.0f, 1.0f);
	}
	private bool _isSfxMuted = false;
	public bool SfxMuted { get => _isSfxMuted; set => _isSfxMuted = value; }

	public bool ToggleSfxMuted() {
		return SfxMuted = !SfxMuted;
	}
	
	#endregion
	
	public override void _ExitTree() {
		StopMusic();
		
		UnloadSounds();
		
		QueueFree();
	}

	private void UnloadSounds() {
		DebugLog("Sound Controller is unloading sounds");
		DebugLog($"Sounds: {_cachedSounds.Count}; Music: {_cachedMusic.Count}");
		this._cachedSounds.ToList().ForEach(e=>e.Value.Dispose());
		this._cachedMusic.ToList().ForEach(e=>e.Value.Dispose());

		DebugLog("Sound Controller has unloaded sounds");
	}
}
