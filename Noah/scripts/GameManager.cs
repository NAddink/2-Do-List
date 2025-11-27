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

}
