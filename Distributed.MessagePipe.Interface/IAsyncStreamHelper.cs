using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributed.MessagePipe.Interface
{
    public interface IAsyncStreamHelper<T>
      where T : class
    {
        Task WriteToResponse(HttpResponse response,
         string reciver,
         CancellationToken cancellationToken);

        Task SendAsync(string reciver, T message);
    }
}
