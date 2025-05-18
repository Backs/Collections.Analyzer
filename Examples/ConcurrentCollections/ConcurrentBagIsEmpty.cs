using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections;

public class ConcurrentBagIsEmpty
{
    public bool Method1()
    {
        var bag = new ConcurrentBag<string>(new[] { "a", "b", "c" });

        return bag.Any();
    }

    public bool Method2()
    {
        var bag = new ConcurrentBag<string>(new[] { "a", "b", "c" });

        return bag.Any(o => o == "d");
    }

    public bool Method3()
    {
        return GetBag().Any();
    }

    private static ConcurrentBag<string> GetBag()
    {
        return new ConcurrentBag<string>(new[] { "a", "b", "c" });
    }

    public bool Method4()
    {
        var bag = new ConcurrentBag<string>(new[] { "a", "b", "c" });

        return !bag.Any();
    }

    public void Method5()
    {
        var bag = new ConcurrentBag<string>(new[] { "a", "b", "c" });

        var isEmpty = !bag.Any();
    }

    public void Method6()
    {
        var bag = new ConcurrentBag<string>(new[] { "a", "b", "c" });

        WithEmpty(bag.Any());
    }

    private void WithEmpty(bool b)
    {
        throw new System.NotImplementedException();
    }
}