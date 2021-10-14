namespace Examples
{
    using System.Linq;

    public class LinqMethods
    {
        public void ToArraySelect()
        {
            var str = "string";

            var result = str.ToArray().Select(o => o);
        }
    }
}