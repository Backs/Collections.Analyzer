using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections;

public class ConcurrentStackIsEmpty
{
    public bool Method1()
    {
        var stack = new ConcurrentStack<string>(new[] { "a", "b", "c" });

        return stack.Any();
    }

    public bool Method2()
    {
        var stack = new ConcurrentStack<string>(new[] { "a", "b", "c" });

        return stack.Any(o => o == "d");
    }

    public bool Method3()
    {
        return GetStack().Any();
    }

    private static ConcurrentStack<string> GetStack()
    {
        return new ConcurrentStack<string>(new[] { "a", "b", "c" });
    }
}