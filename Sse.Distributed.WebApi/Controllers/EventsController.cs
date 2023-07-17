using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using StreamEvent.Pipe.Interface;

namespace Sse.Distributed.WebApi.Controllers;

[ApiController]
[Route("/api/sse")]
public class EventsController : ControllerBase
{
    private IAsyncStreamHelper<MyTestMessage> _asyncStreamHelper;

    public EventsController(IAsyncStreamHelper<MyTestMessage> pipe)
    {
        _asyncStreamHelper = pipe ?? throw new ArgumentNullException(nameof(pipe));
    }

    [HttpGet("subscribe")]
    public async Task Get(CancellationToken cancellationToken, [FromQuery] string receiver)
    {
        await _asyncStreamHelper.WriteToResponse(Response, receiver, cancellationToken);
    }

    [HttpPost("send")]
    public async Task Send([FromBody] (string receiver, MyTestMessage message) req)
    {
        await _asyncStreamHelper.SendAsync(req.receiver, req.message);
    }
}