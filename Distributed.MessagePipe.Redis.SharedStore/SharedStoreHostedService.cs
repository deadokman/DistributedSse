using Distributed.MessagePipe.Interface;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributed.MessagePipe.Redis.SharedStore 
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
