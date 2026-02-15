using Godot;
using System;

public partial class ChoiceUI : Control
{

    VBoxContainer vBox;
    PackedScene buttonScene;

    [Signal]
    public delegate void SelectionMadeEventHandler(int choiceIdx);

    public override void _Ready()
    {
        vBox = GetNode<VBoxContainer>("VBoxContainer");
        // ChoiceButton.tscn
        buttonScene = GD.Load<PackedScene>("uid://cji1ck82vffii");
    }


    public void SetChoices(string[] choices)
    {
        // create buttons for each choice, which will send their index back to DialogUI.cs
        GD.Print("Creating" + choices.Length + "buttons.");
        for(int i = 0; i < choices.Length; i++)
        {
            Button choice = (Button)buttonScene.Instantiate();
            choice.Text = choices[i];
            vBox.AddChild(choice);

            int buttonIdx = i;
            choice.Pressed += () => ButtonPressed(buttonIdx);
            
        }
    }

    public void ClearChoices()
    {
        foreach(Node child in vBox.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void ButtonPressed(int index)
    {
        EmitSignal(SignalName.SelectionMade, index);
    }

}
