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
        app.MapGet("/client/{*routeEnd}", ClientRoute);
        Console.WriteLine("Registered client routes");
    }

    /// <summary>
    /// This route serves the client. The goal is to evenly divide the requests across all nodes - 
    /// the client should be identical on them all!
    /// </summary>
    public static IResult ClientRoute(HttpContext ctx, string routeEnd){

        if (NodeAPI.node_count == 0)
            return Results.Problem("No nodes are currently online.");

        Node node = NodeAPI.Nodes[client_requests % NodeAPI.node_count];

        // Iterate requests
        client_requests++;

        var queryString = ctx.Request.QueryString;

        Console.WriteLine("Redirect to " + node.Address + "/" + routeEnd + queryString);

        return Results.Redirect(node.Address + "/" +
            routeEnd + queryString, false, true);
    }
}