using System.Text.Json;
using Valour.Nodes.Models;

namespace Valour.Nodes;

public static class ConfigManager
{

    const string FOLDER = "ValourConfig";
    const string NODE_TEXT = "Nodes.data";
    const string NODE_PATH = FOLDER + "/" + NODE_TEXT;
    const string CONFIG_TEXT = "Config.json";
    const string CONFIG_PATH = FOLDER + "/" + CONFIG_TEXT;

    /// <summary>
    /// The current config
    /// </summary>
    public static Config Config { get; set; }

    /// <summary>
    /// Initializes the configuration manager
    /// </summary>
    static ConfigManager()
    {
        // Ensure config location exists
        if (!Directory.Exists(FOLDER))
        {
            Directory.CreateDirectory(FOLDER);
            Console.WriteLine("Configuration folder not found. Creating...");
        }

        // Read the main config file
        ReadConfig();
    }

    public static void EnsureFileExists(string path)
    {
        // Ensure file exists. If not, create it.
        if (!File.Exists(path))
        {
            File.Create(path);
            Console.WriteLine($"{path} not found. Creating...");
        }
    }

    public static void ReadConfig()
    {
        // Ensure file exists. If not, create it.
        if (!File.Exists(CONFIG_PATH))
        {
            Config = new Config();
            Config.ApiKey = Guid.NewGuid().ToString();
            File.WriteAllText(CONFIG_PATH, JsonSerializer.Serialize(Config));
            Console.WriteLine("No config found. Generating...");
        }
        else
        {
            // Read json
            Config = JsonSerializer.Deserialize<Config>(File.OpenRead(CONFIG_PATH));
        }
    }

    /// <summary>
    /// Reads the stored node information and returns it as a list of nodes
    /// </summary>
    /// <returns>The list of saved node information</returns>
    public static List<Node> ReadNodes()
    {
        // Ensure file exists. If not, create it.
        EnsureFileExists(NODE_PATH);

        if (new FileInfo(FOLDER + "/" + NODE_TEXT).Length < 1)
            return new List<Node>();
            
        var nodes = JsonSerializer.Deserialize<List<Node>>(File.ReadAllText(FOLDER + "/" + NODE_TEXT));

        // Return nodes
        return nodes;
    }

    public static async Task WriteNodes(List<Node> nodes)
    {
        EnsureFileExists(NODE_PATH);
        string json = JsonSerializer.Serialize(nodes);
        await File.WriteAllTextAsync(NODE_PATH, json);
        Console.WriteLine($"Wrote node data to {NODE_TEXT}");
    }

}