using System;
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

    public static SoundController Singleton =>
        (Engine.GetMainLoop() as SceneTree).CurrentScene.GetNode("/root/SoundController") as SoundController;

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
        set {
            _sfxVolume = Mathf.Clamp(value, 0.0f, 1.0f); ;
            UpdateAmbianceVolume();
        }
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

        MusicPlayer.Stop();
        MusicPlayer.Stream = null;
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

    #region Ambiance-related

    private const string BASE_AMBIANCE_PATH = "res://Assets/Sounds/Ambiance";

    private List<AudioStreamPlayer> AmbiancePlayers = new();
    private List<AudioStreamPlayer> playersToRemove = new();

    private float ambianceVolumeMultiplier = 3;

    private void UpdateAmbianceVolume() {
        foreach (AudioStreamPlayer player in AmbiancePlayers) {
            if (IsInstanceValid(player)) {
                player.VolumeDb = Mathf.LinearToDb(SfxVolume * ambianceVolumeMultiplier);
            } else {
                // Fade out before removing the player
                if (IsInstanceValid(player)) {
                    ReplayPlayerAmbiance(player);
                    Tween tween = CreateTween();
                    tween.TweenProperty(player, "volume_db", -80, 4.0f); // Fade out over 4 second
                    tween.Finished += () => {
                        playersToRemove.Add(player);
                        player.QueueFree();
                    };
                } else {
                    playersToRemove.Add(player);
                }
            }
        }

        foreach (AudioStreamPlayer player in playersToRemove) {
            AmbiancePlayers.Remove(player);
        }
    }

    public void PlayAmbianceType(AmbianceTypeEnum ambianceType, bool stopCurrent = true) {
        if (stopCurrent) {
            StopAmbianceType(ambianceType);
        }

        // Get all files in the corresponding folder for the ambiance type
        string folderPath = $"{BASE_AMBIANCE_PATH}/{ambianceType}";
        var dir = DirAccess.Open(folderPath);
        if (dir == null) {
            return;
        }

        List<string> files = dir.GetFiles().Where(f => f.EndsWith(".mp3")).ToList();

        if (files.Count == 0) {
            return;
        }

        // Pick a random file
        string ambianceName = $"{ambianceType}/{files[GD.RandRange(0, files.Count - 1)]}";
        if (ambianceName == "None") return;

        PlayAmbiance(ambianceName, ambianceType);
    }

    public void StopAmbianceType(AmbianceTypeEnum ambianceType) {
        string ambianceName = ambianceType.ToString();

        if (AmbiancePlayers.Count == 0 || ambianceName == "None") {
            return;
        }

        // Stop all ambiance players of the specified type
        for (int i = 0; i < AmbiancePlayers.Count; i++) {
            AudioStreamPlayer player = AmbiancePlayers[i];
            if (IsInstanceValid(player) == false) {
                AmbiancePlayers.RemoveAt(i);
                i--; // Adjust index after removal
                continue;
            }

            if (player.Name != ambianceName) {
                player.Stop();
                player.QueueFree();
                AmbiancePlayers.RemoveAt(i);
                i--; // Adjust index after removal
            }
        }
    }

    private void PlayAmbiance(string ambianceName, AmbianceTypeEnum ambianceType = AmbianceTypeEnum.None) {
        AudioStreamPlayer player = new();
        AmbiancePlayers.Add(player);

        player.Bus = "Ambiance";
        player.Name = ambianceType.ToString();

        player.Stream = LoadAmbiance(ambianceName);
        player.VolumeDb = -80;

        // Queues the node to be deleted when player.Finished emits.
        player.Finished += () => player.QueueFree();

        AddChild(player);
        player.Play();
        Tween tween = CreateTween();
        tween.TweenProperty(player, "volume_db", Mathf.LinearToDb(SfxVolume * ambianceVolumeMultiplier), 4.0f);
        tween.Finished += () => {
            if (IsInstanceValid(player)) {
                player.VolumeDb = Mathf.LinearToDb(SfxVolume * ambianceVolumeMultiplier);
            }
        };
    }

    private void ReplayPlayerAmbiance(AudioStreamPlayer player) {
        if (IsInstanceValid(player)) {
            player.Stop();
            player.Stream = LoadAmbiance(player.Name);
            player.Play();
        }
    }

    private AudioStream LoadAmbiance(string ambianceName) {
        if (CachedSounds.TryGetValue(ambianceName, out AudioStream audioStream))
            return audioStream;

        // Ambiance not loaded, first time setup
        audioStream = GD.Load<AudioStream>($"{BASE_AMBIANCE_PATH}/{ambianceName}");
        CachedSounds.Add(ambianceName, audioStream);

        return audioStream;
    }

    #endregion Ambiance-related
}