// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncMessagePipeFactory.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Message pipe factory
// </summary>

using Distributed.MessagePipe.Interface;

namespace Distributed.MessagePipe.Implementation
{
    /// <summary>
    /// Message pipe factory
    /// </summary>
    internal class AsyncMessagePipeFactory : IAsyncMessagePipeFactory
    {
        public AsyncMessagePipeFactory(ISharedStateStore sharedStateHolder)
        {
        }

        /// <inheritdoc/>
        public IAsyncMessagePipe<TMessage> Create<TMessage>()
            where TMessage : class
        {
            var instance = Activator.CreateInstance<AsyncMessagePipe<TMessage>>();
            // _sharedStateHolder.AddMessageObserver(instance);

            return instance;
        }

        public void Utilize<TMessage>(IAsyncMessagePipe<TMessage> wrappedPipe) where TMessage : class
        {
            throw new NotImplementedException();
        }
    }
}
