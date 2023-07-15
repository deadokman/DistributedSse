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

namespace Distributed.MessagePipe.Implementation;

/// <summary>
/// async response
/// </summary>
public class AsyncStreamHelper : IAsyncStreamHelper
{
    private readonly IAsyncMessagePipe<T> _pipe;

    public async Task BeginResponseAsync<T>(
        HttpResponse response)
    {
        response.Headers.Add("Content-Type", "text/event-stream");
        await response.Body.FlushAsync();
        
        try
        {
            while (true)
            {
                var msgs = await _pipe.WaitForMessagesAsync(reciver, cancellationToken);
                foreach (var msg in msgs)
                {
                    await response
                        .WriteAsync($"data: {msg}\r\r");
                }
            }
        }
        catch (TaskCanceledException)
        {
            await _pipe.DisconnectAsync(reciver);
        }
        catch (Exception ex)
        {
            await response.WriteAsync(ex.ToString());
            response.StatusCode = 500;
        }
    }
}

public interface IAsyncStreamHelper
{
}