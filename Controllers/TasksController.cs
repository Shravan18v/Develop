using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskManagement.Services;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static readonly List<TaskModel> tasks = new()
    {
        new TaskModel { Id = 1, Title = "Grocery Shopping", Completed = false, DueDate = new DateTime(2024, 3, 15) },
        new TaskModel { Id = 2, Title = "Pay Bills", Completed = false, DueDate = new DateTime(2024, 3, 20) }
    };

    private static int nextTaskId = 3;

    [HttpGet]
    public IActionResult GetTasks() => Ok(tasks);

    [HttpPost]
    public IActionResult CreateTask([FromBody] TaskModel newTask)
    {
        newTask.Id = nextTaskId++;
        newTask.Completed = false;
        if (newTask.DueDate == default)
            newTask.DueDate = DateTime.Today;

        tasks.Add(newTask);
        return CreatedAtAction(nameof(GetTasks), new { id = newTask.Id }, newTask);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTask(int id, [FromBody] TaskModel updatedTask)
    {
     // Looks for the task with the given ID. If not found, returns 404.

        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return NotFound();
        // Updates fields from the request payload.
        task.Title = updatedTask.Title;
        task.Completed = updatedTask.Completed;
        task.DueDate = updatedTask.DueDate;

 // Sends a notification in a separate thread, so the HTTP response is not delayed.
        NotificationService.SendNotificationTaskAsync(id); // Async notification

        // Push to SSE stream
        // Serializes the updated task and adds it to an event queue.
        var eventJson = JsonSerializer.Serialize(new { event = "task_updated", task });
        EventStreamService.AddEvent(eventJson);

        return Ok(task);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return NotFound();

        tasks.Remove(task);
        return NoContent();
    }
}
