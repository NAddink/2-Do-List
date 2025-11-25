using Godot;
using GodotInk;
using System;

public partial class InteractableObject : Node2D
{
    [Export] float range = 1;
    [Export(PropertyHint.MultilineText)] string dataParameter;

    InteractableData data;


    public bool inRange = false;
    public bool activated = false;
    private bool playerInRange;
    private Sprite2D interactionIcon;

    // reference to dialogUI
    private DialogUI dialogUI;

    public override void _Ready()
    {
        // get interaction icon and set visibility to false
        interactionIcon = GetNode<Sprite2D>("InteractIcon");
        interactionIcon.Visible = false;

        // get activation area and scale based on range var
        Area2D activationArea = GetNode<Area2D>("Area2D");
        activationArea.Scale = new Vector2(range, range);

        // Add hooks to activationArea for onEnter and onExit
        activationArea.BodyEntered += OnBodyEnteredActivationArea;
        activationArea.BodyExited += OnBodyExitedActivationArea;

        // set data to dataparameter
        data = new InteractableData(dataParameter);


        // get reference to dialogUI 
        dialogUI = GetNode<DialogUI>("/root/MovementTest/DialogLayer/DialogUI");
        if(dialogUI == null)
        {
            GD.Print("Could not find dialogUI");
        }
    }
    
    private void OnBodyEnteredActivationArea(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // set bool flag to true and make sprite invisible
            inRange = true;
            interactionIcon.Visible = true;
        }
    }

    private void OnBodyExitedActivationArea(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            // set inRange bool flag to false and make sprite invisible
            inRange = false;
            interactionIcon.Visible = false;
        }
    }

    // To be called via player object interaction
    public void activateInteractable()
    {
        if (!activated)
        {
            // Sets activated flag to true (allowing input)
            // and makes dialogUI visible.
            // immediately shows first dialog line
            GD.Print("Activated! Setting dialogUI to true and displaying next line!");

            activated = true;
            dialogUI.Visible = true;
            DisplayNextLine();
        }
    }
    

    // Input controls for dialogUI (only works when enabled == true)
    public override void _Input(InputEvent @event)
    {
        if (activated)
        {
            if (@event is InputEventKey inputKey)
            {
                if(inputKey.Pressed)
                {
                    if(inputKey.Keycode == Key.Space || inputKey.Keycode == Key.E)
                    {
                        if (dialogUI.isAnimating)
                        {
                            dialogUI.SkipTextAnimation();
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
        

        if(data != null)
        {

            if (data.CanContinue())
            {
                string currentLine = data.getCurrentLine();

                string[] lineParts = currentLine.Split("$$$");
                
                if (lineParts.Length > 1)
                {
                    // line has a speaker name tagged on
                    // Kyle $$$ Hi my name is kyle
                    dialogUI.SpeakLine(lineParts[0].Trim(), lineParts[1].Trim());
                }

                GD.Print("Calling DialogUI speak line: " + lineParts[0].Trim());
                dialogUI.SpeakLine(null, lineParts[0].Trim());
            }
            else
            {
                GD.Print("End of data, hiding dialogUI");
                dialogUI.Visible = false;
                activated = false;
                data.currentIndex = 0;
            }

        }
        else
        {
            GD.Print("No data entered in field.");
            return;
        }
    }

}
