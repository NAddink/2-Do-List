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
        
        Node Level1Instance = Level1Scene.Instantiate();

        Node currentScene = GetTree().CurrentScene;
        Node root = GetTree().Root;

        // 3. Add the new scene as a child of the root
        Node2D player = (Node2D)Level1Instance.FindChild("Player");
        player.Position = new Vector2I(-141, 86);
        root.AddChild(Level1Instance);

        // 4. Set the new scene as the current scene for the SceneTree
        GetTree().CurrentScene = Level1Instance;

        // 5. Free the old scene (use CallDeferred to avoid issues if other nodes still need it this frame)
        currentScene.QueueFree();
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
