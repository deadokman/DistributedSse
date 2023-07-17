// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Message pipe extensions
// </summary>

using Distributed.MessagePipe.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Distributed.MessagePipe.Implementation
{
    /// <summary>
    /// Message pipe extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Register
        /// </summary>
        /// <param name="sc">Service collection from DI</param>
        /// <param name="configureSharedStore">Configure shared store delegate</param>
        /// <returns>Redister message pipe implementation</returns>
        public static IServiceCollection RegisterMessagePipe(
            this IServiceCollection sc,
            Func<IServiceProvider, ISharedStateStore> configureSharedStore)
        {
            if (sc is null)
            {
                throw new ArgumentNullException(nameof(sc));
            }

            if (configureSharedStore is null)
            {
                throw new ArgumentNullException(nameof(configureSharedStore));
            }

            sc.TryAddSingleton(typeof(IAsyncMessagePipe<>), typeof(AsyncPipeWrap<>));
            sc.TryAddSingleton(typeof(IAsyncStreamHelper<>), typeof(AsyncStreamHelper<>));
            sc.TryAddSingleton<IAsyncMessagePipeFactory, AsyncMessagePipeFactory>();
            sc.TryAddSingleton(configureSharedStore.Invoke);

            sc.AddHostedService<SharedStoreHostedService>();

            return sc;
        }
    }
}
