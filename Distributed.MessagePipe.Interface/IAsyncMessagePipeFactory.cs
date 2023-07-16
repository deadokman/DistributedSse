using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributed.MessagePipe.Interface
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
        void Utilize<T>(IAsyncMessagePipe<T> wrappedPipe) where T : class;
    }
}
