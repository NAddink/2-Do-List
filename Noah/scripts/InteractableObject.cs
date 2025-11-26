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
                // block player movement
                GameManager.Instance.SetDialogState(true);

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



    // Input controls for dialogUI (only works when enabled == true)
    // Doesn't use the Input.Actions because then the first e press would skip the first line
    public override void _Input(InputEvent @event)
    {
        if (Activated)
        {
            if (@event is InputEventKey inputKey)
            {
                if(inputKey.Pressed)
                {
                    if(inputKey.Keycode == Key.Space || inputKey.Keycode == Key.E)
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

                string[] lineParts = currentLine.Split("$$$");
                
                if (lineParts.Length > 1)
                {
                    // line has a speaker name tagged on
                    // Kyle $$$ Hi my name is kyle
                    DialogUI.SpeakLine(lineParts[0].Trim(), lineParts[1].Trim());
                }
                else
                {
                    // GD.Print("Calling DialogUI speak line: " + lineParts[0].Trim());
                    DialogUI.SpeakLine(null, lineParts[0].Trim());
                }

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
                GameManager.Instance.SetDialogState(false);
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
