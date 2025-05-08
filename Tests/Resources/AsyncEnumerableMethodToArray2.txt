namespace Examples
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ReturnArray
    {
        public static async ValueTask<IEnumerable<int>> Method1()
        {
            var list = new List<int> { 1 };
            await Task.CompletedTask;

            return list.ToArray();
        }
    }
}