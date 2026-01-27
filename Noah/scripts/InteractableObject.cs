using Godot;
using GodotInk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public partial class InteractableObject : Node2D
{
    [Export] string LabelText;

    [Export] float ActivationRange = 1;
    [Export] float VisibleRange = 2;
    [Export] private InkStory inkData;
    private InkStory _story;            // runtime instance

    public bool InRange, Activated = false;

    // short interact buffer so pressing e to end last dialog line doesn't reopen dialog
    public float CooldownTime = .1f;
    public float InteractCooldown;

    // reference to dialogUI
    private DialogUI DialogUI;

    // reference to choiceUI
    private ChoiceUI ChoiceUI;

    public override void _Ready()
    {
        // Attatch proceed logic to proceed signal
        GameManager.Instance.DialogProceed += DialogProceed;

        // get interaction icon and set visibility to false
        GetNode<RichTextLabel>("TextLabel").Visible = false;

        // set label to text to text specified in the field
        GetNode<RichTextLabel>("TextLabel").Text = LabelText;


        // get activation area and scale based on range var
        Area2D activationArea = GetNode<Area2D>("ActivationArea");
        CircleShape2D activationShape = (CircleShape2D)GetNode<CollisionShape2D>("ActivationArea/CollisionShape2D").Shape;
        activationShape.Radius = ActivationRange;

        // Add hooks to activationArea for onEnter and onExit
        activationArea.BodyEntered += OnBodyEnteredActivationArea;
        activationArea.BodyExited += OnBodyExitedActivationArea;

        // get visible area and scale based on range var
        Area2D visibleArea = GetNode<Area2D>("VisibleArea");
        CircleShape2D visibleShape = (CircleShape2D)GetNode<CollisionShape2D>("VisibleArea/CollisionShape2D").Shape;
        visibleShape.Radius = VisibleRange;

        visibleArea.BodyEntered += OnBodyEnteredVisibleArea;
        visibleArea.BodyExited += OnBodyExitedVisibleArea;

        // get reference to dialogUI 
        DialogUI = GetNode<DialogUI>("../UILayer/DialogUI");
        ChoiceUI = GetNode<ChoiceUI>("../UILayer/ChoiceUI");
    }

    

    private void OnBodyEnteredActivationArea(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // set bool flag to true and make label visible
            InRange = true;
            GetNode<RichTextLabel>("TextLabel").AddThemeColorOverride("font_outline_color", Colors.White);
            GetNode<RichTextLabel>("TextLabel").AddThemeColorOverride("default_color", Colors.Black);
        }
    }

    private void OnBodyExitedActivationArea(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // set inRange bool flag to false and make label invisible
            InRange = false;
            GetNode<RichTextLabel>("TextLabel").AddThemeColorOverride("font_outline_color", Colors.Transparent);
            GetNode<RichTextLabel>("TextLabel").AddThemeColorOverride("default_color", Colors.White);
        }
    }

    private void OnBodyEnteredVisibleArea(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // make text label visible
            GetNode<RichTextLabel>("TextLabel").Visible = true;
        }
    }

    private void OnBodyExitedVisibleArea(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // make text label invisible
            GetNode<RichTextLabel>("TextLabel").Visible = false;
        }
    }

    // To be called via player object interaction
    public async Task activateInteractable()
    {
        if (!Activated)
        {
            if(InteractCooldown <= 0)
            {
                // Sets activated flag to true (allowing input)
                // and makes dialogUI visible.
                // immediately shows first dialog line
                // GD.Print("Activated! Setting dialogUI to true and displaying next line!");

                Activated = true;
                DialogUI.Visible = true;

                _story = (InkStory)inkData.Duplicate();
                foreach (var kv in GameManager.Instance.GetFlags())
                {
                    _story.StoreVariable(kv.Key, kv.Value);
                    _story.ObserveVariable(kv.Key, new Callable(this, nameof(OnInkVariableChanged)));
                }

                await DisplayNextLine();
            }
            else
            {
                // GD.Print($"Can't interact, still on cooldown {interactCooldown}");
            }
        }
    }

    // When E or Space is pressed
    private async void DialogProceed()
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

    // Gets next line from inkData and displays it to dialogUI
    // also parses speaker names.
    private async Task DisplayNextLine()
    {
        

        if(_story == null) return;

        GameManager.Instance.SetDialogState(true); // set dialog state to true - pauses movement


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
    
        // End of dialog- cooldown timer
        InteractCooldown = CooldownTime;

        // End of dialog. 
        // hide dialog ui, set activated to false,
        // reset inkdata state
        // unpause player movement
        GD.Print("End of data, hiding dialogUI");
        DialogUI.Visible = false;
        DialogUI.DialogLine.VisibleRatio = 0;
        Activated = false;

        GameManager.Instance.SetDialogState(false); // set dialog state to false - frees movement

        _story = null; // reset story
    }

    public override void _PhysicsProcess(double delta)
    {
        if(InteractCooldown > 0)
        {
            InteractCooldown -= (float)delta;
        }
    }

    public void OnInkVariableChanged(string name, bool value)
    {
        GameManager.Instance.AddFlag(name, value);
    }


}
