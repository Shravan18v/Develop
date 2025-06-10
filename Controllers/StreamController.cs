using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class StreamController : ControllerBase
{
    [HttpGet]
    public async Task Stream()
    {
        Response.Headers.Add("Content-Type", "text/event-stream");

        var queue = EventStreamService.GetEventQueue();

        while (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            if (queue.TryTake(out var eventData, TimeSpan.FromSeconds(5)))
            {
                var message = $"data: {eventData}\n\n";
                var bytes = Encoding.UTF8.GetBytes(message);
                await Response.Body.WriteAsync(bytes, 0, bytes.Length);
                await Response.Body.FlushAsync();
            }
        }
    }
}
