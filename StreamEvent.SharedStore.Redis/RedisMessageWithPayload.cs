namespace StreamEvent.SharedStore.Redis
{
    /// <summary>
    /// Redis message wrap for payload
    /// </summary>
    internal class RedisMessageWithPayload<T>
    {
        /// <summary>
        /// Observer guid
        /// </summary>
        public Guid ObserverGuid { get; set; }

        /// <summary>
        /// Message receeiver
        /// </summary>
        public required string Receiver { get; init; }

        /// <summary>
        /// Payload
        /// </summary>
        public required T Payload { get; init; }
    }
}
