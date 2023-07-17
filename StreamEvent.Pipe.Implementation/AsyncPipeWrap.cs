// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncPipeWrap.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Wraps pipe implementations with logging and pub/sub notification
// For shared state store
// </summary>

using Microsoft.Extensions.Logging;
using StreamEvent.Pipe.Interface;

namespace StreamEvent.Pipe.Implementation
{
    /// <summary>
    /// Wraps pipe implementations with logging and pub/sub notification
    /// For shared state store
    /// </summary>
    /// <typeparam name="TMessage">Message type</typeparam>
    public class AsyncPipeWrap<TMessage> : IAsyncMessagePipe<TMessage>, ISharedStateMessageObserver
        where TMessage : class
    {
        private readonly IAsyncMessagePipe<TMessage> _wrappedPipe;
        private readonly IAsyncMessagePipeFactory _factory;
        private readonly ISharedStateStore _sharedStateHolder;
        private readonly ILogger _logger;

        /// <inheritdoc/>
        public Type Type => typeof(TMessage);

        /// <summary>
        /// Observer unique ID
        /// </summary>
        public Guid ObserverUUID { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AsyncPipeWrap{TMessage}"/>.
        /// </summary>
        /// <param name="factory">Message pipe factory</param>
        /// <param name="sharedStateHolder">Shared source for messages</param>
        /// <param name="logger">Application logger</param>
        public AsyncPipeWrap(
            IAsyncMessagePipeFactory factory,
            ISharedStateStore sharedStateHolder,
            ILogger<AsyncPipeWrap<TMessage>> logger)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sharedStateHolder = sharedStateHolder ?? throw new ArgumentNullException(nameof(sharedStateHolder));
            _wrappedPipe = factory.Create<TMessage>();
            ObserverUUID = Guid.NewGuid();
        }

        /// <inheritdoc/>
        public async Task DisconnectAsync(string receiver)
        {
            _logger.LogInformation(
                "Receiver [{Receiver}] disconecting UUID [{ObserverUuid}]",
                receiver,
                ObserverUUID);
            await _wrappedPipe.DisconnectAsync(receiver).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _factory.Utilize(_wrappedPipe);
            _sharedStateHolder.Unsubscribe(this);
            _wrappedPipe.Dispose();
        }

        /// <inheritdoc/>
        public async Task OnNewMessageAsync(string receiver, object message)
        {
            if (message is TMessage msg)
            {
                await _wrappedPipe.SendAsync(receiver, msg).ConfigureAwait(false);
            }
            else
            {
                _logger.LogWarning(
                    "Unsupported messsage type {Type} for receiver [{Receiver}] UUID [{ObserverUUID}]",
                    message?.GetType(),
                    receiver,
                    ObserverUUID);
            }
        }

        /// <inheritdoc/>
        public async Task SendAsync(string receiver, TMessage message)
        {
            _logger.LogDebug("Sending message to receiver [{Receiver}] UUID [{ObserverUUID}", receiver, ObserverUUID);
            await _sharedStateHolder.NotifyNewMessageAsync(receiver, message).ConfigureAwait(false);
            await _wrappedPipe.SendAsync(receiver, message).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<TMessage>> WaitForMessagesAsync(string receiver, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Receiver [{Receiver}] waits for messages UUID [{ObserverUUID}", receiver, ObserverUUID);
            await _sharedStateHolder.AddObserverAsync<TMessage>(this).ConfigureAwait(false);
            return await _wrappedPipe.WaitForMessagesAsync(receiver, cancellationToken).ConfigureAwait(false);
        }
    }
}
