namespace Examples.Strings
{
    using System.Linq;

    public class LinqMethods
    {
        public void ToCharArraySelect()
        {
            var str = "string";

            var result = str.ToCharArray().Select(o => o);
        }

        public void ToArraySelect()
        {
            var str = "string";

            var result = str.ToArray().Select(o => o);
        }
        
        public void ToListSelect()
        {
            var str = "string";

            var result = str.ToList().Any();
        }
    }
}