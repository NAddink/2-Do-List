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
    }


    [Signal]
    public delegate void ToggleDialogEventHandler(bool isPaused);

    [Signal]
    public delegate void DialogProceedEventHandler();

    [Signal]
    public delegate void DialogButtonPressedEventHandler();

    public bool IsInDialog { get; private set; }

    private Dictionary<string, bool> FlagsData;
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

                if(inputKey.Keycode == Key.P)
                {
                    GetTree().ChangeSceneToFile("res://Noah/scenes/DialogUI.tscn");
                }
            }
        }
    }

    // Run on start
    public override void _EnterTree()
    {
        FlagsData = new Dictionary<string, bool>();
        Load();

        FlagsData.Add("ExampleTrueVar", true);
        FlagsData.Add("ExampleFalseVar", false);
        Save();
    }


    public void Save()
    {
        ConfigFile configFile = new ConfigFile();
        var jsonData = JsonSerializer.Serialize(FlagsData);

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
        
        FlagsData = JsonSerializer.Deserialize<Dictionary<string,bool>>(jsonText);
        

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

    public Dictionary<string, bool> GetFlags()
    {
        return FlagsData;
    }

}
