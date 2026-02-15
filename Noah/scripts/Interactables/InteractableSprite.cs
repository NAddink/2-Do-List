using System;
using System.Threading.Tasks;
using Godot;
public partial class InteractableSprite : InteractableObject
{
    [Export]
    AnimatedSprite2D Sprite;
    Player Player;

    public override void _Ready()
    {
        base._Ready();
        Player = GetNode<Player>("%Player");
     }

    protected override async Task ActivateInternal()
    {
        RotateSprite();
        await activateInteractable();
    }


    private void RotateSprite()
    {
        // Difference between this pos and player
        Vector2 directionVector = (Position - Player.Position).Normalized();

        if(Mathf.Abs(directionVector.X) > Mathf.Abs(directionVector.Y)) // left or right
        {
            if(directionVector.X > 0)
            {
                // Rotate sprite right
            }
            else
            {
                // Rotate sprite left
            }
        }
        else // up or down
        {
            if(directionVector.Y > 0)
            {
                // Rotate sprite down
            }
            else
            {
                // Rotate sprite up
            }
        }


    }
    public void UnrotateSprite()
    {
        // restore saved sprite animation
    }


    public override void ExitDialog()
    {
        // End of dialog- cooldown timer
        InteractCooldown = CooldownTime;

        // End of dialog. 
        // hide dialog ui, set activated to false,
        // reset inkdata state
        // unpause player movement
        GD.Print("End of data, hiding dialogUI");
        DialogUI.Visible = false;
        DialogUI.DialogLine.VisibleRatio = 0;
        Activated = false;

        GameManager.Instance.SetDialogState(false); // set dialog state to false - frees movement

        UnrotateSprite();
    }
}
