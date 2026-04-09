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

            if (!inputEventKey.Pressed || inputEventKey.Echo)
            return;

            // Require ALT to be held
            if (!inputEventKey.AltPressed)
                return;
            
            if (inputEventKey.PhysicalKeycode == Key.P)
            {
                GameManager.ResetAllFlags();
                LabelPopup("Debug: Reset all flags");
            }

            if (inputEventKey.PhysicalKeycode == Key.L)
            {
                GameManager.LoadChoices();
                LabelPopup("Debug: Force loaded current flags");
            }

            if (inputEventKey.PhysicalKeycode == Key.N)
            {
                
                GameManager.SaveManager.SaveNodeData();
                LabelPopup("Debug: Added player pos to savegame");
                
            }
            
            if (inputEventKey.PhysicalKeycode == Key.M)
            {
                
                GameManager.OnListComplete();
                LabelPopup("Debug: Forced List Complete state");

                
            }

            if (inputEventKey.PhysicalKeycode == Key.N)
            {
                // Fully restart godot app
                string executablePath = OS.GetExecutablePath();
                OS.ExecuteWithPipe(executablePath, new string[] { }, false);
                GetTree().Quit();
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

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GetTree().Quit();
        }
    }

}
