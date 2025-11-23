using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] int max_speed = 100;
    Vector2 last_direction = new Vector2(1,0);



    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = direction * max_speed;
        MoveAndSlide();

        if(direction.Length() > 0)
        {
            last_direction = direction;
            PlayWalkAnimation(direction);
        }
        else
        {
            PlayIdleAnimation(last_direction);
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
}
