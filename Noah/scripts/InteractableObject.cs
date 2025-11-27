using Godot;
using GodotInk;
using System;

public partial class InteractableObject : Node2D
{
    [Export] float range = 1;
    [Export] private InkStory inkData;

    public bool InRange, Activated = false;

    // short interact buffer so pressing e to end last dialog line doesn't reopen dialog
    public float CooldownTime = .1f;
    public float InteractCooldown;

    // reference to dialogUI
    private DialogUI DialogUI;

    public override void _Ready()
    {
        // Attatch proceed logic to proceed signal
        GameManager.Instance.DialogProceed += DialogProceed;

        // get interaction icon and set visibility to false
        GetNode<Sprite2D>("InteractIcon").Visible = false;

        // get activation area and scale based on range var
        Area2D activationArea = GetNode<Area2D>("Area2D");
        activationArea.Scale = new Vector2(range, range);

        // Add hooks to activationArea for onEnter and onExit
        activationArea.BodyEntered += OnBodyEnteredActivationArea;
        activationArea.BodyExited += OnBodyExitedActivationArea;

        // get reference to dialogUI 
        DialogUI = GetNode<DialogUI>("/root/MovementTest/DialogLayer/DialogUI");
        if(DialogUI == null)
        {
            GD.Print("Could not find dialogUI");
        }
    }

    

    private void OnBodyEnteredActivationArea(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // set bool flag to true and make sprite invisible
            InRange = true;
            GetNode<Sprite2D>("InteractIcon").Visible = true;
        }
    }

    private void OnBodyExitedActivationArea(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // set inRange bool flag to false and make sprite invisible
            InRange = false;
            GetNode<Sprite2D>("InteractIcon").Visible = false;
        }
    }

    // To be called via player object interaction
    public void activateInteractable()
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
                DisplayNextLine();
            }
            else
            {
                // GD.Print($"Can't interact, still on cooldown {interactCooldown}");
            }
        }
    }

    // When E or Space is pressed
    private void DialogProceed()
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
                DisplayNextLine();
            }
        }
    }

    // Gets next line from inkData and displays it to dialogUI
    // also parses speaker names.
    private void DisplayNextLine()
    {
        

        if(inkData != null)
        {

            if (inkData.CanContinue)
            {
                string currentLine = inkData.Continue();
                DialogUI.SpeakLine(currentLine);

            }
            else
            {
                // End of dialog- cooldown timer
                InteractCooldown = CooldownTime;

                // End of dialog. 
                // hide dialog ui, set activated to false,
                // reset inkdata state
                // unpause player movement
                GD.Print("End of data, hiding dialogUI");
                DialogUI.Visible = false;
                Activated = false;
                inkData.ResetState();
            }

        }
        else
        {
            GD.Print("No ink data found");
            return;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if(InteractCooldown > 0)
        {
            InteractCooldown -= (float)delta;
        }
    }


}
