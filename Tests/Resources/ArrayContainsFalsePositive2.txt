using System;

public static class Extensions
{
    public static bool Contains(this int[] arr, int value) => true;
}

public class MyClass
{
    public void Foo()
    {
        int[] numbers = new[] { 1, 2, 3 };
        if (numbers.Contains(1))
        {
        }
    }
}
