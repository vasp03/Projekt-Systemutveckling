using System.IO;

namespace Goodot15.Scripts.Game.Controller;

using Godot;
using System.Collections.Generic;

public partial class SoundController : Node {
	private readonly Dictionary<string, AudioStream> audioStreams = new();
	private float _volume = 1.0f;

	public override void _Ready() {
		LoadSounds();
	}

	private void LoadSounds() {
		audioStreams["Combine"] = GD.Load<AudioStream>("res://Sounds/Combine.wav");
		audioStreams["Stack"] = GD.Load<AudioStream>("res://Sounds/Stack.wav");
		audioStreams["Pickup"] = GD.Load<AudioStream>("res://Sounds/Pickup.wav");
		audioStreams["Drop"] =  GD.Load<AudioStream>("res://Sounds/Drop.wav");
	}

	public void PlaySound(string soundName) {
		if (!audioStreams.ContainsKey(soundName)) {
			GD.PushWarning($"Sound '{soundName}' not found.");
			return;
		}

		AudioStreamPlayer player = new AudioStreamPlayer();
		player.Stream = audioStreams[soundName];
		player.VolumeDb = Mathf.LinearToDb(_volume);
		AddChild(player);

		player.Finished += () => player.QueueFree();
	}

	public void SetVolume(float volume) {
		_volume = Mathf.Clamp(volume, 0.0f, 1.0f);
	}
}