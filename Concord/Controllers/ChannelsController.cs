using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;


using Concord.Models;
using Concord.Hubs;

namespace Concord.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IHubContext<ChatHub> _hub;


//constructor
//accept the hub here 
    public ChannelsController( DatabaseContext context, IHubContext<ChatHub> hub)
    {
        _context = context;
        _hub = hub;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Channel>>> GetChannel()
    {
        return await _context.Channels.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Channel>> GetChannel(int id)
    {
        var Channel = await _context.Channels.FindAsync(id);

        if (Channel == null)
        {
            return NotFound();
        }

        return Channel;
    }

    

//GET: api/Channels/5/Messages

[HttpGet("{channelId}/messages")]
public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int channelId)
{
    var messages = await _context.Messages.Where(m => m.ChannelId == channelId).ToListAsync();

    if (messages == null)
    {
        return NotFound();
    }

    return messages;
}




    [HttpPost]
    public async Task<ActionResult<Channel>> PostChannel(Channel Channel)
    {
        _context.Channels.Add(Channel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChannel), new { id = Channel.Id }, Channel);
    }

    //POST: api/Channels/5/Messages
    [HttpPost("{channelId}/Messages")]
      public async Task<Message> PostChannelMessage(int channelId, Message Message)
    {
        Message.ChannelId = channelId;
        _context.Messages.Add(Message);
        await _context.SaveChangesAsync();

        //broadcast message to all clients( message object)
        // await _hub.Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", Message);
        await _hub.Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", Message);
        return Message;

    }



    [HttpPut("{id}")]
    public async Task<IActionResult> PutChannel(int id, Channel Channel)
    {
        if (id != Channel.Id)
        {
            return BadRequest();
        }

        _context.Entry(Channel).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChannel(int id)
    {
        var Channel = await _context.Channels.FindAsync(id);
        if (Channel == null)
        {
            return NotFound();
        }

        _context.Channels.Remove(Channel);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}