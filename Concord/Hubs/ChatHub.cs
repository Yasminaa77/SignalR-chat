using Microsoft.AspNetCore.SignalR;


namespace Concord.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        Console.WriteLine("Message recieved: "+ message );
        //Broadcast to all clients
        await Clients.All.SendAsync("ReceiveMessage", message); //send message to others
    } 

//allow someone to connect to specific group
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("A Client Connected:" + Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync (Exception exception)
    {
        Console.WriteLine("A Client Disonnected:" + Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }




public async Task DeleteMessage(int messageId)
{
    // TODO: Delete message from the database

    // Notify all clients that the message has been deleted
    await Clients.All.SendAsync("MessageDeleted", messageId);
}

    public async Task RemoveFromGroup(string groupName)
    {
        //Authenticate user if you want 
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        // await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        // Console.WriteLine("Removed from group: " + groupName);
    }

// what this function do is allow a client to join a group
    public async Task AddtoGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        // await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        // Console.WriteLine("Added to group: " + groupName);
    }

}