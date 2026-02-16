using Godot;
using GodotInk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Tool]
public partial class InteractableObject : ActivatableObject
{
    [Export]
    public string LabelText
    {
        get => _labelText;
        set
        {
            _labelText = value;
            UpdateLabelText();
        }
    }
    private string _labelText = "";

    [Export]
    public float ActivationRange
    {
        get => _activationRange;
        set
        {
            _activationRange = value;
            UpdateActivationArea();
            QueueRedraw();
        }
    }
    private float _activationRange = 1f;

    [Export]
    public float VisibleRange
    {
        get => _visibleRange;
        set
        {
            _visibleRange = value;
            UpdateVisibleArea();
            QueueRedraw();
        }
    }
    private float _visibleRange = 2f;

    [Export]
    public float LabelYOffset
    {
        get => _labelYOffset;
        set
        {
            _labelYOffset = value;
            UpdateLabelOffset();
        }
    }
    private float _labelYOffset = 0f;

    private Area2D _activationArea;
    private Area2D _visibleArea;



    [Export] private InkStory inkData;
    private InkStory _story;            // runtime instance

    // short interact buffer so pressing e to end last dialog line doesn't reopen dialog
    public float CooldownTime = .1f;
    public float InteractCooldown;

    // reference to dialogUI
    protected DialogUI DialogUI;

    // reference to choiceUI
    protected ChoiceUI ChoiceUI;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            EditorSetup();
            return;
        }

        RuntimeSetup();
    }

    private void RuntimeSetup()
    {

        CreateActivationArea();
        CreateVisibleArea();
        
        
        // Attatch proceed logic to proceed signal
        GameManager.Instance.DialogProceed += DialogProceed;

        // TEXT LABEL
        RichTextLabel label = GetNode<RichTextLabel>("TextLabel");
        // get interaction icon and set visibility to false
        label.Visible = false;

        // set label to text to text specified in the field
        label.Text = LabelText;

        // Set label offset
        var pos = label.Position;
        pos.Y = _labelYOffset;
        label.Position = pos;


        // get reference to dialogUI 
        DialogUI = GetTree().GetRoot().GetNode<DialogUI>("Level/UI/DialogUI");
        ChoiceUI = GetTree().GetRoot().GetNode<ChoiceUI>("Level/UI/ChoiceUI");

        
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

    public override void Activate()
    {
        if (Activated) return;
        if (InteractCooldown > 0) return;
        if (!InRange) return;
        
        _ = ActivateInternal();
    }

    // For subclasses, Activate remains unchanged but ActivateInternal logic can be changed
    protected virtual async Task ActivateInternal()
    {
        await activateInteractable();
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
                try
                {
                    foreach (var kv in GameManager.Instance.GetFlags())
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
            else
            {
                // GD.Print($"Can't interact, still on cooldown {interactCooldown}");
            }
        }
    }

    // When E or Space is pressed
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

        // Exit dialog
        ExitDialog();

        _story = null; // reset story
    }

    public virtual void ExitDialog()
    {
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
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        if(InteractCooldown > 0)
        {
            InteractCooldown -= (float)delta;
        }
    }

    public void OnInkVariableChanged(string name, bool value)
    {
        GameManager.Instance.AddFlag(name, value);
    }


    
    private void CreateActivationArea()
    {
        _activationArea = new Area2D();
        _activationArea.Name = "ActivationArea";

        var collision = new CollisionShape2D();
        var shape = new CircleShape2D();
        shape.Radius = ActivationRange;

        collision.Shape = shape;

        _activationArea.AddChild(collision);
        AddChild(_activationArea);

        _activationArea.BodyEntered += OnBodyEnteredActivationArea;
        _activationArea.BodyExited += OnBodyExitedActivationArea;
    }

    private void CreateVisibleArea()
    {
        _visibleArea = new Area2D();
        _visibleArea.Name = "VisibleArea";

        var collision = new CollisionShape2D();
        var shape = new CircleShape2D();
        shape.Radius = VisibleRange;

        collision.Shape = shape;

        _visibleArea.AddChild(collision);
        AddChild(_visibleArea);

        _visibleArea.BodyEntered += OnBodyEnteredVisibleArea;
        _visibleArea.BodyExited += OnBodyExitedVisibleArea;
    }


    // =============================================
    // Everything below here is for range visibility in editor
    // Not related to game logic
    // =============================================

    // Code for updating areas visually in editor for creating scenes
    private void EditorSetup()
    {
        QueueRedraw();

        var label = GetNodeOrNull<RichTextLabel>("TextLabel");
        if (label != null)
        {
            label.Text = LabelText;
            label.Visible = true;
        }
    }

    // Helper methods for showing ranges and text in editor
    private void UpdateActivationArea()
    {
        if (_activationArea == null) return;

        var shape = _activationArea
            .GetNode<CollisionShape2D>("CollisionShape2D")
            .Shape as CircleShape2D;

        if (shape != null)
            shape.Radius = _activationRange;
    }

    private void UpdateVisibleArea()
    {
        if (_visibleArea == null) return;

        var shape = _visibleArea.GetNode<CollisionShape2D>("CollisionShape2D").Shape as CircleShape2D;

        if(shape != null)
        {
            shape.Radius = _visibleRange;
        }

    }

    private void UpdateLabelText()
    {
        if (!IsInsideTree())
        return;

        RichTextLabel label = GetNodeOrNull<RichTextLabel>("TextLabel");
        if (label == null)
            return;

        label.Text = _labelText;

    }

        private void UpdateLabelOffset()
    {
        if (!IsInsideTree())
            return;

        var label = GetNodeOrNull<RichTextLabel>("TextLabel");
        if (label == null)
            return;

        var pos = label.Position;
        pos.Y = _labelYOffset;
        label.Position = pos;
    }


    public override void _Draw()
    {
        if (!Engine.IsEditorHint())
            return;

        DrawCircle(Vector2.Zero, ActivationRange, new Color(1, 0, 0, 0.25f));
        DrawCircle(Vector2.Zero, VisibleRange, new Color(0, 1, 0, 0.25f));
    }


}
