using Godot;
using System;

public partial class FullScreenToggle : Node
{
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_fullscreen"))
        {
            Window window = GetWindow();

            if(window.Mode == Window.ModeEnum.Fullscreen)
            {
                window.Mode = Window.ModeEnum.Windowed;
            }
            else
            {
                window.Mode = Window.ModeEnum.Fullscreen;
            }

        }
    }

}
