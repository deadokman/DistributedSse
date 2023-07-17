// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISharedStateHolder.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Async message pipe interface
// </summary>

namespace StreamEvent.Pipe.Interface;

/// <summary>
/// Shared state holder
/// </summary>
public interface ISharedStateStore : IDisposable
{
    /// <summary>
    /// Adds new message observer to shared messages state
    /// </summary>
    /// <param name="messageObserver">Message observer</param>
    Task AddObserverAsync<TMessage>(ISharedStateMessageObserver messageObserver)
        where TMessage : class;


    /// <summary>
    /// Notificates all message observers about new message
    /// </summary>
    /// <param name="receiver"> Receiver </param>
    /// <param name="message"> Message </param>
    Task NotifyNewMessageAsync<T>(string receiver, T message) where T : class;

    Task ConnectToSharedStoreAsync();
    Task Unsubscribe(ISharedStateMessageObserver asyncPipeWrap);
}