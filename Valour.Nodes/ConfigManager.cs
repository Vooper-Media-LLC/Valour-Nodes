using System.Text.Json;
using Valour.Nodes.Models;

namespace Valour.Nodes;

public static class ConfigManager {

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
        if (!Directory.Exists(FOLDER)){
            Directory.CreateDirectory(FOLDER);
            Console.WriteLine("Configuration folder not found. Creating...");
        }

        // Read the main config file
        ReadConfig();
    }

    public static void EnsureFileExists(string path){
        // Ensure file exists. If not, create it.
        if (!File.Exists(path)){
            File.Create(path);
            Console.WriteLine($"{path} not found. Creating...");
        }
    }

    public static void ReadConfig()
    {
        // Ensure file exists. If not, create it.
        if (!File.Exists(CONFIG_PATH)) {
            Config = new Config();
            Config.API_Key = Guid.NewGuid().ToString();
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
    public static Node[] ReadNodes()
    {
        // Ensure file exists. If not, create it.
        EnsureFileExists(NODE_PATH);

        // Read all lines and prepare an array for the nodes
        var lines = File.ReadAllLines(FOLDER + "/" + NODE_TEXT);
        Node[] nodes = new Node[lines.Length];

        // Create node instances using names
        int i = 0;
        foreach (string line in lines){
            nodes[i] = new Node(line);
            i++;
        }

        // Return nodes
        return nodes;
    }

    public static async Task WriteNode(Node node){
        EnsureFileExists(NODE_PATH);
        await File.WriteAllLinesAsync(NODE_PATH, new string[] { node.Name });
        Console.WriteLine($"Added '{node.Name}' to {NODE_TEXT}");
    }

}