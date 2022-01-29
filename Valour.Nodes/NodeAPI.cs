using System.Collections.Concurrent;
using Valour.Nodes.Models;

namespace Valour.Nodes;

public class NodeAPI
{
    /// <summary>
    /// All of the nodes, selectable by name
    /// </summary>
    public static ConcurrentDictionary<string, Node> Nodes = new();

    /// <summary>
    /// A map from Planet ID to the node it belongs to
    /// </summary>
    public static ConcurrentDictionary<ulong, Node> PlanetMap = new();

    public static void AddRoutes(WebApplication app)
    {
        app.MapGet("/ping", Ping);
        app.MapGet("/nodes", GetNodes);

        RegisterNode(new Node("emma"));
    }

    public static void RegisterNode(Node node)
    {
        Nodes[node.Name] = node;
    }

    public static string Ping() => "pong";

    public static object GetNodes() => Nodes;
}

