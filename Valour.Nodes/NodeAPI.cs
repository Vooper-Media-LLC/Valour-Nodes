using System.Collections.Concurrent;
using Valour.Nodes.Models;

namespace Valour.Nodes;

public static class NodeAPI
{
    /// <summary>
    /// All of the nodes, selectable by name
    /// </summary>
    public static ConcurrentDictionary<string, Node> Nodes = new();

    /// <summary>
    /// A map from Planet ID to the node it belongs to
    /// </summary>
    public static ConcurrentDictionary<ulong, Node> PlanetMap = new();

    /// <summary>
    /// Initializes the Node API
    /// </summary>
    static NodeAPI()
    {
        Console.WriteLine("Loading saved nodes...");

        // Load saved nodes
        foreach (var node in ConfigManager.ReadNodes()){
            // Place node into node dictionary
            Nodes[node.Name] = node;
            Console.WriteLine($"Loading node {node}...");
        }
    }

    public static void AddRoutes(WebApplication app)
    {
        app.MapGet("/ping", Ping);
        app.MapGet("/nodes", GetNodes);
    }

    public static void RegisterNode(Node node)
    {
        Nodes[node.Name] = node;
    }

    public static string Ping() => "pong";

    public static object GetNodes() => Nodes;
}

