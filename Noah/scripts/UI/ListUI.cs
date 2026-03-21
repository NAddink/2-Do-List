using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ListUI : Control
{
    
    [Export(PropertyHint.MultilineText)]
    public string DescriptionText;

    [Export]
    public Theme SelectedTheme { get; set; }

    private GameManager GameManager;


    public override void _Ready()
    {
        GameManager = GetTree().Root.GetNode<GameManager>("GameManager");
        if(GameManager == null)
        {
            while (true)
            {
                GD.Print("GAME MANAGER IS NULL");
                
            }
        }
        Theme = SelectedTheme;
        DisplayLabels();

        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputKey)
        {
            if(inputKey.Pressed)
            {
                if(inputKey.Keycode == Key.T)
                {
                    if (Visible)
                    {
                        Visible = false;
                        GameManager.SetDialogState(false);
                    }
                    else if (!Visible)
                    {
                        Visible = true;
                        GameManager.SetDialogState(true);
                        DisplayLabels();
                    }
                    
                }
            }
        }
    }


    public void DisplayLabels()
    {
        // Clear existing items
        var children = GetNode("ListItems").GetChildren();
        foreach(Node child in children)
        {
            child.QueueFree();
        }

        
        // Get list items shuffled
        Array<string> items = new Array<string>();
        

        if(DescriptionText.Length != 0)
        {
            foreach(string s in DescriptionText.Split("\n"))
                items.Add(s);
        }

        items.Shuffle();

        if (items.Count == 0) return;

        int itemCnt = items.Count;

        int maxY = 480;
        int gap = (itemCnt > 1) ? maxY / (itemCnt - 1) : 0;

        int yOffset = 0;
        

        Random rand = new Random();

        foreach (string item in items)
        {
            RichTextLabel label = new RichTextLabel();
            label.BbcodeEnabled = true;
            label.AutowrapMode = TextServer.AutowrapMode.Off;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.FitContent = true;
            label.ScrollActive = false;

            // labels will match the width of the parent node to center properly
            label.CustomMinimumSize = new Vector2(990, 30); 

            string[] parts = item.Split("$$$");
            if(parts.Length < 2)
            {
                GD.PrintErr("Todo list item " + parts[0] + " is missing an assigned flag.");
                return;
            }

            string itemName = parts[0].Trim();
            string itemFlag = parts[1].Trim();

            // If flag is set to true - checked
            if (GameManager.GetFlag(itemFlag))
            {
                label.Text =
                    "[s][color=DIM_GRAY]" +
                    itemName +
                    "[/color][/s]";

            }
            else // unchecked
            {
                label.Text =
                    "[shake rate=2.0 level=15 connected=1]" +
                    "[wave amp=30.0 freq=1.0 connected=1]" +
                    itemName +
                    "[/wave][/shake]";
            }

            // Center X around 300, shift randomly left/right
            int randXOffset = rand.Next(-50, 51);


            label.Position = new Vector2(randXOffset, yOffset);

            yOffset += gap;

            GetNode<Control>("ListItems").AddChild(label);
        }
    
    }
}
