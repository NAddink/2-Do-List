using Godot;
using System;
using Godot.Collections;
using System.Text.Json;

public partial class GameManager : Node
{

    public static GameManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        Load();
        EnsureDefaultFlags();
        Save();
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

    // Run on start
    // public override void _Ready()
    // {
        
    // }


    public void Save()
    {
        ConfigFile configFile = new ConfigFile();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var jsonData = JsonSerializer.Serialize(FlagsData, options);

        using FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file != null)
        {
            file.StoreString(jsonData);
            file.Close();
            GD.Print("Successfully saved game data as json");
        }
        else
        {
            GD.PrintErr("Error occured when attempting to save to file");
        }
    }

    public void Load()
    {
        if (!FileAccess.FileExists(SavePath))
        {
            GD.PrintErr("Save file not found!");
            FlagsData = new Dictionary<string, bool>();
            return;
        }

        using FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        string jsonText = file.GetAsText();
        file.Close();

        if (!string.IsNullOrEmpty(jsonText))
        {
            FlagsData = JsonSerializer.Deserialize<Dictionary<string,bool>>(jsonText); 
        }
        else
        {
            GD.PrintErr("Attempted to load data from json but there was no data to load!");
        }
        
        

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

        Save();
    }

    public bool GetFlag(string flagName)
    {
        Load(); // Get updated flagdata

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

        Save();
    }

    public Dictionary<string, bool> GetFlags()
    {
        return FlagsData;
    }

    private void EnsureDefaultFlags()
    {
        var flags = GetFlagsFromGlobals();
        if (flags == null)
        {
            GD.Print("Globals.ink had no flags!");
            return;
        }

        foreach (var flag in flags)
        {
            if (!FlagsData.ContainsKey(flag))
                FlagsData[flag] = false;
        }
        
    }

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

        if (!string.IsNullOrEmpty(text))
        {
            // get string list of flags from globals.ink
            string[] lines = text.Split("\n");

            for(int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("VAR","").Replace("= false","").Trim();
            }

            return lines;

        }
        else
        {
            GD.PrintErr("Attempted to load data from globals.ink but there was no data to load!");
            return null;
        }

    }

   
}
