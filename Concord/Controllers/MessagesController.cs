using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Concord.Models;
using Concord.Hubs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Concord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IHubContext<ChatHub> _hub;

        public MessagesController(DatabaseContext context, IHubContext<ChatHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int channelId)
        {
            var messages = await _context.Messages.Where(m => m.ChannelId == channelId).ToListAsync();
            return Ok(messages);
        }
       

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("DeleteMessage", id);
            return NoContent();
        }


    }
}
