using System.Collections.Generic;
using System.Linq;

namespace Examples.Enumerable
{
    public class Constructor
    {
        public void CallConstructor()
        {
            ICollection<int> list = new int[10];

            var obj = new MyClass<int>(list.ToArray());
        }

        private class MyClass<T>
        {
            private readonly ICollection<T> source;

            public MyClass(ICollection<T> source)
            {
                source = source;
            }
        }
    }
}