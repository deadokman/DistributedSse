using Distributed.MessagePipe.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributed.MessagePipe.Redis.SharedStore
{
    /// <summary>
    /// Shared state subscription
    /// </summary>
    public class SharedSubscription
    {
        public TaskCompletionSource InitCompletionSource { get; private set; }
        
        public ISharedStateMessageObserver Observer { get; internal set; }
        public ChannelMessageQueue Queue { get; internal set; }

        /// <summary>
        /// Shared subscription
        /// </summary>
        public SharedSubscription()
        {
            InitCompletionSource = new TaskCompletionSource();
        }

        public void Complete()
        {
            InitCompletionSource.SetResult();
        }
    }
}
