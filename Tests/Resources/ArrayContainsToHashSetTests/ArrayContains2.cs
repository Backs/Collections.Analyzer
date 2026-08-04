using System.Linq;

class TestClass
{
    private readonly int[] _list2 = new int[] { 1, 2, 3, 4 };

    public bool Method()
    {
        return _list2.Contains(5);
    }
}
