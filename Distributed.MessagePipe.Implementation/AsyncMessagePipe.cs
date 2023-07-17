// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BiDepthByPlanController.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Implementation for async message pipe
// </summary>

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Distributed.MessagePipe.Implementation.Contracts;
using Distributed.MessagePipe.Interface;

namespace Distributed.MessagePipe.Implementation;

/// <summary>
/// Implementation for async message pipe
/// </summary>
/// <typeparam name="T">Message type</typeparam>
internal class AsyncMessagePipe<T> : IAsyncMessagePipe<T>
    where T : class
{
    private ConcurrentDictionary<string, AsyncTaskPack<T>> _messagePipeHolder;

    /// <summary>
    /// Default pipe constructor
    /// </summary>
    public AsyncMessagePipe()
    {
        _messagePipeHolder = new();
    }

    /// <inheritdoc/>
    public async Task SendAsync(string receiver, T message)
    {
        if (_messagePipeHolder.TryGetValue(receiver, out var pack))
        {
            pack.Add(message);

            var tcs = pack.Tcs;
            pack.Reset();
            tcs.TrySetResult();
        }
    }

    /// <inheritdoc/>
    public async Task<ReadOnlyCollection<T>> WaitForMessagesAsync(string receiver, CancellationToken cancellationToken)
    {
        if (!_messagePipeHolder.TryGetValue(receiver, out var pack))
        {
            if (!_messagePipeHolder.TryAdd(receiver, pack = new AsyncTaskPack<T>(new TaskCompletionSource(), cancellationToken)))
            {
                pack = _messagePipeHolder[receiver];
            }
        }

        var resp = await pack.Tcs.Task.ContinueWith(_ =>
        {
            var msgs = new ReadOnlyCollection<T>(pack.Messages.ToList());
            pack.Messages.Clear();
            return msgs;
        }, cancellationToken);

        return resp;
    }

    /// <inheritdoc/>
    public Task DisconnectAsync(string receiver)
    {
        if (_messagePipeHolder.TryRemove(receiver, out var pack))
        {
            pack.Tcs.TrySetCanceled();
            pack.Messages.Clear();
        }

        return Task.CompletedTask;
    }


    public void Dispose()
    {
        _messagePipeHolder.Clear();
    }
}