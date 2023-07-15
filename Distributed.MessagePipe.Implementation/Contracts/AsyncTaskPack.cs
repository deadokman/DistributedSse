// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncTaskPack.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Pack for async pipe operation
// </summary>

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Distributed.MessagePipe.Implementation.Contracts;

/// <summary>
/// Contains task completion source
/// </summary>
public class AsyncTaskPack<T>
{
    /// <summary>
    /// Task completion source
    /// </summary>
    public required TaskCompletionSource Tcs { get; set; }
    
    /// <summary>
    /// Cancellation token
    /// </summary>
    public required CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    public ConcurrentBag<T> Messages { get; private set; } = new();
    
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
    public void Add(T message)
    {
        Messages.Add(message);
    }
    
    [SetsRequiredMembers]
    public AsyncTaskPack(TaskCompletionSource tcs, CancellationToken ct) => (Tcs, CancellationToken) = (tcs, ct);
}