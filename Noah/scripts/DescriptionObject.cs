using Godot;
using GodotInk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Tool]
public partial class DescriptionObject : ActivatableObject
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

    [Export(PropertyHint.MultilineText)]
    public string DescriptionText;


    [Export]
    public float ActivationRange
    {
        get => _activationRange;
        set
        {
            _activationRange = value;
            UpdateActivationArea();
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



    // short interact buffer so pressing e to end last dialog line doesn't reopen dialog
    public float CooldownTime = .1f;
    public float InteractCooldown;

    // reference to dialogUI
    private DialogUI DialogUI;

    // reference to choiceUI
    private ChoiceUI ChoiceUI;

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


    public override void Activate()
    {
        _ = activateDescription();
    }


    // To be called via player object interaction
    public async Task activateDescription()
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
                // would normally display next line, since description object only has one line- end
                InteractCooldown = CooldownTime;
                GD.Print("End of data, hiding dialogUI");
                DialogUI.Visible = false;
                DialogUI.DialogLine.VisibleRatio = 0;
                Activated = false;

                GameManager.Instance.SetDialogState(false); // set dialog state to false - frees movement
            }
        }
    }

    // Gets next line from inkData and displays it to dialogUI
    // also parses speaker names.
    private async Task DisplayNextLine()
    {
        
        DialogUI.SpeakLine("MC $$$ " + DescriptionText);
        // End of dialog- cooldown timer
            

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




    // Code for updating areas visually in editor for creating scenes
    private void EditorSetup()
    {
        UpdateActivationArea();
        UpdateVisibleArea();

        // Optional: preview label text
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
        var shape = GetNodeOrNull<CollisionShape2D>(
            "ActivationArea/CollisionShape2D"
        )?.Shape as CircleShape2D;

        if (shape != null)
            shape.Radius = _activationRange;
    }

    private void UpdateVisibleArea()
    {
        var shape = GetNodeOrNull<CollisionShape2D>(
            "VisibleArea/CollisionShape2D"
        )?.Shape as CircleShape2D;

        if (shape != null)
            shape.Radius = _visibleRange;
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


}
