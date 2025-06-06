using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;

public partial class QuickGuideButton : TextureButton {
    public override void _Ready() {
        Pressed += OnButtonPressed;
        TextureNormal = ResourceLoader.Load<Texture2D>("res://Assets/Icons/64x64/fc289.png");
    }

    private void OnButtonPressed() {
        GameController.Singleton.MenuController.QuickOpenGuideMenu();

        if (GameController.Singleton.GameEventManager.EventInstance<DayTimeEvent>() is IPausable pausable)
            pausable.SetPaused(true);

        SoundController.Singleton.MusicMuted = true;
        GameController.Singleton.HideHUD();
        Visible = false;
    }
}