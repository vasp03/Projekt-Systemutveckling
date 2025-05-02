using Godot;

public partial class TimeHudController {
    [Export] private Label Label;

    public void UpdateTimeDisplay(string time) {
        GD.Print("Updating time display to: " + time);

        if (Label != null) {
            Label.Text = time;
        }
    }
}