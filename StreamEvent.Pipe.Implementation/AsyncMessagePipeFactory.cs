// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncMessagePipeFactory.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Message pipe factory
// </summary>

using StreamEvent.Pipe.Interface;

namespace StreamEvent.Pipe.Implementation
{
    /// <summary>
    /// Message pipe factory
    /// </summary>
    internal class AsyncMessagePipeFactory : IAsyncMessagePipeFactory
    {
        /// <inheritdoc/>
        public IAsyncMessagePipe<TMessage> Create<TMessage>()
            where TMessage : class
        {
            var instance = Activator.CreateInstance<AsyncMessagePipe<TMessage>>();
            return instance;
        }

        /// <inheritdoc/>
        public void Utilize<TMessage>(IAsyncMessagePipe<TMessage> wrappedPipe)
            where TMessage : class
        {
            return;
        }
    }
}
