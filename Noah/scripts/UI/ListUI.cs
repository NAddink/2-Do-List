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

    [Signal]
    public delegate void ListCompleteEventHandler();


    public override void _Ready()
    {
        GameManager = GetTree().Root.GetNode<GameManager>("GameManager");
        if(GameManager == null)
        {
            GD.Print("GAME MANAGER IS NULL");
        }
        Theme = SelectedTheme;
        DisplayLabels();
        GameManager.FlagChanged += OnFlagChanged;

        ListComplete += GameManager.OnListComplete;

        
    }

    private void OnFlagChanged(string flagName, bool value)
    {
        CheckListCompleted();
    }


    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_list"))
        {

            Visible = !Visible;
            GameManager.SetDialogState(Visible);

            if (Visible)
            {
                DisplayLabels();
            }       
                
            
        }
    }


    public void DisplayLabels()
    {
        var list = GetNode<VBoxContainer>("VBoxContainer");

        // Clear existing items
        foreach (Node child in list.GetChildren())
            child.QueueFree();

        Array<string> items = new Array<string>();
        Array<string> visibleItems = new Array<string>();

        Random rand = new Random();

        // Populate
        if (DescriptionText.Length != 0)
        {
            foreach (string s in DescriptionText.Split("\n"))
            {
                if (!string.IsNullOrWhiteSpace(s))
                    items.Add(s);
            }
        }

        items.Shuffle();

        // Filter visible items
        foreach (string item in items)
        {
            string[] parts = item.Split("$$$");

            if (parts.Length < 2)
                continue;

            if (parts.Length == 2)
            {
                visibleItems.Add(item);
            }
            else // Gated
            {
                bool visible = true;

                for (int i = 1; i < parts.Length - 1; i++) // Check all items except first and last
                {
                    if (!GameManager.GetFlag(parts[i].Trim()))
                    {
                        visible = false;
                        break;
                    }
                }

                if (visible)
                    visibleItems.Add(item);
            }
        }

        if (visibleItems.Count == 0)
            return;

        // Create UI items
        foreach (string item in visibleItems)
        {
            string[] parts = item.Split("$$$");

            string itemName = parts[0].Trim();
            string finalFlag = parts.Last().Trim();

            // Wrapper VBoxContainer controls these which hold the labels
            Control wrapper = new Control();
            wrapper.CustomMinimumSize = new Vector2(0, 40); // row height 40

            // Label
            RichTextLabel label = new RichTextLabel();
            label.BbcodeEnabled = true;
            label.AutowrapMode = TextServer.AutowrapMode.Off;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.FitContent = true;
            label.ScrollActive = false;
            label.CustomMinimumSize = new Vector2(990, 30);

            // Style text
            if (GameManager.GetFlag(finalFlag))
            {
                label.Text =
                    "[s][color=DIM_GRAY]" +
                    itemName +
                    "[/color][/s]";
            }
            else
            {
                label.Text =
                    "[shake rate=2.0 level=15 connected=1]" +
                    "[wave amp=30.0 freq=1.0 connected=1]" +
                    itemName +
                    "[/wave][/shake]";
            }

            // Centering left-right
            label.AnchorLeft = 0.5f;
            label.AnchorRight = 0.5f;
            label.OffsetLeft = -label.CustomMinimumSize.X / 2;
            label.OffsetRight = label.CustomMinimumSize.X / 2;

            // Apply left-right jitter
            int randXOffset = rand.Next(-50, 51);
            label.Position += new Vector2(randXOffset, 0);

            // Create labels
            wrapper.AddChild(label);
            list.AddChild(wrapper);
        }
    }

    public void CheckListCompleted()
    {
        Array<string> items = new Array<string>();
        bool complete = true;

        // Populate
        if (DescriptionText.Length != 0)
        {
            foreach (string s in DescriptionText.Split("\n"))
            {
                if (!string.IsNullOrWhiteSpace(s))
                    items.Add(s);
            }
        }

        foreach(string item in items)
        {
            string[] parts = item.Split("$$$");
            if (!GameManager.GetFlag(parts.Last().Trim()))
            {
                complete = false;
                break;
            }
            
        }

        GD.Print("GAME COMPLETE?" + complete);
        if (complete)
        {
            EmitSignal(SignalName.ListComplete);
        }
    }

    public override void _ExitTree()
    {
        if (GameManager != null)
        {
            ListComplete -= GameManager.OnListComplete;
            GameManager.FlagChanged -= OnFlagChanged;
        }
    }

}
