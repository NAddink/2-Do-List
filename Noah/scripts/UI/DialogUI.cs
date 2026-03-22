using Godot;
using Godot.Collections;
using System;

public partial class DialogUI : Control
{


    public AnimatedSprite2D PortraitSprite;
    public RichTextLabel DialogLine;
    public bool IsAnimating = false;



    public AudioStreamPlayer2D Audio;

    [Export]
    public int AudioSpeed = 3;

    [Export]
    public int AnimationSpeed = 30; // Animation speed (exported to view in godot editor)
    
    public int NoiseCounter = 0;

    private GameManager GameManager;

    private Dictionary<string, SpriteFrames> Portraits = new Dictionary<string, SpriteFrames>();


    public override void _Ready()
    {
        GameManager = GetTree().Root.GetNode<GameManager>("GameManager");
        GameManager.DialogButtonPressed += DialogProceed;

        PortraitSprite = GetNode<AnimatedSprite2D>("PortraitBox/Portrait");
        DialogLine = GetNode<RichTextLabel>("DialogBox/DialogLine");


        // Get audio stream player for playing noises   
        Audio =  (AudioStreamPlayer2D)GetNode("Audio/AudioStreamPlayer2D");

        // Set spritesheets for portraits
        Portraits["Sam"] = GD.Load<SpriteFrames>("uid://7eapc6hmjvnp");
        Portraits["Janice"] = GD.Load<SpriteFrames>("uid://dhu47qrousdeq");
        Portraits["Boss"] = GD.Load<SpriteFrames>("uid://dn3kme42byuqd");
        Portraits["MC"] = GD.Load<SpriteFrames>("uid://ba0hatkdit40r");
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

        if (Portraits.ContainsKey(speaker))
        {
            PortraitSprite.SpriteFrames = Portraits[speaker];
            PortraitSprite.Play();
            GetNode<Control>("PortraitBox").Visible = true;
        }
        else
        {
            GetNode<Control>("PortraitBox").Visible = false;
        }

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
            GameManager.SignalDialogProceed();
        }
    }
}
