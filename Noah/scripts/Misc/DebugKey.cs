using Godot;
using System;

public partial class DebugKey : Node
{
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputEventKey)
        {
            // Check if the specific key is Key.E and if it was pressed down (not an echo/repeat)
            if (inputEventKey.PhysicalKeycode == Key.P && inputEventKey.Pressed && !inputEventKey.Echo)
            {
                GD.Print("The 'P' key was just pressed!");
                // Add your code here
                GameManager.Instance.ResetAllFlags();
                LabelPopup();
            }

        }
    }


    public async void LabelPopup()
    {
        Player player = (Player) GetTree().GetRoot().FindChild("Player", true, false);

        Label label = new Label();

        label.Text = "Debug: Reset all flags";
        label.Visible = true;
        label.Position = player.Position;

        GetTree().GetRoot().AddChild(label);

        await ToSignal(GetTree().CreateTimer(1.0), "timeout");

        label.QueueFree();
    }

}
