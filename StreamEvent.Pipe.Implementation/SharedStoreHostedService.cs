// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharedStoreHostedService.cs" company="StepanovNO">
// Copyright (c) StepanovNO. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Hosted service for shared store runtime sync
// </summary>

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StreamEvent.Pipe.Interface;

namespace StreamEvent.Pipe.Implementation
{
    /// <summary>
    /// Shared store hosted service
    /// </summary>
    internal class SharedStoreHostedService : IHostedService, IDisposable
    {
        private readonly ISharedStateStore _store;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger _logger;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SharedStoreHostedService"/>.
        /// </summary>
        /// <param name="store">Store</param>
        /// <param name="lifetime">Host application _lifetime</param>
        /// <param name="logger">Logger</param>
        public SharedStoreHostedService(
            ISharedStateStore store,
            IHostApplicationLifetime lifetime,
            ILogger<SharedStoreHostedService> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _lifetime = lifetime;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _store.Dispose();
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(OnApplicationStarted);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _store.Dispose();
            return Task.CompletedTask;
        }

        private async void OnApplicationStarted()
        {
            try
            {
                await _store.ConnectToSharedStoreAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());
                throw;
            }
        }
    }
}
