using Valour.Nodes.Models;

namespace Valour.Nodes;

/// <summary>
/// Handles input for the node controller
/// </summary>
public static class InputManager
{
    /// <summary>
    /// Runs the input handling logic
    /// </summary>
    public static async Task Run(){
        while (true){
            // Reads incoming command
            string command = Console.ReadLine().ToLower();
            // Splits into words
            var words = command.Split(' ');
            // Creates args array
            var args = words.Skip(1).ToArray();
            // Fires off command handler
            await RunCommand(words[0], args);
        }
    }

    public static readonly Dictionary<string, (Func<string[], Task> task, string description)> Commands = new(){
        { "help", (Help, "Shows this dialogue. Usage: help") },
        { "addnode", (AddNode, "Adds a node to the node list. Usage: addnode <name>") },
        { "nodes", (Nodes, "Shows the node list. Usage: nodes") }
    };

    /// <summary>
    /// Runs incoming commands
    /// </summary>
    public static async Task RunCommand(string command, string[] args){
        if (Commands.ContainsKey(command)){
            await Commands[command].task.Invoke(args);
        }
        else
        {
            Console.WriteLine($"Command '{command}' was not found. Try 'help'.");
        }
    }

    public static async Task Help(string[] args){
        Console.WriteLine("\nWelcome to the Valour Node Manager. The following commands can be used: \n");
        foreach (var pair in Commands)
        {
            Console.WriteLine($"[{pair.Key}]:");
            Console.WriteLine($"- {pair.Value.description}\n");
        }
    }

    public static async Task AddNode(string[] args){
        if (args.Length != 1){
            Console.WriteLine("Usage: addnode <name>");
            return;
        }

        var name = args[0];

        if (NodeAPI.NodeMap.ContainsKey(name)){
            Console.WriteLine($"Node {name} is already registered.");
            return;
        }

        Node node = new(name);
        await ConfigManager.WriteNode(node);
        NodeAPI.RegisterNode(node);
    }

    public static async Task Nodes(string[] args){
        Console.WriteLine("\n-- Current Nodes --");

        foreach (var node in NodeAPI.NodeMap.Values){
            Console.WriteLine($"• {node.Name}");
        }
    }
}