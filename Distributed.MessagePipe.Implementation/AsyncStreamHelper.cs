// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncStreamHelper.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  async response
// </summary>

using Distributed.MessagePipe.Interface;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Distributed.MessagePipe.Implementation;

/// <summary>
/// async response
/// </summary>
public class AsyncStreamHelper<T> : IAsyncStreamHelper<T>
    where T : class
{
    private readonly IAsyncMessagePipe<T> _pipe;

    public AsyncStreamHelper(IAsyncMessagePipe<T> pipe)
    {
        _pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
    }

    public async Task WriteToResponse(
                HttpResponse response,
        string reciver,
        CancellationToken cancellationToken)
    {
        response.Headers.Add("Content-Type", "text/event-stream");
        await response.Body.FlushAsync();
        
        try
        {
            while (true)
            {
                var msgs = await _pipe.WaitForMessagesAsync(reciver, cancellationToken)
                    .ContinueWith(e =>
                    {
                        if (e.IsFaulted)
                        {
                            throw e.Exception;
                        }

                        return e.Result;
                    });
                foreach (var msg in msgs)
                {
                    var msgBody = JsonSerializer.Serialize(msg);
                    await response
                        .WriteAsync($"data: {msgBody}\r\r");
                }
            }
        }
        catch (TaskCanceledException)
        {
            await _pipe.DisconnectAsync(reciver);
        }
        catch (Exception ex)
        {
            await response.WriteAsync($"data: {ex.Message}\r\r");
            response.StatusCode = 500;
        }
    }

    public async Task SendAsync(string reciver, T message)
    {
        await _pipe.SendAsync(reciver, message);
    }
}