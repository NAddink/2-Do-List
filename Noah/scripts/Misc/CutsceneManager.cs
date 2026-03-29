using Godot;
using GodotInk;
using System;
using System.Threading.Tasks;

public partial class CutsceneManager : Node
{

    private GameManager GameManager;
    protected DialogUI DialogUI;
    protected ChoiceUI ChoiceUI;

    [Export] private InkStory inkData;

    private InkStory _story;

    private bool Activated;

    private PackedScene MainMenuScene;


    public override void _Ready()
    {
        Activated = false;
        GameManager = GetTree().Root.GetNode<GameManager>("GameManager");
        DialogUI = GetNode<DialogUI>("../DialogUI");
        ChoiceUI = GetNode<ChoiceUI>("../ChoiceUI");
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

        // Wait until dialog is finished
        while (GameManager.IsInDialog)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        

        CanvasLayer canvasLayer = (CanvasLayer)GetTree().Root.FindChild("UI", true, false);

        ColorRect blackPanel = new ColorRect();
        blackPanel.Color = Colors.Black; 
        blackPanel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        blackPanel.ZIndex = -1;
        blackPanel.MouseFilter = Control.MouseFilterEnum.Ignore;

        // Start fully transparent
        blackPanel.Modulate = new Color(1, 1, 1, 0);

        canvasLayer.AddChild(blackPanel);

        // Create tween
        var tween = CreateTween();
        tween.TweenProperty(blackPanel, "modulate:a", 1.0f, 1.5f); // fade to black over 1.5 sec

        // Wait for fade to finish
        await ToSignal(tween, Tween.SignalName.Finished);

        // Optional extra delay after fade
        await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);

        DialogUI.Visible = true;
        GameManager.SetDialogState(true);

        Activated = true;
        _story = (InkStory)inkData.Duplicate();
        try
        {
            foreach (var kv in GameManager.GetFlags())
            {
                _story.StoreVariable(kv.Key, kv.Value);
                _story.ObserveVariable(kv.Key, new Callable(this, nameof(OnInkVariableChanged)));
            }
        }
        catch (Exception e)
        {
            GD.PrintErr("=====\n=====\nIssue with flags, some flags are missing or misformed." + e.Message);
            GD.PrintErr("Make sure the flags are the same in globals.ink AND gamemanager.cs\n=====\n=====");
        }
        await DisplayNextLine();
    }



    // Below is dialog logic taken from interactableobject

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


        if (_story.CurrentChoices.Count > 0)
        {
            // ChoiceUI is already open, return
            if(ChoiceUI.Visible) return;

            // Choice
            GD.Print("Choice!");

            DialogUI.Visible = false; // hide dialog UI

            // get array of each choice
            string[] choices = new string[_story.CurrentChoices.Count];
            for(int i = 0; i < _story.CurrentChoices.Count; i++)
            {
                GD.Print(_story.CurrentChoices[i].Text);
                choices[i] = _story.CurrentChoices[i].Text;
            }
            
            ChoiceUI.SetChoices(choices);
            ChoiceUI.Visible = true;



            var result = await ToSignal(ChoiceUI, ChoiceUI.SignalName.SelectionMade);
            int choiceIdx = (int)result[0];

            GD.Print("Button " + choiceIdx + " selected.");

            ChoiceUI.Visible = false; // hide choiceUI
            ChoiceUI.ClearChoices(); // remove all choice buttons

            // select choice via ink and proceed
            _story.ChooseChoiceIndex(choiceIdx); 

            DialogUI.Visible = true; // display dialogUI again
            await DisplayNextLine();
            return;

            
            
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

    public void OnInkVariableChanged(string name, bool value)
    {
        GameManager.AddFlag(name, value);
    }

    // Loads main menu to restart demo
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
