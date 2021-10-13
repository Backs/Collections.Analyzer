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

            var result = str.ToArray().Any(o => o != ' ');
        }
        
        public void ToListSelect()
        {
            var str = "string";

            var result = str.ToList().Any();
        }
        
        public void GetStringToCharArraySelect()
        {
            var result = GetString().ToCharArray().Select(o => o);
        }
        
        public void GetStringToArraySelect()
        {
            var result = GetString().ToArray().Select(o => o);
        }
        
        private static string GetString()
        {
            return "str";
        }
    }
}