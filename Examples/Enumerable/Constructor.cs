﻿using System.Collections.Generic;
using System.Linq;

namespace Examples.Enumerable
{
    public class Constructor
    {
        public void CallConstructor()
        {
            var enumerable = System.Linq.Enumerable.Empty<int>();

            var obj = new MyClass<int>(enumerable.ToList());
        }

        public void ManyArgumentsConstructor()
        {
            ICollection<Constructor> list = new Constructor[10];

            var obj = new MyClass<Constructor>(list.ToArray(), false, GetEnumerable().ToArray());
        }

        public void ArrayTest()
        {
            var list = new List<int>();
            
            var obj = new MyClass<int>(list.ToArray(), true);
        }

        private static IEnumerable<Constructor> GetEnumerable()
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
            
            public MyClass(ICollection<T> source)
            {
                source = source;
            }

            public MyClass(IEnumerable<T> source, bool value, IReadOnlyCollection<T> test)
            {
                source = source;
                if (value) source = test;
            }
            
            public MyClass(T[] source, bool value)
            {
                source = source;
                if (value) source = null;
            }
        }
    }
}