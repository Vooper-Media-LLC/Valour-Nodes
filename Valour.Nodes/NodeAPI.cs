using System.Collections.Concurrent;
using System.Text.Json;
using Valour.Nodes.Models;

namespace Valour.Nodes;

public static class NodeAPI
{
    public enum NodeState
    {
        Online,
        Problem,
        Offline
    }

    /// <summary>
    /// All of the nodes, selectable by name
    /// </summary>
    public static ConcurrentDictionary<string, Node> NodeMap = new();

    /// <summary>
    /// A map from Planet ID to the node it belongs to
    /// </summary>
    public static ConcurrentDictionary<long, Node> HostedPlanetMap = new();

    /// <summary>
    /// All of the nodes
    /// </summary>
    public static List<Node> Nodes = new();

    /// <summary>
    /// All of the loaded planet ids
    /// </summary>
    public static List<long> HostedPlanetIds = new();

    /// <summary>
    /// The total number of nodes
    /// </summary>
    public static int node_count = 0;

    /// <summary>
    /// The total number of planets currently loaded
    /// </summary>
    public static int planets_hosted = 0;

    /// <summary>
    /// Http client
    /// </summary>
    private static HttpClient _http;

    public class NodeHandshakeResponse
    {
        public string Version { get; set; }
        public List<long> PlanetIds { get; set; }
    }

    /// <summary>
    /// Initializes the Node API
    /// </summary>
    static NodeAPI()
    {
        _http = new HttpClient();

        Console.WriteLine("Loading saved nodes...");

        // Load saved nodes
        foreach (var node in ConfigManager.ReadNodes())
        {
            // Place node into node dictionary
            RegisterNode(node);
            Console.WriteLine($"Loading node {node}...");
        }
    }

    public static void AddRoutes(WebApplication app)
    {
        app.MapGet("/ping", PingRoute);
        app.MapGet("/nodes", GetNodesRoute);
        app.MapGet("/nodes/planet/{planet_id}", GetPlanetNodeRoute);
        app.MapGet("/nodes/planet/{planet_id}/name", GetPlanetNodeNameRoute);
    }

    public static async void RegisterNode(Node node)
    {
        // Perform handshake with node to ensure it is valid and online
        var response = await _http.GetAsync($"{node.Address}/api/version");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to contact node '{node.Name} ({response.StatusCode})'");
        }
        else
        {
            var content = await JsonSerializer.DeserializeAsync<NodeHandshakeResponse>(await response.Content.ReadAsStreamAsync());

            node.LastPingTime = DateTime.UtcNow;
            node.Version = content.Version;

            // Set planets that the node is serving.
            foreach (var planetId in content.PlanetIds)
            {
                if (!HostedPlanetMap.ContainsKey(planetId))
                {
                    HostedPlanetMap[planetId] = node;
                }
                else
                {
                    // TODO: Tell the node to stop serving this planet, since we already have a node
                    // serving it. Or determine to tell *that* node to stop. 
                }
            }

            NodeMap[node.Name] = node;
            Nodes.Add(node);
            Console.WriteLine($"Registered node '{node.Name}' (Version {node.Version})");
            node_count++;
        }
    }

    // Brings a planet online by finding the best suited node and
    // instructing it to host the planet
    public static Node AddHostedPlanet(long planet_id)
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


        // Return the chosen node
        return node;
    }

    /// <summary>
    /// Returns the node for a given planet id;
    /// Sets up the planet if there is none.
    /// </summary>
    public static Node GetPlanetNode(long planet_id)
    {
        // Check if we have the planet registered
        if (HostedPlanetMap.ContainsKey(planet_id))
        {
            return HostedPlanetMap[planet_id];
        }

        // In this case the planet is not owned by any nodes yet.
        return AddHostedPlanet(planet_id);
    }

    #region routes

    /// <summary>
    /// Returns the node of the given planet id
    /// </summary>
    public static IResult GetPlanetNodeRoute(long planet_id)
    {
        return Results.Json(GetPlanetNode(planet_id));
    }

    /// <summary>
    /// Returns the node name of the given planet id
    /// </summary>
    public static IResult GetPlanetNodeNameRoute(long planet_id)
    {
        return Results.Ok(GetPlanetNode(planet_id).Name);
    }

    public static string PingRoute() => "pong";

    public static object GetNodesRoute() => Nodes;

    #endregion
}

