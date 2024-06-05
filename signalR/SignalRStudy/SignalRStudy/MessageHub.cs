using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using SignalRStudy.Akka;
using System.Collections.Concurrent;

namespace SignalRStudy;

public class MessageHub : Hub
{
    private readonly ActorSystem _actorSystem;
    private readonly IHubContext<MessageHub> _hubContext;
    private static readonly ConcurrentDictionary<string, IActorRef> _userActors = new ConcurrentDictionary<string, IActorRef>();

    public MessageHub(ActorSystem actorSystem, IHubContext<MessageHub> hubContext)
    {
        _actorSystem = actorSystem;
        _hubContext = hubContext;
    }

    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;
        var userActor = _actorSystem.ActorOf(Props.Create(() 
            => new UserActor(connectionId, _hubContext)), $"userActor-{connectionId}");

        _userActors[connectionId] = userActor;

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;
        if (!string.IsNullOrEmpty(connectionId) && _userActors.TryRemove(connectionId, out var userActor))
        {
            _actorSystem.Stop(userActor);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public void Send(string message, string userId)
    {
        string connectionId = Context.ConnectionId;
        if (_userActors.TryGetValue(connectionId, out var userActor))
        {
            userActor.Tell(new UserMessage(userId, message));
        }
        //await Clients.All.SendAsync("receive", message, userId);
        //await Clients.Client(connectionId).SendAsync("receive", message, userId);
    }

    public async Task SendToUser(string message, string targetUserId)
    {
        if (_userActors.TryGetValue(targetUserId, out var userActor))
        {
            userActor.Tell(new UserMessage(targetUserId, message));
        }

        await Clients.Client(targetUserId).SendAsync("receive", message, targetUserId);
    }
}

