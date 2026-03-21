using Godot;
using System;
using Godot.Collections;
using System.Text.Json;

public partial class GameManager : Node2D
{

    public SaveManager SaveManager;

    public override void _Ready()
    {
        // Get SaveManager
        SaveManager = GetTree().Root.GetNode<SaveManager>("SaveManager");;

        LoadChoices();
        EnsureDefaultFlags();
        SaveChoices();
    }


    [Signal]
    public delegate void ToggleDialogEventHandler(bool isPaused);

    [Signal]
    public delegate void DialogProceedEventHandler();

    [Signal]
    public delegate void DialogButtonPressedEventHandler();

    public bool IsInDialog { get; private set; }

    private Dictionary<string, bool> FlagsData = new Dictionary<string, bool>();
    

    private const string SavePath = "user://savegame.json";

    public void SetDialogState(bool paused)
    {
        IsInDialog = paused;
        EmitSignal(SignalName.ToggleDialog, IsInDialog);
    }

    public void SignalDialogProceed()
    {
        EmitSignal(SignalName.DialogProceed);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputKey)
        {
            if(inputKey.Pressed)
            {
                if(inputKey.Keycode == Key.Space || inputKey.Keycode == Key.E)
                {
                    EmitSignal(SignalName.DialogButtonPressed);
                }
            }
        }
    }


    // Saves flags to disk
    public void SaveChoices()
    {
        SaveManager.SaveChoices(SavePath, FlagsData);
    }

    // Loads flags from disk
    public void LoadChoices()
    {
        FlagsData = SaveManager.LoadChoices(SavePath, FlagsData);
    }

    
    public void AddFlag(string flagName, bool flagValue)
    {
        // flag already exists, set data
        if (FlagsData.ContainsKey(flagName))
        {
            FlagsData[flagName] = flagValue;
        }
        else // flag does not already exist
        {
            FlagsData.Add(flagName, flagValue);
        }

        SaveChoices();
    }

    public bool GetFlag(string flagName)
    {
        if (FlagsData.ContainsKey(flagName))
        {
            return FlagsData[flagName]; // Return flag as true or false
        }
        else
        {
            return false;
        }
    }

    // Sets all flags to false
    public void ResetAllFlags()
    {
        foreach(System.Collections.Generic.KeyValuePair<string, bool> flag in FlagsData)
        {
            FlagsData[flag.Key] = false;
        }

        SaveChoices();
    }

    private void EnsureDefaultFlags()
    {
        var flags = GetFlagsFromGlobals();
        if (flags == null)
        {
            GD.Print("Globals.txt had no flags!");
            return;
        }

        foreach (var flag in flags)
        {
            if (!FlagsData.ContainsKey(flag))
                FlagsData[flag] = false;
        }
        
    }

    // Retrieves all flags from globals.txt and sets flagdata to contain all those flags as null
    // Helps with consistency as otherwise
    private string[] GetFlagsFromGlobals()
    {
        string filePath = "res://Noah/inks/globals.txt";
        
        if (!FileAccess.FileExists(filePath))
        {
            GD.PrintErr("Globals.ink not found!");
            FlagsData = new Dictionary<string, bool>();
            return null;
        }

        using FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string text = file.GetAsText();
        file.Close();
        
        if (string.IsNullOrEmpty(text))
        {
            GD.PrintErr("globals.txt is empty!");
            return null;
        }


        // get string list of flags from globals.ink
        System.Collections.Generic.List<string> flags = new System.Collections.Generic.List<string>();

        foreach(string line in text.Split("\n"))
        {
            string trimmed = line.Trim();

            // Skip blank lines
            if (string.IsNullOrEmpty(trimmed))
                continue;

            // Skip lines that don't start with VAR
            if (!trimmed.StartsWith("VAR "))
                continue;

            string[] parts = trimmed.Split("=");
            if (parts.Length < 2) // Lines should have =
                continue;

            string flagName = parts[0].Replace("VAR", "").Trim(); 

            if (!string.IsNullOrEmpty(flagName))
                flags.Add(flagName);            
        }

        return flags.ToArray();


    }

    public Dictionary<string, bool> GetFlags()
    {
        return FlagsData;
    }

   
}
