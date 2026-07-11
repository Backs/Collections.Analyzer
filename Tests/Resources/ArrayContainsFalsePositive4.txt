using System.Linq;

public class MyClass
{
    public void Foo()
    {
        int[] numbers = new[] { 1, 2, 3 };
        Bar(numbers);
        if (numbers.Contains(1))
        {
        }
    }

    private void Bar(int[] arr) {}
}
