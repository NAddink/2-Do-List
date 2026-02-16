using System;
using System.Threading.Tasks;
using Godot;

[Tool]
public partial class InteractableSprite : InteractableObject
{
    [Export]
    AnimatedSprite2D Sprite;
    Player Player;


    String CurrentAnimation;

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

        // Save current animation for unrotate
        CurrentAnimation = Sprite.Animation;


        if(Mathf.Abs(directionVector.X) > Mathf.Abs(directionVector.Y)) // left or right
        {
            if(directionVector.X > 0)
            {
                // Rotate sprite left
                Sprite.Animation = "idle-left";
            }
            else
            {
                // Rotate sprite right
                Sprite.Animation = "idle-right";
            }
        }
        else // up or down
        {
            if(directionVector.Y > 0)
            {
                // Rotate sprite up
                Sprite.Animation = "idle-up";
            }
            else
            {
                // Rotate sprite down
                Sprite.Animation = "idle-down";
            }
        }


    }
    public void UnrotateSprite()
    {
        // restore saved sprite animation
        Sprite.Animation = CurrentAnimation;
    }

    public void SetSpriteAnimation(string animationName)
    {
        Sprite.Animation = animationName;
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
