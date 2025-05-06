using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class SoundController : Node {
    private readonly IDictionary<string, AudioStream> _cachedMusic = new Dictionary<string, AudioStream>();

    private readonly IDictionary<string, AudioStream> _cachedSounds = new Dictionary<string, AudioStream>();
    private string currentPlayingMusicPath;

    private AudioStreamPlayer musicPlayer;
    private SettingsManager SettingsManager => GetNode<SettingsManager>("/root/SettingsManager");

    private void DebugLog(string message) {
        // GD.Print($"[{GetType().FullName}] {message}");
    }


    public override void _Ready() {
        SetupMusicPlayer();

        MusicVolume = SettingsManager.MusicVolume;
        SfxVolume = SettingsManager.SfxVolume;
        
    
        musicPlayer.Finished += OnMusicFinished;
    }

    public void OnMusicFinished() {
        if (ShouldCurrentSongLoop()) {
            GD.Print("Looping music" + currentPlayingMusicPath);
            musicPlayer.Play();
        }
    }
    
    /*
     * Helper method for OnMusicFinished class
     * Checks to see if a song should replay after .finished has emitted
     * Necessary cause MP3 songs don't have built in looping through Godot
     */
    public bool ShouldCurrentSongLoop() {
        if (string.IsNullOrEmpty(currentPlayingMusicPath)) {
            return false;
        }

        return currentPlayingMusicPath.Contains("DayTimeSongs/Day");
    }

    public static void ConfigureLoopingSound(AudioStream audioStreamAsset) {
        if (audioStreamAsset is AudioStreamOggVorbis oggStream) {
            oggStream.Loop = true;
        } else if (audioStreamAsset is AudioStreamWav wavStream) {
            wavStream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;

            wavStream.LoopEnd = wavStream.Data.Length;
        }
    }

    public override void _ExitTree() {
        StopMusic();

        UnloadSounds();

        QueueFree();
    }

    private void UnloadSounds() {
        DebugLog("Sound Controller is unloading sounds");
        DebugLog($"Sounds: {_cachedSounds.Count}; Music: {_cachedMusic.Count}");
        _cachedSounds.ToList().ForEach(e => e.Value.Dispose());
        _cachedMusic.ToList().ForEach(e => e.Value.Dispose());

        DebugLog("Sound Controller has unloaded sounds");
    }

    #region Music-related

    private const string BASE_MUSIC_PATH = "res://Assets/Music";

    private void SetupMusicPlayer() {
        musicPlayer = new AudioStreamPlayer();
        musicPlayer.Bus = "Music";
        AddChild(musicPlayer);
    }

    public void PlayMenuMusic() {
        PlayMusic("xDeviruchi/02 - Title Theme.wav");
    }

    public void PlayGameMusic() {
        PlayMusic("xDeviruchi/03 - Definitely Our Town.wav");
    }

    public void PlayShopMusic() {
        PlayMusic("xDeviruchi/08 - Shop.wav");
    }

    public void PlayDayTimeSong(string dayTime) {
        string musicPath = $"DayTimeSongs/{dayTime}.mp3";

        if (currentPlayingMusicPath == musicPath) {
            musicPlayer.Play();
            return;
        }

        PlayMusic(musicPath);
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
        if (_cachedMusic.TryGetValue(soundAssetName, out AudioStream audioStream))
            // Return already loaded asset
            return audioStream;

        // Music not loaded, first time setup
        DebugLog($"First time loading audio stream: {soundAssetName}");
        audioStream = GD.Load<AudioStream>($"{BASE_MUSIC_PATH}/{soundAssetName}");
        ConfigureLoopingSound(audioStream);
        _cachedMusic.Add(soundAssetName, audioStream);

        return audioStream;
    }

    public void StopMusic() {
        musicPlayer?.Stop();
        currentPlayingMusicPath = "";
    }

    #endregion Music-related


    #region SFX-related

    public void PlaySound(string soundName) {
        if (SfxMuted || !_cachedSounds.TryGetValue(soundName, out AudioStream sfxAudioStream)) {
            GD.PushWarning($"Sound '{soundName}' not found or muted.");
            return;
        }

        AudioStreamPlayer player = new();
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
        set {
            _musicVolume = Mathf.Clamp(value, 0.0f, 1.0f);
            UpdateMusicVolume();
        }
    }

    private void UpdateMusicVolume() {
        if (!MusicMuted) musicPlayer.VolumeDb = Mathf.LinearToDb(MusicVolume);
    }

    private bool _musicMuted;

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

    public bool SfxMuted { get; set; }

    public bool ToggleSfxMuted() {
        return SfxMuted = !SfxMuted;
    }

    #endregion
}