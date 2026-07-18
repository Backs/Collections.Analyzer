using System;
using System.Collections.Generic;
using System.Linq;

class TestClass
{
    class Requisite
    {
        public int DocType { get; set; }
    }

    void TestMethod(List<Requisite> actual, List<Requisite> requisites)
    {
        foreach (var requisite in requisites)
        {
            if (actual.Any(a => a.DocType == requisite.DocType))
            {
                Console.WriteLine("Found");
            }
        }
    }
}
