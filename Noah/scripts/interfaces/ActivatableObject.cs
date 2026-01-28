using Godot;
using System;

public abstract partial class ActivatableObject : Node2D
{
    public bool InRange { get; set; }
    public bool Activated { get; set; }

    public abstract void Activate();

}
