using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class ButtonWithSound : Button {
    private const string GENERIC_CLICK_SFX = "General Sounds/Buttons/sfx_sounds_button11.wav";
    public virtual string ClickSfx => GENERIC_CLICK_SFX;

    public override void _Ready() {
        ButtonDown += OnButtonDown;
    }

    private void OnButtonDown() {
        SoundController.Singleton.PlaySound(ClickSfx);
    }

    public static void PlayGenericClickSound() {
        SoundController.Singleton.PlaySound(GENERIC_CLICK_SFX);
    }
}