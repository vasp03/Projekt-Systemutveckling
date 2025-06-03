using System.Collections.Generic;
using System.Linq;
using Godot;
using Goodot15.Scripts.Game.Model.Enums;

namespace Goodot15.Scripts.Game.Controller;

public partial class SoundController : Node {
    public override void _ExitTree() {
        StopMusic();

        UnloadSounds();

        QueueFree();
    }

    /// <summary>
    ///     Triggers disposal of cached audio
    /// </summary>
    private void UnloadSounds() {
        CachedSounds.ToList().ForEach(e => e.Value.Dispose());
        CachedMusic.ToList().ForEach(e => e.Value.Dispose());
    }

    public static void ConfigureLoopingSound(AudioStream audioStreamAsset) {
        if (audioStreamAsset is AudioStreamOggVorbis oggStream) {
            oggStream.Loop = true;
        } else if (audioStreamAsset is AudioStreamWav wavStream) {
            wavStream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;

            wavStream.LoopEnd = wavStream.Data.Length;
        }
    }

    #region Sfx-related

    private readonly IDictionary<string, AudioStream> CachedSounds = new Dictionary<string, AudioStream>();

    private float _sfxVolume;


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
            _sfxVolume = Mathf.Clamp(value, 0.0f, 1.0f);
            UpdateAmbianceVolume(_sfxVolume);
        }
    }

    public override void _Ready() {
        SetupMusicPlayer();

        MusicVolume = SettingsManager.MusicVolume;
        SfxVolume = SettingsManager.SfxVolume;
    }

    /// <summary>
    ///     Plays a sound effect. Automatically loads and caches
    ///     the sound asset after with a specified pitch scale.
    /// </summary>
    /// <param name="soundName">Path and name of sound (excluding <b>res://Assets/Sounds/</b>)</param>
    /// <param name="pitchScale">Pitch of the sound being played, 1 plays the sound as normal.</param>
    public void PlaySound(string soundName, float pitchScale = 1f) {
        AudioStreamPlayer player = new();
        player.Stream = LoadSound(soundName);
        player.VolumeDb = Mathf.LinearToDb(SfxVolume);

        player.PitchScale = pitchScale;

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

    #endregion Sfx-related

    #region Music-related

    private const string BASE_MUSIC_PATH = "res://Assets/Music";

    private readonly IDictionary<string, AudioStream> CachedMusic = new Dictionary<string, AudioStream>();

    private AudioStreamPlayer MusicPlayer;

    private bool _musicMuted;
    private float _musicVolume;

    private string CurrentPlayingMusicPath;

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
    private void PlayMusic(string musicPath, bool hardCutSong = false) {
        if (CurrentPlayingMusicPath == musicPath) {
            MusicPlayer.Play();
            return;
        }

        if (MusicPlayer.Stream != null && MusicPlayer.Playing && !hardCutSong) {
            Tween tween = CreateTween();
            tween.TweenProperty(MusicPlayer, "volume_db", -80, 5f);
            tween.Finished += () => {
                MusicPlayer.Stop();
                LoadAndStartPlayingMusic(musicPath);
            };
        } else {
            if (MusicPlayer.Stream != null && MusicPlayer.Playing) MusicPlayer.Stop();

            LoadAndStartPlayingMusic(musicPath);
        }
    }

    private void LoadAndStartPlayingMusic(string musicPath) {
        MusicPlayer.Stream = LoadMusic(musicPath);
        CurrentPlayingMusicPath = musicPath;
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
    private readonly List<AudioStreamPlayer> AmbiancePlayers = new();
    private float ambianceVolumeMultiplier = 3;

    private void UpdateAmbianceVolume(float sfxVolume) {
        foreach (AudioStreamPlayer player in AmbiancePlayers)
            if (IsInstanceValid(player)) {
                player.VolumeDb = Mathf.LinearToDb(sfxVolume * ambianceVolumeMultiplier);
            } else {
                AmbiancePlayers.Remove(player);
                player.QueueFree();
            }
    }

    /// <summary>
    ///     Plays a random ambiance sound of the specified type.
    /// </summary>
    /// <param name="ambianceSoundType">The type of ambiance to play, e.g. Rain, Forest, etc.</param>
    /// <param name="stopCurrent">If true, stops any currently playing ambiance of the same type before playing the new one.</param>
    public void PlayAmbianceType(AmbianceSoundType ambianceSoundType) {
        if (ambianceSoundType == AmbianceSoundType.None ||
            AmbiancePlayers.Any(p => p.Name == ambianceSoundType.ToString())) return;

        if (ambianceSoundType == AmbianceSoundType.Wind)
            StopAmbianceType(AmbianceSoundType.Forest);
        else if (ambianceSoundType == AmbianceSoundType.Forest) StopAmbianceType(AmbianceSoundType.Wind);

        // Get all files in the corresponding folder for the ambiance type
        string folderPath = $"{BASE_AMBIANCE_PATH}/{ambianceSoundType}";
        DirAccess dir = DirAccess.Open(folderPath);
        if (dir == null) {
            GD.PrintErr($"Failed to open directory: {folderPath}, SoundController");
            return;
        }

        List<string> files = dir.GetFiles().Where(f => f.EndsWith(".mp3")).ToList();

        if (files.Count == 0) {
            GD.PrintErr($"No ambiance files found in: {folderPath}, SoundController");
            return;
        }

        // Pick a random file
        string ambianceName = $"{ambianceSoundType}/{files[GD.RandRange(0, files.Count - 1)]}";

        PlayAmbiance(ambianceName, ambianceSoundType);
    }

    /// <summary>
    ///     Stops all currently playing ambiance sounds of the specified type.
    /// </summary>
    /// <param name="ambianceTypes">The type of ambiance to stop, e.g. Rain, Forest, etc. from a list</param>
    public void StopAmbianceType(List<AmbianceSoundType> ambianceTypes) {
        foreach (AmbianceSoundType ambianceType in ambianceTypes) StopAmbianceType(ambianceType);
    }

    /// <summary>
    ///     Stops all currently playing ambiance sounds of the specified type.
    /// </summary>
    /// <param name="ambianceSoundType">The type of ambiance to stop, e.g. Rain, Forest, etc.</param>
    public void StopAmbianceType(AmbianceSoundType ambianceSoundType) {
        string ambianceName = ambianceSoundType.ToString();

        if (AmbiancePlayers.Count is 0) return;

        // Find and stop all players of the specified ambiance type
        for (int i = AmbiancePlayers.Count - 1; i >= 0; i--) {
            AudioStreamPlayer player = AmbiancePlayers[i];
            if (IsInstanceValid(player) && player.Name == ambianceName) {
                player.Stop();
                player.QueueFree();
                AmbiancePlayers.RemoveAt(i);
            } else {
                AmbiancePlayers.RemoveAt(i);
            }
        }
    }

    /// <summary>
    ///     Stops all currently playing ambiance sounds and clears the list of ambiance players.
    /// </summary>
    public void StopAllAmbiance() {
        // Stop all ambiance players
        foreach (AudioStreamPlayer player in AmbiancePlayers)
            if (IsInstanceValid(player)) {
                player.Stop();
                player.QueueFree();
            }

        AmbiancePlayers.Clear();
    }

    /// <summary>
    ///     Plays an ambiance sound with the specified name and type.
    /// </summary>
    /// <param name="ambianceName">Name coresponds to the file name in the Ambiance folder, e.g. "Rain" or "Forest"</param>
    private void PlayAmbiance(string ambianceName, AmbianceSoundType ambianceSoundType) {
        if (ambianceSoundType == AmbianceSoundType.None ||
            AmbiancePlayers.Any(p => p.Name == ambianceSoundType.ToString())) return;

        AudioStreamPlayer player = new();
        AmbiancePlayers.Add(player);

        player.Bus = "Ambiance";
        player.Name = ambianceSoundType.ToString();

        player.Stream = LoadAmbiance(ambianceName);
        player.VolumeDb = -Mathf.LinearToDb(SfxVolume * ambianceVolumeMultiplier);

        // Queues the node to be deleted when player.Finished emits.
        player.Finished += () => {
            if (AmbiancePlayers.Contains(player)) AmbiancePlayers.Remove(player);

            player.QueueFree();
        };

        AddChild(player);
        player.Play();
    }

    /// <summary>
    ///     Loads the specified ambiance sound asset and caches it in memory
    /// </summary>
    /// <param name="ambianceName">Name of the ambiance sound asset to be loaded, uses BASE_AMBIANCE_PATH</param>
    private AudioStream LoadAmbiance(string ambianceName) {
        if (CachedSounds.TryGetValue(ambianceName, out AudioStream audioStream))
            return audioStream;

        // Ambiance not loaded, first time setup
        audioStream = GD.Load<AudioStream>($"{BASE_AMBIANCE_PATH}/{ambianceName}");
        CachedSounds.Add(ambianceName, audioStream);

        return audioStream;
    }

    public void LogAllAmbiancePlaying() {
        GD.PrintErr("--------------------------");
        GD.PrintErr("Currently playing ambiance sounds:");
        foreach (AudioStreamPlayer player in AmbiancePlayers)
            if (IsInstanceValid(player))
                GD.PrintErr($"{player.Name} ({player.Stream.ResourcePath})");
            else
                GD.PrintErr("Invalid player instance");
    }

    /// <summary>
    ///     Forces the currently playing song to stop immediately, clearing the current playing music path.
    /// </summary>
    public void ForceStopSong() {
        if (MusicPlayer != null && MusicPlayer.Playing) MusicPlayer.Stop();
        CurrentPlayingMusicPath = string.Empty;
    }

    #endregion Ambiance-related
}