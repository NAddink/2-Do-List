using Godot;
using System;

public partial class DialogUI : Control
{


    [Export] public Label speakerName;
    public RichTextLabel dialogLine;
    public bool isAnimating = false;



    public AudioStreamPlayer2D audio;

    [Export]
    public int audioSpeed = 3;

    [Export]
    public int ANIMATION_SPEED = 30; // Animation speed (exported to view in godot editor)
    
    public int noiseCounter = 0;

    public override void _Ready()
    {
        speakerName = GetNode<Label>("SpeakerBox/SpeakerName");
        dialogLine = GetNode<RichTextLabel>("DialogBox/DialogLine");


        // Get audio stream player for playing noises   
        audio =  (AudioStreamPlayer2D)GetNode("Audio/AudioStreamPlayer2D");
    }

    public void SpeakLine(string speaker, string dialog)
    {
        if(speaker != null && speaker != "MC")
        {
            speakerName.Text = speaker;
            GetNode<Control>("SpeakerBox").Visible = true;
        }

        if(speaker == "MC")
            GetNode<Control>("SpeakerBox").Visible = false;

        // Sets characters to all invisble and starts animation
        dialogLine.VisibleCharacters = 0;
        dialogLine.Text = dialog;
        isAnimating = true;
    }

    public void SkipTextAnimation()
    {
        dialogLine.VisibleRatio = 1;
    }

    public void SetTypingNoise(string fileName, int speed)
    {
        audio.Stream.TakeOverPath(fileName);
        audioSpeed = speed;
    }

    public override void _Process(double delta)
    {
        if (isAnimating)
        {
            if (dialogLine.VisibleRatio < 1)
            {
                dialogLine.VisibleRatio += (float)((1.0 / dialogLine.Text.Length) * (ANIMATION_SPEED * delta));

                if (noiseCounter < audioSpeed)
                {
                    noiseCounter++;
                }
                else
                {
                    audio.Play();
                    noiseCounter = 0;
                }
                

            }
            else
            {
                isAnimating = false;
            }
        }
    }
}
