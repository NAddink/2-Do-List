using Godot;
using GodotInk;
using Ink.Parsed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public partial class InteractableObject : Node2D
{
    [Export] float range = 1;
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
        GetNode<Sprite2D>("InteractIcon").Visible = false;

        // get activation area and scale based on range var
        Area2D activationArea = GetNode<Area2D>("Area2D");
        activationArea.Scale = new Vector2(range, range);

        // Add hooks to activationArea for onEnter and onExit
        activationArea.BodyEntered += OnBodyEnteredActivationArea;
        activationArea.BodyExited += OnBodyExitedActivationArea;

        // get reference to dialogUI 
        DialogUI = GetNode<DialogUI>("/root/MovementTest/UILayer/DialogUI");
        ChoiceUI = GetNode<ChoiceUI>("/root/MovementTest/UILayer/ChoiceUI");
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
        

        if(_story != null)
        {

            if (_story.CurrentChoices.Count > 0)
            {
                // ChoiceUI is already open, return
                if(ChoiceUI.Visible) return;

                // Choice
                // TODO: Choice UI will be called here, along with choice logic
                GD.Print("Choice!");

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
                await DisplayNextLine();

                
                
            }
            else if (_story.CanContinue)
            {
                string currentLine = _story.Continue();
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
                DialogUI.DialogLine.VisibleRatio = 0;
                Activated = false;

                _story = null; // reset story
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

    public void OnInkVariableChanged(string name, bool value)
    {
        GameManager.Instance.AddFlag(name, value);
    }


}
