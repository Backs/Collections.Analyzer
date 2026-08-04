using System.Linq;

public class MyClass
{
    public void Foo()
    {
        int[] numbers = new[] { 1, 2, 3 };
        if (Enumerable.Contains(numbers, 1))
        {
        }
    }
}
