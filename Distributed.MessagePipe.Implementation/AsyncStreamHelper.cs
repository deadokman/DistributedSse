// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncStreamHelper.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  async response
// </summary>

using System.Text.Json;
using Distributed.MessagePipe.Interface;
using Microsoft.AspNetCore.Http;

namespace Distributed.MessagePipe.Implementation;

/// <summary>
/// async response
/// </summary>
/// <typeparam name="TMessage">Message type</typeparam>
public class AsyncStreamHelper<TMessage> : IAsyncStreamHelper<TMessage>
    where TMessage : class
{
    private readonly IAsyncMessagePipe<TMessage> _pipe;

    /// <summary>
    /// »нициализирует новый экземпл€р класса <see cref="AsyncStreamHelper{T}"/>.
    /// </summary>
    /// <param name="pipe">Pipe</param>
    public AsyncStreamHelper(IAsyncMessagePipe<TMessage> pipe)
    {
        _pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
    }

    /// <inheritdoc/>
    public async Task WriteToResponse(
                HttpResponse response,
                string reciver,
                CancellationToken cancellationToken)
    {
        response.Headers.Add("Content-Type", "text/event-stream");
        await response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            while (true)
            {
                var msgs = await _pipe.WaitForMessagesAsync(reciver, cancellationToken)
                    .ContinueWith(
                    e =>
                    {
                        if (e.IsFaulted)
                        {
                            throw e.Exception;
                        }

                        return e.Result;
                    },
                    TaskScheduler.Default).ConfigureAwait(false);
                foreach (var msg in msgs)
                {
                    var msgBody = JsonSerializer.Serialize(msg);
                    await response
                        .WriteAsync($"data: {msgBody}\r\r", cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (TaskCanceledException)
        {
            await _pipe.DisconnectAsync(reciver).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await response.WriteAsync($"data: {ex.Message}\r\r", cancellationToken: cancellationToken).ConfigureAwait(false);
            response.StatusCode = 500;
        }
    }

    /// <inheritdoc/>
    public async Task SendAsync(string reciver, TMessage message)
    {
        await _pipe.SendAsync(reciver, message).ConfigureAwait(false);
    }
}