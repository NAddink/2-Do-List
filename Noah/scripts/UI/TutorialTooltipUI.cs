using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class TutorialTooltipUI : Control
{

    private bool AllTutorialsCompleted = false;


    // List of input buttons to be pressed during tutorial
    private Dictionary<string, bool> InputTutorials;
    private Dictionary<string, Sprite2D> InputSprites;

    [Export]
    Sprite2D RightArrow, LeftArrow, UpArrow, DownArrow, Interact, TodoList;


    public override void _Ready()
    {
        InputTutorials = new Dictionary<string, bool>
        {
            { "toggle_list", false },
            { "move_up", false },
            { "move_down", false },
            { "move_left", false },
            { "move_right", false }, 
            { "interact", false }
        };

        InputSprites = new Dictionary<string, Sprite2D>
        {
            { "toggle_list", TodoList },
            { "move_up", UpArrow },
            { "move_down", DownArrow },
            { "move_left", LeftArrow },
            { "move_right", RightArrow }, 
            { "interact", Interact }
        };
        
    }

    public override void _PhysicsProcess(double delta)
    {
        if (AllTutorialsCompleted) return;

        if (!InputTutorials.ContainsValue(false)) // Check if all are completed
        {
            AllTutorialsCompleted = true;
            
            _ = HideTooltip();
        } 

        var keys = new List<string>(InputTutorials.Keys);

        foreach (var action in keys)
        {
            if (!InputTutorials[action] && Input.IsActionPressed(action))
            {
                InputTutorials[action] = true;
                GD.Print("Action " + action + " completed for first time!");
                // Hide specific label for this tutorial item
                var sprite = InputSprites[action];
                var atlas = sprite.Texture as AtlasTexture;

                if (atlas != null)
                {
                    atlas = (AtlasTexture)atlas.Duplicate();
                    sprite.Texture = atlas;

                    var region = atlas.Region;
                    region.Position += new Vector2(64, 0);
                    atlas.Region = region;
                }
            }
        }

    }

    private async Task HideTooltip()
    {
        var tween = CreateTween();

        tween.TweenProperty(this, "position", Position + new Vector2(0, -400), 0.5f);

        await ToSignal(tween, Tween.SignalName.Finished);

        Visible = false;
        QueueFree();
    }


}