// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisSharedStateHolder.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Async message pipe interface
// </summary>

using Distributed.MessagePipe.Interface;
using Distributed.MessagePipe.Redis.SharedStore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Distributed.MessagePipe.Implementation;

/// <summary>
/// Messages shared state holder
/// </summary>
public class RedisSharedStateStore : ISharedStateStore
{
    private readonly IOptions<RedisSharedStoreOptions> _options;
    private readonly ConcurrentDictionary<(Type, Guid), string> _channelCache;

    protected ConcurrentDictionary<Type, SharedSubscription> ObserversCollection { get; set; }

    protected ConnectionMultiplexer RedisConnection { get; private set; }

    protected ISubscriber RedisSub { get; set; }

    public RedisSharedStateStore(IOptions<RedisSharedStoreOptions> options)
    {
        ObserversCollection = new();
        _channelCache = new ();
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public virtual async Task AddObserverAsync<TMessage>(ISharedStateMessageObserver observer)
        where TMessage : class
    {
        if (!ObserversCollection.ContainsKey(observer.Type))
        {
            var sharedSub = new SharedSubscription();
            sharedSub.Observer = observer;

            if (ObserversCollection.TryAdd(observer.Type, sharedSub))
            {
                var channelName = CreateChannelName(observer.Type, observer.ObserverUUID);

                // Create sub/pub for message type
                var channel = await RedisSub
                    .SubscribeAsync(CreateChannelName(observer.Type, observer.ObserverUUID));
                sharedSub.Queue = channel;

                channel.OnMessage(async (m) =>
                {
                    if (m.Message.HasValue && ObserversCollection.TryGetValue(observer.Type, out var subscription))
                    {
                        var messageWithPayload = JsonSerializer.Deserialize<RedisMessageWithPayload<TMessage>>(m.Message);
                        if (subscription.Observer.ObserverUUID != messageWithPayload.ObserverGuid)
                        {
                            await subscription.Observer
                                .OnNewMessageAsync(messageWithPayload.Receiver, messageWithPayload.Payload);
                        }
                    }
                });

                sharedSub.Complete();
            }
        }
    }

    public virtual async Task NotifyNewMessageAsync<T>(string receiver, T message)
        where T : class
    {
        var type = typeof(T);
        if (ObserversCollection.TryGetValue(type, out var subscription))
        {
            await subscription.InitCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));
            var channel = CreateChannelName(type, subscription.Observer.ObserverUUID);
            var msg = new RedisMessageWithPayload<T>()
            {
                ObserverGuid = subscription.Observer.ObserverUUID,
                Receiver = receiver,
                Payload = message,
            };

            var payload = JsonSerializer.Serialize(msg);
            await RedisSub.PublishAsync(channel, payload);
        }
    }

    public virtual async Task ConnectToSharedStoreAsync()
    {
        if (RedisConnection != null)
        {
            await RedisConnection.DisposeAsync();
        }

        var configuration = ConfigurationOptions.Parse(_options.Value.RedisConnectionString);
        RedisConnection = await ConnectionMultiplexer.ConnectAsync(configuration);
        RedisSub = RedisConnection.GetSubscriber();
    }

    private string CreateChannelName(Type messageType, Guid uuid)
    {
        if (!_channelCache.TryGetValue((messageType, uuid), out var channel))
        {
            channel = $"sharedstore-{messageType.AssemblyQualifiedName}-{messageType.FullName}"
                .Trim().Replace(" ", "").Replace(',', '.');
            _channelCache[(messageType, uuid)] = channel;
        }

        return channel;
    }

    public void Dispose()
    {
        RedisConnection.Dispose();
    }

    public async Task Unsubscribe(ISharedStateMessageObserver asyncPipeWrap)
    {
        if (ObserversCollection.TryRemove(asyncPipeWrap.Type, out var subscription))
        {
            await subscription?.Queue.UnsubscribeAsync();
        }
    }
}