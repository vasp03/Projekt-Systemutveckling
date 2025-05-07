using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Goodot15.Scripts.Game.Controller;

public partial class SoundController : Node {
    private const string BASE_MUSIC_PATH = "res://Assets/Music";
    private readonly IDictionary<string, AudioStream> CachedMusic = new Dictionary<string, AudioStream>();
    private readonly IDictionary<string, AudioStream> CachedSounds = new Dictionary<string, AudioStream>();
    private bool _musicMuted;
    private float _musicVolume;
    private float _sfxVolume;
    private string CurrentPlayingMusicPath;
    private AudioStreamPlayer MusicPlayer;
    private SettingsManager SettingsManager => GetNode<SettingsManager>("/root/SettingsManager");
    public bool SfxMuted { get; set; }

    public bool MusicMuted {
        get => _musicMuted;
        set {
            _musicMuted = value;
            UpdateMusicMuted();
        }
    }

    public float SfxVolume {
        get => _sfxVolume;
        set => _sfxVolume = Mathf.Clamp(value, 0.0f, 1.0f);
    }

    public override void _Ready() {
        SetupMusicPlayer();

        MusicVolume = SettingsManager.MusicVolume;
        SfxVolume = SettingsManager.SfxVolume;


        // MusicPlayer.Finished += OnMusicFinished;
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
        CachedSounds.ToList().ForEach(e => e.Value.Dispose());
        CachedMusic.ToList().ForEach(e => e.Value.Dispose());
    }

    #region Music-related

    private void SetupMusicPlayer() {
        MusicPlayer = new AudioStreamPlayer();
        MusicPlayer.Bus = "Music";
        AddChild(MusicPlayer);
        // MusicPlayer.Finished += OnMusicFinished;
    }

    /*
     * Helper method for OnMusicFinished class
     * Checks to see if a song should replay after .finished has emitted
     * Necessary cause MP3 songs don't have built in looping through Godot
     */
    public bool ShouldCurrentSongLoop() {
        if (string.IsNullOrEmpty(CurrentPlayingMusicPath)) {
            return false;
        }

        return CurrentPlayingMusicPath.Contains("DayTimeSongs/Day");
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

        if (CurrentPlayingMusicPath == musicPath) {
            MusicPlayer.Play();
            return;
        }

        PlayMusic(musicPath);
    }

    private void PlayMusic(string musicPath) {
        if (CurrentPlayingMusicPath == musicPath) {
            MusicPlayer.Play();
            return;
        }

        CurrentPlayingMusicPath = musicPath;
        MusicPlayer.Stream = LoadMusic(musicPath);
        MusicPlayer.VolumeDb = MusicMuted
            ? -80
            : Mathf.LinearToDb(MusicVolume);
        MusicPlayer.Play();
    }

    private AudioStream LoadMusic(string soundAssetName) {
        if (CachedMusic.TryGetValue(soundAssetName, out AudioStream audioStream)) {
            return audioStream;
        }

        // Music not loaded, first time setup
        audioStream = GD.Load<AudioStream>($"{BASE_MUSIC_PATH}/{soundAssetName}");
        ConfigureLoopingSound(audioStream);
        CachedMusic.Add(soundAssetName, audioStream);

        return audioStream;
    }

    public void StopMusic() {
        MusicPlayer?.Stop();
        CurrentPlayingMusicPath = "";
    }

    private const string BASE_SOUND_PATH = "res://Assets/Sounds";

    public void PlaySound(string soundName) {
        AudioStreamPlayer player = new();
        player.Stream = LoadSound(soundName);
        player.VolumeDb = Mathf.LinearToDb(SfxVolume);

        // Queues the node to be deleted when player.Finished emits.
        player.Finished += () => player.QueueFree();
        
        AddChild(player);
        player.Play();
    }

    private AudioStream LoadSound(string soundAssetName) {
        if (CachedMusic.TryGetValue(soundAssetName, out AudioStream audioStream))
            // Return already loaded asset
            return audioStream;

        // Music not loaded, first time setup
        audioStream = GD.Load<AudioStream>($"{BASE_SOUND_PATH}/{soundAssetName}");
        // ConfigureLoopingSound(audioStream);
        CachedMusic.Add(soundAssetName, audioStream);

        return audioStream;
    }

    #endregion Music-related

    #region Settings & configurability related

    public float MusicVolume {
        get => _musicVolume;
        set {
            _musicVolume = Mathf.Clamp(value, 0.0f, 1.0f);
            UpdateMusicVolume();
        }
    }

    private void UpdateMusicVolume() {
        if (!MusicMuted) {
            MusicPlayer.VolumeDb = Mathf.LinearToDb(MusicVolume);
        }
    }

    public bool ToggleMusicMuted() {
        MusicMuted = !MusicMuted;
        UpdateMusicMuted();
        return MusicMuted;
    }

    private void UpdateMusicMuted() {
        MusicPlayer.VolumeDb = MusicMuted
            ? -80
            : Mathf.LinearToDb(MusicVolume);
    }

    public bool ToggleSfxMuted() {
        return SfxMuted = !SfxMuted;
    }

    #endregion Settings & configurability related
}