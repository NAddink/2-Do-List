using Godot;
using GodotInk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Tool]
public partial class DescriptionObject : InteractableObject
{
    [Export(PropertyHint.MultilineText)]
    public string DescriptionText;

    protected override async Task ActivateInternal()
    {
        if (!Activated && InteractCooldown <= 0)
        {
            Activated = true;
            DialogUI.Visible = true;

            GameManager.SetDialogState(true);

            DialogUI.SpeakLine("MC $$$ " + DescriptionText);
        }
    }

    protected override async void DialogProceed()
    {
        if (!Activated) return;

        if (DialogUI.IsAnimating)
        {
            DialogUI.SkipTextAnimation();
        }
        else
        {
            ExitDialog();
        }
    }




}
