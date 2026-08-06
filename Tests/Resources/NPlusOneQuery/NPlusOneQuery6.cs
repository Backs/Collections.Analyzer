using System;
using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
}

public class TestClass
{
    public void TestMethod(IEnumerable<User> users)
    {
        var repository = new MyRepository();
        foreach (var user in users)
        {
            var data = {|CI0011:repository.GetData(user.Id)|};
        }
    }

    public class MyRepository
    {
        public string GetData(int id) => id.ToString();
    }
}
