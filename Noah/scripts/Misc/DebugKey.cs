using Godot;
using System;

public partial class DebugKey : Node
{

    private GameManager GameManager;

    public override void _Ready()
    {
        GameManager = GetTree().Root.GetNode<GameManager>("GameManager");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputEventKey)
        {
            if (inputEventKey.PhysicalKeycode == Key.P && inputEventKey.Pressed && !inputEventKey.Echo)
            {
                GameManager.ResetAllFlags();
                LabelPopup("Debug: Reset all flags");
            }

            if (inputEventKey.PhysicalKeycode == Key.L && inputEventKey.Pressed && !inputEventKey.Echo)
            {
                GameManager.LoadChoices();
                LabelPopup("Debug: Force loaded current flags");
            }

            if (inputEventKey.PhysicalKeycode == Key.N && inputEventKey.Pressed && !inputEventKey.Echo)
            {
                
                GameManager.SaveManager.SaveNodeData();
                LabelPopup("Debug: Added player pos to savegame");
                
            }

        }
    }


    public async void LabelPopup(string text)
    {
        Player player = (Player) GetTree().GetRoot().FindChild("Player", true, false);

        Label label = new Label();

        label.Text = text;
        label.Visible = true;
        label.Position = player.Position;

        GetTree().GetRoot().AddChild(label);

        await ToSignal(GetTree().CreateTimer(1.0), "timeout");

        label.QueueFree();
    }

}
