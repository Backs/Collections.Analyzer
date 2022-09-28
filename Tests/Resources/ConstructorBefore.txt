using System.Collections.Generic;
using System.Linq;

namespace Examples.Enumerable
{
    public class Constructor
    {
        public void ManyArgumentsConstructor()
        {
            var list = System.Linq.Enumerable.Empty<Constructor>();

            var obj = new MyClass<Constructor>({|CI0003:list.ToArray()|}, false, {|CI0003:GetList().ToArray()|});
        }

        private static IEnumerable<Constructor> GetList()
        {
            return System.Linq.Enumerable.Empty<Constructor>();
        }

        private class MyClass<T>
        {
            private readonly IEnumerable<T> source;

            public MyClass(IEnumerable<T> source)
            {
                source = source;
            }
            
            public MyClass(IEnumerable<T> source, bool value, IEnumerable<T> test)
            {
                source = source;
                if (value)
                {
                    source = test;
                }
            }
        }
    }
}