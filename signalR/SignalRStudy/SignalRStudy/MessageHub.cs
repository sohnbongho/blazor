using Microsoft.AspNetCore.SignalR;

namespace SignalRStudy
{
    public class MessageHub : Hub
    {
        public async Task Send(string message, string userId)
        {
            // 추가적인 작업가능
            await Clients.All.SendAsync("receive", message, userId);
        }
    }
}
