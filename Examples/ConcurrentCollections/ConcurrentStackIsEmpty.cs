using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections;

public class ConcurrentStackIsEmpty
{
    public bool Method1()
    {
        var dict = new ConcurrentStack<string>(new[] { "a", "b", "c" });

        return dict.Any();
    }

    public bool Method2()
    {
        var dict = new ConcurrentStack<string>(new[] { "a", "b", "c" });

        return dict.Any(o => o == "d");
    }

    public bool Method3()
    {
        return GetBag().Any();
    }

    private static ConcurrentStack<string> GetBag()
    {
        return new ConcurrentStack<string>(new[] { "a", "b", "c" });
    }
}