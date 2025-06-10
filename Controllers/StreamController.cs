using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class StreamController : ControllerBase
{

  // This controller supports Server-Sent Events (SSE), allowing clients (e.g. RxJS) to subscribe to task updates.
  // This responds to a GET request to /api/stream with a continuous data stream (text/event-stream).
    [HttpGet]
    public async Task Stream()
    {
    // Tells the browser this is a streaming connection using SSE.
        Response.Headers.Add("Content-Type", "text/event-stream");

        var queue = EventStreamService.GetEventQueue();
     // Keeps the connection open indefinitely.

        while (!HttpContext.RequestAborted.IsCancellationRequested)
        {
         // Tries to get the next task update from the shared event queue (BlockingCollection<string>).
            if (queue.TryTake(out var eventData, TimeSpan.FromSeconds(5)))
            {
             // Sends a properly formatted SSE message to the client.
                var message = $"data: {eventData}\n\n";
                var bytes = Encoding.UTF8.GetBytes(message);
                await Response.Body.WriteAsync(bytes, 0, bytes.Length);
                await Response.Body.FlushAsync();
            }
        }

        
    }
}
