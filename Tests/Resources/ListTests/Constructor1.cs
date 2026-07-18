using System.Collections.Generic;
using System.Linq;

namespace Examples.Enumerable
{
    public class Constructor
    {
        public void CallConstructor()
        {
            var list = System.Linq.Enumerable.Empty<int>();

            var obj = new MyClass<int>(list.ToArray());
        }

        private class MyClass<T>
        {
            private readonly IEnumerable<T> source;

            public MyClass(IEnumerable<T> source)
            {
                source = source;
            }
        }
    }
}