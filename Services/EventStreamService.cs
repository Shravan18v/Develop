using System.Collections.Concurrent;

public static class EventStreamService
{
    private static readonly BlockingCollection<string> _events = new();

    public static void AddEvent(string eventJson)
    {
        _events.Add(eventJson);
    }

    public static BlockingCollection<string> GetEventQueue()
    {
        return _events;
    }
}
