using Godot;
using Godot.Collections;
using System.Text.Json;

public partial class SaveManager : Node2D
{


    public void SaveNodeData()
    {
        using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);

        var saveNodes = GetTree().GetNodesInGroup("Persist");
        foreach (Node saveNode in saveNodes)
        {
            // Check the node is an instanced scene so it can be instanced again during load.
            if (string.IsNullOrEmpty(saveNode.SceneFilePath))
            {
                GD.Print($"persistent node '{saveNode.Name}' is not an instanced scene, skipped");
                continue;
            }

            // Check the node has a save function.
            if (!saveNode.HasMethod("Save"))
            {
                GD.Print($"persistent node '{saveNode.Name}' is missing a Save() function, skipped");
                continue;
            }

            // Call the node's save function.
            var nodeData = saveNode.Call("Save");

            // Json provides a static method to serialized JSON string.
            var jsonString = Json.Stringify(nodeData);

            // Store the save dictionary as a new line in the save file.
            saveFile.StoreLine(jsonString);
        }
    }

    public void SaveChoices(string SavePath, Dictionary<string, bool> FlagsData)
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

    public Dictionary<string, bool> LoadChoices(string SavePath, Dictionary<string, bool> FlagsData)
    {
        Dictionary<string, bool> Data = new Dictionary<string, bool>();

        if (!FileAccess.FileExists(SavePath))
        {
            GD.PrintErr("Save file not found!");
            FlagsData = new Dictionary<string, bool>();
            return Data;
        }

        using FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        string jsonText = file.GetAsText();
        file.Close();

        if (!string.IsNullOrEmpty(jsonText))
        {
            Data = JsonSerializer.Deserialize<Dictionary<string,bool>>(jsonText); 
            return Data;
        }
        
        GD.PrintErr("Attempted to load data from json but there was no data to load!");
        return Data;
        
    }
}