using System;

public class MyClass
{
    public void Foo()
    {
        int[] numbers = new[] { 1, 2, 3 };
        if (CustomContains(numbers, 1))
        {
        }
    }

    private bool CustomContains(int[] arr, int value) => true;
}
