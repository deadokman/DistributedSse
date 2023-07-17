using Microsoft.Extensions.Hosting;
using StreamEvent.Pipe.Interface;

namespace StreamEvent.SharedStore.Redis 
{
    internal class SharedStoreHostedService : IHostedService, IDisposable
    {
        private readonly ISharedStateStore _store;

        public SharedStoreHostedService(ISharedStateStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public void Dispose()
        {
             _store.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _store.ConnectToSharedStoreAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _store.Dispose();
        }
    }
}
