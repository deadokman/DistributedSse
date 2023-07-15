using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Distributed.MessagePipe.Interface;

namespace Sse.Distributed.WebApi.Controllers;

[ApiController]
[Route("/api/sse")]
public class EventsController : ControllerBase
{
    private IAsyncMessagePipe<string> _pipe;

    public EventsController(IAsyncMessagePipe<string> pipe)
    {
        _pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
    }

    [HttpGet("subscribe")]
    public async Task Get(CancellationToken cancellationToken, [FromQuery] string reciver)
    {
       
    }

    [HttpPost("send")]
    public async Task Get([FromBody] (string reciver, string message) req)
    {
        await _pipe.Send(req.reciver, req.message);
    }
}