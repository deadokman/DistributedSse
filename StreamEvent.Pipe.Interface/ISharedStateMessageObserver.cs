namespace StreamEvent.Pipe.Interface
{
    /// <summary>
    /// Part of observer pattern for new messages income
    /// </summary>
    public interface ISharedStateMessageObserver
    {
        /// <summary>
        /// Observer unique id
        /// </summary>
        Guid ObserverUUID { get; }

        /// <summary>
        /// Observable message type
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Notify got new message
        /// </summary>
        /// <returns>Asybc operation</returns>
        Task OnNewMessageAsync(string receiver, object message);
    }
}
