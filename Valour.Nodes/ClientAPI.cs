using Valour.Nodes.Models;

namespace Valour.Nodes;

/// <summary>
/// The API for serving the client
/// </summary>
public static class ClientAPI
{
    /// <summary>
    /// Keeps track of the number of client requests
    /// </summary>
    public static int client_requests = 0;

    public static void AddRoutes(WebApplication app){
        app.MapGet("/client", ClientRoute);
        Console.WriteLine("Registered client routes");
    }

    /// <summary>
    /// This route serves the client. The goal is to evenly divide the requests across all nodes - 
    /// the client should be identical on them all!
    /// </summary>
    public static IResult ClientRoute(){

        Node node = NodeAPI.Nodes[client_requests % NodeAPI.node_count];

        // Iterate requests
        client_requests++;

        Console.WriteLine("Redirect to " + node.Address);

        return Results.Redirect(node.Address, false, true);
    }
}