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
        public IAsyncMessagePipe<T> Create<T>()
            where T : class
        {
            var instance = Activator.CreateInstance<AsyncMessagePipe<T>>();
            // _sharedStateHolder.AddMessageObserver(instance);

            return instance;
        }

        public void Utilize<T>(IAsyncMessagePipe<T> wrappedPipe) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
