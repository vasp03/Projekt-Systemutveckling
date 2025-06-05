using Godot;
using Goodot15.Scripts.Game.Controller;

namespace Goodot15.Scripts.Game.View;

public partial class ButtonWithSound : Button {
    private const string GENERIC_CLICK_SFX = "General Sounds/Buttons/sfx_sounds_button11.wav";
    protected virtual string ClickSfx => GENERIC_CLICK_SFX;

    public override void _Ready() {
        ButtonDown += OnButtonDown;
    }

    private void OnButtonDown() {
        SoundController tempQualifier = SoundController.Singleton;
        tempQualifier.PlaySound(ClickSfx);
    }

    public static void PlayGenericClickSound() {
        SoundController tempQualifier = SoundController.Singleton;
        tempQualifier.PlaySound(GENERIC_CLICK_SFX);
    }
}