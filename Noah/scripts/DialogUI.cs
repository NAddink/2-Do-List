using Godot;
using System;

public partial class DialogUI : Control
{


    [Export] public Label SpeakerName;
    public RichTextLabel DialogLine;
    public bool IsAnimating = false;



    public AudioStreamPlayer2D Audio;

    [Export]
    public int AudioSpeed = 3;

    [Export]
    public int AnimationSpeed = 30; // Animation speed (exported to view in godot editor)
    
    public int NoiseCounter = 0;

    public override void _Ready()
    {
        GameManager.Instance.DialogButtonPressed += DialogProceed;

        SpeakerName = GetNode<Label>("SpeakerBox/SpeakerName");
        DialogLine = GetNode<RichTextLabel>("DialogBox/DialogLine");


        // Get audio stream player for playing noises   
        Audio =  (AudioStreamPlayer2D)GetNode("Audio/AudioStreamPlayer2D");
    }



    public void SpeakLine(string line)
    {
        string[] lineParts = line.Split("$$$");
        string speaker = null;
        string dialog = null;

        if (lineParts.Length > 1)
        {
            // line has a speaker name tagged on
            // Kyle $$$ Hi my name is kyle
            speaker = lineParts[0].Trim();
            dialog  = lineParts[1].Trim();
        }
        else
        {
            // Line is just dialog no name
            dialog = lineParts[0].Trim();
        }

        // Has a name, set speaker text to the name
        if(speaker != null && speaker != "MC")
        {
            SpeakerName.Text = speaker;
            GetNode<Control>("SpeakerBox").Visible = true;
        }

        // For MC remove the speaker box
        if(speaker == "MC")
            GetNode<Control>("SpeakerBox").Visible = false;

        // Sets characters to all invisble and starts animation
        DialogLine.VisibleCharacters = 0;
        DialogLine.Text = dialog;
        IsAnimating = true;
    }

    public void SkipTextAnimation()
    {
        DialogLine.VisibleRatio = 1;
    }

    public void SetTypingNoise(string fileName, int speed)
    {
        Audio.Stream.TakeOverPath(fileName);
        AudioSpeed = speed;
    }

    public override void _Process(double delta)
    {
        // If the dialog ui is visible, set the dialog state to true in Game Manager and vice versa
        if(Visible)
            if(!GameManager.Instance.IsInDialog)
                GameManager.Instance.SetDialogState(true);
        if(!Visible)
            if(GameManager.Instance.IsInDialog)
                GameManager.Instance.SetDialogState(false);

        if (IsAnimating)
        {
            if (DialogLine.VisibleRatio < 1)
            {
                DialogLine.VisibleRatio += (float)((1.0 / DialogLine.Text.Length) * (AnimationSpeed * delta));

                if (NoiseCounter < AudioSpeed)
                {
                    NoiseCounter++;
                }
                else
                {
                    Audio.Play();
                    NoiseCounter = 0;
                }
                

            }
            else
            {
                IsAnimating = false;
            }
        }
    }

    private void DialogProceed()
    {
        if (Visible)
        {
            GameManager.Instance.SignalDialogProceed();
        }
    }
}
