using StackExchange.Redis;
using StreamEvent.Pipe.Interface;

namespace StreamEvent.SharedStore.Redis
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
