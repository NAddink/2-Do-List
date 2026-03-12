using Godot;
using System;
using System.Collections;

public partial class Player : CharacterBody2D
{
    [Export] int MaxSpeed = 100;
    [Export] int SprintSpeed; // speed when holding shift

    [Export] float AnimationSpeed;
    [Export] float SprintAnimationSpeed;

    Vector2 LastDirection = new Vector2(1,0);

    private bool isPaused = false;
    private GameManager GameManager;

    public override void _Ready()
    {
        GameManager = GetTree().CurrentScene.GetNode<GameManager>("GameManager");
        GameManager.ToggleDialog += OnDialogEnter;
    }

    // Called when player enters dialog state
    private void OnDialogEnter(bool IsInDialog)
    {
        isPaused = IsInDialog;

        // Get current animation and set it to idle version.

        GD.Print("CURRENT ANIM:" + GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation);

        if(GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "walk_left")
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle_left");

        if(GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "walk_right")
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle_right");

        if(GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "walk_up")
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle_up");

        if(GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "walk_down")
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle_down");
    }


    public override void _PhysicsProcess(double delta)
    {
        if(isPaused)
            return;

        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        
        int currentSpeed = MaxSpeed;
        bool isSprinting = Input.IsActionPressed("sprint");
        if (isSprinting)
            currentSpeed = SprintSpeed;

        // Speed up animation while sprinting
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        sprite.SpeedScale = isSprinting ? SprintAnimationSpeed : AnimationSpeed;
        
        Velocity = direction * currentSpeed;
        MoveAndSlide();

        

        if(direction.Length() > 0)
        {
            LastDirection = direction;
            PlayWalkAnimation(direction);
        }
        else
        {
            PlayIdleAnimation(LastDirection);
        }

        if (Input.IsActionJustPressed("interact"))
        {
            if (GameManager.IsInDialog)
            {
                GD.Print("In dialog when E was pressed");
            }
            ActivateClosestInteractable();
        }
    }

    public void PlayWalkAnimation(Vector2 direction)
    {
        if(direction.X > 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("walk_right");
        }
        else if(direction.X < 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("walk_left");
        }
        else if(direction.Y > 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("walk_down");
        }
        else if(direction.Y < 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("walk_up");
        }
    }

    public void PlayIdleAnimation(Vector2 direction)
    {
        if(direction.X > 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle_right");
        }
        else if(direction.X < 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle_left");
        }
        else if(direction.Y > 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle_down");
        }
        else if(direction.Y < 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle_up");
        }
    }

    // Activates the closest visible interactable
    private void ActivateClosestInteractable()
    {
        Godot.Collections.Array<ActivatableObject> interactablesInRange = [];
        float closestDistance = float.MaxValue;
        ActivatableObject closestInteractable = null;

        

        foreach(Node node in GetTree().GetNodesInGroup("interactable"))
        {
            // make sure node is ActivatableObject- it should be
            // adds any in range interactables to the list
            if (node is ActivatableObject interactable) 
                if(interactable.InRange)
                    interactablesInRange.Add(interactable);
        }

        // GD.Print($"{interactablesInRange.Count} Total interactables in range.");

        // find closest in-range interactable 
        foreach(ActivatableObject interactable in interactablesInRange)
        {
            float distance = interactable.GlobalPosition.DistanceTo(this.GlobalPosition);
            if( distance < closestDistance)
            {
                closestInteractable = interactable;
                closestDistance = distance;
            }
        }

        if(closestInteractable != null)
        {
            // GD.Print($"Closest interactable is {closestDistance} away. Activating.");
            if (!closestInteractable.Activated)
            {
                closestInteractable.Activate();
            }
        }
        else
        {
            // GD.Print("Closest interactable is null.");
        }

    }

    public Godot.Collections.Dictionary<string, Variant> Save()
    {
        return new Godot.Collections.Dictionary<string, Variant>()
        {
            { "PosX", Position.X }, // Vector2 is not supported by JSON
            { "PosY", Position.Y },
        };
    }
    
}
