using Distributed.MessagePipe.Interface;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributed.MessagePipe.Implementation
{
    internal class SharedStoreHostedService : IHostedService, IDisposable
    {
        private readonly ISharedStateStore _store;
        private readonly IHostApplicationLifetime lifetime;

        static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
        {
            // 👇 Создаём TaskCompletionSource для ApplicationStarted
            var startedSource = new TaskCompletionSource();
            using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());

            // 👇 Создаём TaskCompletionSource для stoppingToken
            var cancelledSource = new TaskCompletionSource();
            using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

            // Ожидаем любое из событий запуска или запроса на остановку
            Task completedTask = await Task.WhenAny(startedSource.Task, cancelledSource.Task).ConfigureAwait(false);

            // Если завершилась задача ApplicationStarted, возвращаем true, иначе false
            return completedTask == startedSource.Task;
        }

        public SharedStoreHostedService(ISharedStateStore store, IHostApplicationLifetime lifetime)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            this.lifetime = lifetime;
        }

        public void Dispose()
        {
            _store.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    await _store.ConnectToSharedStoreAsync();
                } 
                catch (Exception ex)
                {
                    
                }
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _store.Dispose();
            return Task.CompletedTask;
        }
    }
}
