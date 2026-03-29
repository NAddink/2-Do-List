using Godot;
using GodotInk;
using System;
using System.Threading.Tasks;

public partial class CutsceneManager : Node
{

    private GameManager GameManager;
    protected DialogUI DialogUI;

    [Export] private InkStory inkData;

    private InkStory _story;

    private bool Activated;

    private PackedScene MainMenuScene;


    public override void _Ready()
    {
        Activated = false;
        GameManager = GetTree().Root.GetNode<GameManager>("GameManager");
        DialogUI = GetNode<DialogUI>("../DialogUI");
        GameManager.LevelComplete += LevelComplete;
        GameManager.DialogProceed += DialogProceed;

        MainMenuScene = GD.Load<PackedScene>("res://Noah/scenes/MainMenu.tscn");
    }

    private void LevelComplete()
    {
        DemoCutscene();
    }

    private async void DemoCutscene()
    {
        CanvasLayer canvasLayer = (CanvasLayer)GetTree().Root.FindChild("UI", true, false);

        ColorRect blackPanel = new ColorRect();
        blackPanel.Color = Colors.Black; 
        blackPanel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        blackPanel.ZIndex = -1;

        canvasLayer.AddChild(blackPanel);

        DialogUI.Visible = true;
        GameManager.SetDialogState(true);

        Activated = true;
        _story = (InkStory)inkData.Duplicate();
        await DisplayNextLine();
    }

    protected virtual async void DialogProceed()
    {
        if (Activated)
        {

            if (DialogUI.IsAnimating)
            {
                GD.Print("Skipping current animation");
                DialogUI.SkipTextAnimation();
            }
            else
            {
                await DisplayNextLine();
            }
        }
    }
    


    

    private async Task DisplayNextLine()
    {
        

        if(_story == null) return;

        GameManager.SetDialogState(true); // set dialog state to true - pauses movement


        while (_story.CanContinue && _story.CurrentChoices.Count == 0)
        {
            string line = _story.Continue().Trim();
            if (!string.IsNullOrEmpty(line))
            {
                DialogUI.SpeakLine(line);
                return;
            }
        }
    

        

        // Exit dialog
        ExitDialog();

        _story = null; // reset story
    }

    public void ExitDialog()
    {

        // End of dialog. 
        // hide dialog ui, set activated to false,
        // reset inkdata state
        // unpause player movement
        GD.Print("End of data, hiding dialogUI");
        DialogUI.Visible = false;
        DialogUI.DialogLine.VisibleRatio = 0;
        Activated = false;

        GameManager.SetDialogState(false); // set dialog state to false - frees movement

        LoadMainMenu();
    }

    private void LoadMainMenu()
    {

        GetTree().Root.GetNode<GameManager>("GameManager").ResetAllFlags();
        
        Node levelInstance = MainMenuScene.Instantiate();

        Node currentScene = GetTree().CurrentScene;
        Node root = GetTree().Root;

        // Set current scene to level1
        root.AddChild(levelInstance);
        GetTree().CurrentScene = levelInstance;

        if (currentScene != null)
        {
            currentScene.SetProcess(false);
            currentScene.SetPhysicsProcess(false);
            currentScene.CallDeferred("queue_free");
        }
    }

    public override void _ExitTree()
    {
        if (GameManager != null)
        {
            GameManager.DialogProceed -= DialogProceed;
            GameManager.LevelComplete -= LevelComplete; // if used
        }
    }



}
