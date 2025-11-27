using Godot;
using System;

public partial class GameManager : Node
{

    public static GameManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }


    [Signal]
    public delegate void ToggleDialogEventHandler(bool isPaused);

    [Signal]
    public delegate void DialogProceedEventHandler();

    [Signal]
    public delegate void DialogButtonPressedEventHandler();

    public bool IsInDialog { get; private set; }

    public void SetDialogState(bool paused)
    {
        IsInDialog = paused;
        EmitSignal(SignalName.ToggleDialog, IsInDialog);
    }

    public void SignalDialogProceed()
    {
        EmitSignal(SignalName.DialogProceed);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputKey)
        {
            if(inputKey.Pressed)
            {
                if(inputKey.Keycode == Key.Space || inputKey.Keycode == Key.E)
                {
                    EmitSignal(SignalName.DialogButtonPressed);
                }
            }
        }
    }

}
