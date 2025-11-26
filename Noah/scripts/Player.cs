using Godot;
using System;
using System.Collections;

public partial class Player : CharacterBody2D
{
    [Export] int MaxSpeed = 100;
    Vector2 LastDirection = new Vector2(1,0);



    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = direction * MaxSpeed;
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
        Godot.Collections.Array<InteractableObject> interactablesInRange = [];
        float closestDistance = float.MaxValue;
        InteractableObject closestInteractable = null;

        

        foreach(Node node in GetTree().GetNodesInGroup("interactable"))
        {
            // make sure node is interactableObject- it should be
            // adds any in range interactables to the list
            if (node is InteractableObject interactable) 
                if(interactable.InRange)
                    interactablesInRange.Add(interactable);
        }

        // GD.Print($"{interactablesInRange.Count} Total interactables in range.");

        // find closest in-range interactable 
        foreach(InteractableObject interactable in interactablesInRange)
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
                closestInteractable.activateInteractable();
            }
        }
        else
        {
            // GD.Print("Closest interactable is null.");
        }

    }
}
