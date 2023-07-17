using Microsoft.AspNetCore.Http;

namespace StreamEvent.Pipe.Interface
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
