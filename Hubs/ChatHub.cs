using Microsoft.AspNetCore.SignalR;
using PortfolioBackend.Data;
using PortfolioBackend.Models;
using System.Threading.Tasks;

namespace PortfolioBackend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string senderName, string content)
        {
            // Store message in database
            var message = new Message
            {
                SenderName = senderName,
                Content = content
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Broadcast to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}