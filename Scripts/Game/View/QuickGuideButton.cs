using Godot;
using Goodot15.Scripts.Game.Controller;
using Goodot15.Scripts.Game.Controller.Events;
using Goodot15.Scripts.Game.Model.Interface;
using Goodot15.Scripts.Game.View;
using System;

public partial class QuickGuideButton : TextureButton {
    public override void _Ready() {
        Pressed += OnButtonPressed;
        TextureNormal = GD.Load<Texture2D>("res://Assets/Icons/64x64/fc9.png");
    }

    public void OnButtonPressed() {
        SoundController.Singleton.PlaySound("General Sounds/Buttons/sfx_sounds_button11.wav");
        GameController.Singleton.MenuController.QuickOpenGuideMenu();

        if (GameController.Singleton.GameEventManager.EventInstance<DayTimeEvent>() is IPausable pausable) pausable.SetPaused(true);

        SoundController.Singleton.MusicMuted = true;
        GameController.Singleton.HideHUD();
        Visible = false;
    }
}
