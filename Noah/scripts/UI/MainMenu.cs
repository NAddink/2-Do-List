using Godot;
using System;

public partial class MainMenu : Control
{
    [Export] Button NewGame;
    [Export] Button LoadAutoSave;
    [Export] Button Options;
    [Export] Button Exit;

    private PackedScene Level1Scene;

    public override void _Ready()
    {
        NewGame.Pressed += NewGameButtonPress;
        LoadAutoSave.Pressed += LoadAutoSaveButtonPress;
        Options.Pressed += OptionsButtonPress;
        Exit.Pressed += ExitButtonPress;

        Level1Scene = GD.Load<PackedScene>("res://Noah/scenes/Levels/Level1.tscn");
    }







    private void NewGameButtonPress()
    {
        GetTree().ChangeSceneToFile("res://Noah/scenes/Levels/Level1.tscn");
    }

    private void LoadAutoSaveButtonPress()
    {
        
        Node levelInstance = Level1Scene.Instantiate();

        Node currentScene = GetTree().CurrentScene;
        Node root = GetTree().Root;

        // Set current scene to level1
        root.AddChild(levelInstance);
        GetTree().CurrentScene = levelInstance;

        // 🔑 Delay loading until scene is fully ready
        CallDeferred(nameof(ApplySaveData), levelInstance);

        if (currentScene != null)
        {
            currentScene.SetProcess(false);
            currentScene.SetPhysicsProcess(false);
            currentScene.CallDeferred("queue_free");
        }
    }

    private void ApplySaveData(Node levelInstance)
    {
        if (!FileAccess.FileExists("user://savegame.save"))
        {
            GD.Print("No save file found");
            return;
        }

        using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);

        while (saveFile.GetPosition() < saveFile.GetLength())
        {
            string line = saveFile.GetLine();
            var json = Json.ParseString(line);

            if (json.VariantType != Variant.Type.Dictionary)
                continue;

            var data = (Godot.Collections.Dictionary)json;

            if (!data.ContainsKey("NodeName"))
                continue;

            string nodeName = (string)data["NodeName"];

            Node foundNode = levelInstance.FindChild(nodeName);

            if (foundNode is Node2D node2D)
            {
                float x = (float)(double)data["PosX"];
                float y = (float)(double)data["PosY"];

                node2D.Position = new Vector2(x, y);
            }
        }
    }

    private void OptionsButtonPress()
    {
        throw new NotImplementedException();
    }

    private void ExitButtonPress()
    {
        throw new NotImplementedException();
    }
    

}
