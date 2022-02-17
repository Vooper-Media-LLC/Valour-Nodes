using System.Collections.Concurrent;
using Valour.Nodes.Models;

namespace Valour.Nodes;

public static class NodeAPI
{
    /// <summary>
    /// All of the nodes, selectable by name
    /// </summary>
    public static ConcurrentDictionary<string, Node> NodeMap = new();

    /// <summary>
    /// A map from Planet ID to the node it belongs to
    /// </summary>
    public static ConcurrentDictionary<ulong, Node> HostedPlanetMap = new();

    /// <summary>
    /// All of the nodes
    /// </summary>
    public static List<Node> Nodes = new();

    /// <summary>
    /// All of the loaded planet ids
    /// </summary>
    public static List<ulong> HostedPlanetIds = new();

    /// <summary>
    /// The total number of nodes
    /// </summary>
    public static int node_count = 0;

    /// <summary>
    /// The total number of planets currently loaded
    /// </summary>
    public static int planets_hosted = 0;

    /// <summary>
    /// Initializes the Node API
    /// </summary>
    static NodeAPI()
    {
        Console.WriteLine("Loading saved nodes...");

        // Load saved nodes
        foreach (var node in ConfigManager.ReadNodes()){
            // Place node into node dictionary
            RegisterNode(node);
            Console.WriteLine($"Loading node {node}...");
        }
    }

    public static void AddRoutes(WebApplication app)
    {
        app.MapGet("/ping", PingRoute);
        app.MapGet("/nodes", GetNodesRoute);
        app.MapGet("/locate/{planet_id}", GetPlanetLocation);
    }

    public static void RegisterNode(Node node)
    {
        NodeMap[node.Name] = node;
        Nodes.Add(node);
        Console.WriteLine($"Registered node '{node.Name}'");
        node_count++;
    }

    // Brings a planet online by finding the best suited node and
    // instructing it to host the planet
    public static Node AddHostedPlanet(ulong planet_id)
    {
        // For now, choose a node by simply iterating over
        // the available options
        Node node = NodeAPI.Nodes[planets_hosted % NodeAPI.node_count];

        // Now that a node is chosen, add the planet as belonging to that node
        HostedPlanetMap[planet_id] = node;

        // Add to loaded planets
        planets_hosted++;

        // Communicate with the actual Valour node
        // to add the planet
        // TODO

        // Return the chosen node
        return node;
    }

    #region routes

    /// <summary>
    /// Returns the node location of the given planet id
    /// </summary>
    public static Node GetPlanetLocation(ulong planet_id)
    {
        // Check if we have the planet registered
        if (HostedPlanetMap.ContainsKey(planet_id))
        {
            return HostedPlanetMap[planet_id];
        }

        // In this case the planet is not owned by any nodes yet.
        return AddHostedPlanet(planet_id);
    }

    public static string PingRoute() => "pong";

    public static object GetNodesRoute() => Nodes;

    #endregion
}

