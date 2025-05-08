using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections;

public class ConcurrentQueueIsEmpty
{
    public bool Method1()
    {
        var queue = new ConcurrentQueue<string>(new[] { "a", "b", "c" });

        return queue.Any();
    }

    public bool Method2()
    {
        var queue = new ConcurrentQueue<string>(new[] { "a", "b", "c" });

        return queue.Any(o => o == "d");
    }

    public bool Method3()
    {
        return GetQueue().Any();
    }

    private static ConcurrentQueue<string> GetQueue()
    {
        return new ConcurrentQueue<string>(new[] { "a", "b", "c" });
    }
}