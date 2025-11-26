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
    public delegate void MovementPauseToggledEventHandler(bool isPaused);

    public bool MovementPaused { get; private set; }

    public void ToggleMovementPause()
    {
        MovementPaused = !MovementPaused;
        EmitSignal(SignalName.MovementPauseToggled, MovementPaused);
    }

    public void SetMovementPause(bool paused)
    {
        MovementPaused = paused;
        EmitSignal(SignalName.MovementPauseToggled, MovementPaused);
    }

}
