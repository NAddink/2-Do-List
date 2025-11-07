using Godot;
using GodotInk;
using System;

public partial class Example : Node2D
{

    [Export]
    private InkStory story;

    private static DialogUI dialogUI;

    public override void _Ready()
    {
        dialogUI = GetNode<DialogUI>("DialogLayer/DialogUI");

    }




    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputKey)
        {
            if(inputKey.Pressed)
            {
                if(inputKey.Keycode == Key.Space)
                {
                    if (dialogUI.isAnimating)
                    {
                        dialogUI.SkipTextAnimation();
                    }
                    else
                    {
                        if (story.CanContinue)
                        {
                            string[] line = story.Continue().Split("$$$");

                            if (line.Length > 1)
                            {
                                // line has a speaker name tagged on
                                // Kyle $$$ Hi my name is kyle
                                dialogUI.SpeakLine(line[0].Trim(), line[1].Trim());
                            }
                            
                            dialogUI.SpeakLine(null, line[0].Trim());

                        }
                        else
                        {
                            GD.Print("End of file");
                        }
                    }
                }
            }
        }
    }


   


}
