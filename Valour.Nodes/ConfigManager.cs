using Valour.Nodes.Models;

namespace Valour.Nodes;

public static class ConfigManager {

    const string FOLDER = "ValourConfig";
    const string NODE_TEXT = "Nodes.data";
    const string NODE_PATH = FOLDER + "/" + NODE_TEXT;
    
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
    }

    /// <summary>
    /// Reads the stored node information and returns it as a list of nodes
    /// </summary>
    /// <returns>The list of saved node information</returns>
    public static Node[] ReadNodes()
    {
        // Ensure file exists. If not, create it.
        if (!File.Exists(NODE_PATH)){
            File.Create(NODE_PATH);
            Console.WriteLine("Nodes file not found. Creating...");
        }

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

}