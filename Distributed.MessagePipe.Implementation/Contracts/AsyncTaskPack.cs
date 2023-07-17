// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncTaskPack.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Pack for async pipe operation
// Includes async pipe subscriptuion, cancellation token
// </summary>

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Distributed.MessagePipe.Implementation.Contracts;

/// <summary>
/// Contains task completion source
/// </summary>
/// <typeparam name="TMessage">Message type</typeparam>
public class AsyncTaskPack<TMessage>
{
    /// <summary>
    /// Task completion source
    /// </summary>
    required public TaskCompletionSource Tcs { get; set; }

    /// <summary>
    /// Cancellation token
    /// </summary>
    required public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    protected ConcurrentBag<TMessage> Messages { get; private set; } = new ();

    /// <summary>
    /// Sync object
    /// </summary>
    private object _sync = new ();

    /// <summary>
    /// Set completion source
    /// </summary>
    public void Reset()
    {
        Tcs = new TaskCompletionSource(CancellationToken);
    }

    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="message">Message</param>
    public void Add(TMessage message)
    {
        lock (_sync)
        {
            Messages.Add(message);
        }
    }

    /// <summary>
    /// Clear messages
    /// </summary>
    public void Clear()
    {
        lock (_sync)
        {
            Messages.Clear();
        }
    }

    /// <summary>
    /// Get messages from collection
    /// </summary>
    /// <returns>Return collection</returns>
    public IReadOnlyCollection<TMessage> GetMessages()
    {
        lock (_sync)
        {
            var collection = new ReadOnlyCollection<TMessage>(Messages.ToArray());
            Messages.Clear();
            return collection;
        }
    }

    /// <summary>
    /// »нициализирует новый экземпл€р класса <see cref="AsyncTaskPack{T}"/>.
    /// </summary>
    /// <param name="tcs">Task completion source</param>
    /// <param name="ct">Cancellation token</param>
    [SetsRequiredMembers]
    public AsyncTaskPack(TaskCompletionSource tcs, CancellationToken ct) => (Tcs, CancellationToken) = (tcs, ct);
}