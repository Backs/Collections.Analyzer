using System.Linq;

namespace Examples.Strings
{
    public class Join
    {
        public static void JoinArray()
        {
            var array = new int[10];

            var result = string.Join("; ", array.Where(o => o != 1).ToArray());
        }

        public static void MethodJoinArray()
        {
            var result = string.Join("; ", GetArray().Where(o => o != 1).ToList());
        }

        private static int[] GetArray()
        {
            return new int[10];
        }
    }
}