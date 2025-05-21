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
    
    public static SoundController Singleton => (Engine.GetMainLoop() as SceneTree).CurrentScene.GetNode("/root/SoundController") as SoundController;
    /// <summary>
    ///     Settings manager instance reference
    /// </summary>
    private SettingsManager SettingsManager => GetNode<SettingsManager>("/root/SettingsManager");

    /// <summary>
    ///     Determines if sound effects are muted
    /// </summary>
    public bool SfxMuted { get; set; }


    public float SfxVolume {
        get => _sfxVolume;
        set => _sfxVolume = Mathf.Clamp(value, 0.0f, 1.0f);
    }

    public override void _Ready() {
        SetupMusicPlayer();

        MusicVolume = SettingsManager.MusicVolume;
        SfxVolume = SettingsManager.SfxVolume;
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

    /// <summary>
    ///     Helper method for OnMusicFinished class
    ///     Checks to see if a song should replay after .finished has emitted
    ///     Necessary cause MP3 songs don't have built in looping through Godot
    /// </summary>
    /// <returns></returns>
    private bool ShouldCurrentSongLoop() {
        return !string.IsNullOrEmpty(CurrentPlayingMusicPath) && CurrentPlayingMusicPath.Contains("DayTimeSongs/Day");
    }

    /// <summary>
    ///     Plays regular (main) menu music
    /// </summary>
    public void PlayMenuMusic() {
        PlayMusic("xDeviruchi/02 - Title Theme.wav");
    }

    /// <summary>
    ///     Plays regular game music
    /// </summary>
    public void PlayGameMusic() {
        PlayMusic("xDeviruchi/03 - Definitely Our Town.wav");
    }

    public void PlayShopMusic() {
        PlayMusic("xDeviruchi/08 - Shop.wav");
    }

    /// <summary>
    ///     Plays a song with for the specified <see cref="dayTime" /> DayTime value. Automatically loads and caches the sound
    ///     asset after
    /// </summary>
    /// <param name="dayTime">Relevant Day-time</param>
    public void PlayDayTimeSong(string dayTime) {
        string musicPath = $"DayTimeSongs/{dayTime}.mp3";

        if (CurrentPlayingMusicPath == musicPath) {
            MusicPlayer.Play();
            return;
        }

        PlayMusic(musicPath);
    }

    /// <summary>
    ///     Plays the specified <see cref="musicPath" /> music.
    /// </summary>
    /// <param name="musicPath">Music path, only the music path is needed</param>
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

    /// <summary>
    ///     Loads the specified music sound asset and caches it in memory
    /// </summary>
    /// <param name="soundAssetName">Music asset to be loaded</param>
    /// <returns>Loaded music as an AudioStream</returns>
    private AudioStream LoadMusic(string soundAssetName) {
        if (CachedMusic.TryGetValue(soundAssetName, out AudioStream audioStream)) return audioStream;

        // Music not loaded, first time setup
        audioStream = GD.Load<AudioStream>($"{BASE_MUSIC_PATH}/{soundAssetName}");
        ConfigureLoopingSound(audioStream);
        CachedMusic.Add(soundAssetName, audioStream);

        return audioStream;
    }

    /// <summary>
    ///     Stops playing the current music
    /// </summary>
    public void StopMusic() {
        MusicPlayer?.Stop();
        CurrentPlayingMusicPath = "";
    }

    private const string BASE_SOUND_PATH = "res://Assets/Sounds";

    /// <summary>
    ///     Plays a sound effect with for the specified <see cref="dayTime" /> DayTime value. Automatically loads and caches
    ///     the sound asset after
    /// </summary>
    /// <param name="soundName">Path and name of sound (excluding <b>res://Assets/Sounds/</b>)</param>
    public void PlaySound(string soundName) {
        AudioStreamPlayer player = new();
        player.Stream = LoadSound(soundName);
        player.VolumeDb = Mathf.LinearToDb(SfxVolume);

        // Queues the node to be deleted when player.Finished emits.
        player.Finished += () => player.QueueFree();

        AddChild(player);
        player.Play();
    }

    /// <summary>
    ///     Loads the specified sound effect asset and caches it in memory
    /// </summary>
    /// <param name="soundAssetName">Music asset to be loaded</param>
    /// <returns>Loaded sound effect as an AudioStream</returns>
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

    /// <summary>
    ///     Determines if music is muted or not
    /// </summary>
    public bool MusicMuted {
        get => _musicMuted;
        set {
            _musicMuted = value;
            UpdateMusicMuted();
        }
    }

    /// <summary>
    ///     The current music volume, range is <b>0-1</b> inclusive.
    /// </summary>
    public float MusicVolume {
        get => _musicVolume;
        set {
            _musicVolume = Mathf.Clamp(value, 0.0f, 1.0f);
            UpdateMusicVolume();
        }
    }

    /// <summary>
    ///     Fired when music volume changes, applies the new music volume to the music player
    /// </summary>
    private void UpdateMusicVolume() {
        if (!MusicMuted) MusicPlayer.VolumeDb = Mathf.LinearToDb(MusicVolume);
    }

    /// <summary>
    ///     Toggles if music is muted or not
    /// </summary>
    /// <returns>True or false if music was just muted or not</returns>
    public bool ToggleMusicMuted() {
        MusicMuted = !MusicMuted;
        UpdateMusicMuted();
        return MusicMuted;
    }

    /// <summary>
    ///     Fired when music is muted changes, applies the new muted property to the music player
    /// </summary>
    private void UpdateMusicMuted() {
        MusicPlayer.VolumeDb = MusicMuted
            ? -80
            : Mathf.LinearToDb(MusicVolume);
    }

    /// <summary>
    ///     Toggles if sound effects are muted or not
    /// </summary>
    /// <returns>True or false if sound effects were just muted or not</returns>
    public bool ToggleSfxMuted() {
        return SfxMuted = !SfxMuted;
    }

    #endregion Settings & configurability related
}