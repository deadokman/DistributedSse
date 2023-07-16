using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Distributed.MessagePipe.Interface;
using Distributed.MessagePipe.Implementation;

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
    public async Task Send([FromBody] (string reciver, MyTestMessage message) req)
    {
        await _asyncStreamHelper.SendAsync(req.reciver, req.message);
    }
}