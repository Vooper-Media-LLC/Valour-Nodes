using System.Text.Json.Serialization;

namespace Valour.Nodes.Models;

/// <summary>
/// A node is a server instance running Valour.Server software.
/// </summary>
public class Node
{
    /// <summary>
    /// The name of this node. Should be short and memorable.
    /// </summary>

    public string Name { get; set; }

    public string Address { get; set; }

    public string Version { get; set; }

    public DateTime LastPingTime { get; set; }

    public Node()
    {

    }

    public Node(string name, string address)
    {
        Name = name;
        Address = address;
    }
}

