using System;
using System.Threading;

public static class NotificationService
{
    public static void SendNotificationTaskAsync(int taskId)
    {
        new Thread(() =>
        {
            Thread.Sleep(2000); // Simulate delay
            Console.WriteLine($"✅ Notification sent for task {taskId}");
        })
        {
            IsBackground = true
        }.Start();
    }
}
