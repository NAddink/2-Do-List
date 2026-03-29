using Godot;
using System;

public partial class CutsceneManager : Node
{

    private GameManager GameManager;

    public override void _Ready()
    {
        GameManager = GetTree().Root.GetNode<GameManager>("GameManager");
        GameManager.LevelComplete += LevelComplete;
    }

    private void LevelComplete()
    {
        DemoCutscene();
    }

    private void DemoCutscene()
    {
        CanvasLayer CanvasLayer = (CanvasLayer)GetTree().Root.FindChild("UI", true, false);
        Control DialogUI = (Control)GetTree().Root.FindChild("DialogUI", true, false);
        ColorRect blackPanel = new ColorRect();
        blackPanel.Color = Colors.Black; 
        
        blackPanel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        blackPanel.ZIndex = -1;


        CanvasLayer.AddChild(blackPanel);


        DialogUI.Visible = true;
        GameManager.SetDialogState(true);
    }



}
