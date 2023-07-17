// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncMessagePipeFactory.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Message pipe factory
// </summary>

namespace StreamEvent.Pipe.Interface
{
    /// <summary>
    /// Message pipe factory
    /// </summary>
    public interface IAsyncMessagePipeFactory
    {
        /// <summary>
        /// Creates instance of message pipe
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <returns></returns>
        IAsyncMessagePipe<T> Create<T>()
            where T : class;
        
        /// <summary>
        /// Utilize messag pipe
        /// </summary>
        /// <param name="wrappedPipe">Pipe instance</param>
        /// <typeparam name="TMessage">Message type</typeparam>
        void Utilize<TMessage>(IAsyncMessagePipe<TMessage> wrappedPipe) 
            where TMessage : class;
    }
}
