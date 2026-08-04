using System.Linq;

class TestClass
{
    private readonly int[] {|CI0008:_list2|} = { 1, 2, 3, 4 };

    public bool Method()
    {
        return _list2.Contains(5);
    }
}
