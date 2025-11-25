using System;
using Godot;

public class InteractableData
{

    public string data { get; set; }
    public string[] lines { get; set; }
    public int currentIndex { get; set; }


    public InteractableData(string data)
    {
        this.data = data;
        lines = data.Split("||");
        currentIndex = 0;
    }


    public bool CanContinue()
    {
        if(currentIndex >= lines.Length)
        {
            return false;
        }

        return true;
    }

    public string getCurrentLine()
    {
        // returns the current line and also increments the current index
        currentIndex++;
        return lines[currentIndex - 1];
    }


}