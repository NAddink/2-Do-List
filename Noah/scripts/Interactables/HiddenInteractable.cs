using Godot;
using GodotInk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Tool]
public partial class HiddenInteractable : InteractableObject
{
    [Export]
    public string RequiredFlag = "";

    private bool IsUnlocked => 
        string.IsNullOrEmpty(RequiredFlag) || GameManager.GetFlag(RequiredFlag);

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
            return; // Don't do any logic in editor on ready

        base._Ready();

        GameManager.FlagChanged += OnFlagChanged;

        UpdateVisibility();
    }

    private void OnFlagChanged(string name, bool value)
    {
        if (name == RequiredFlag)
            UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (Engine.IsEditorHint())
            return;

        bool visible = IsUnlocked;

        // Whole object visibility
        Visible = visible;

        // Disable interaction if locked
        SetProcess(visible);
        SetPhysicsProcess(visible);

        // Optional: disable collision areas
        var activation = GetNodeOrNull<Area2D>("ActivationArea");
        var visibleArea = GetNodeOrNull<Area2D>("VisibleArea");

        if (activation != null)
            activation.Monitoring = visible;

        if (visibleArea != null)
            visibleArea.Monitoring = visible;

        


    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
            return; // Don't do any logic in editor on ready
        
        if (!IsUnlocked)
            return;

        base._PhysicsProcess(delta);
    }

    public override void Activate()
    {
        if (!IsUnlocked)
            return;

        base.Activate();
    }




}
