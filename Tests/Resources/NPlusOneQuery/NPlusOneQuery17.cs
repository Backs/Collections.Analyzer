using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Resources.NPlusOneQuery
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserRepository
    {
        public static User GetUserById(int id)
        {
            return new User { Id = id, Name = "User" + id };
        }
        
        public User GetUserByIdInstance(int id)
        {
            return new User { Id = id, Name = "User" + id };
        }
    }

    public class TestClass
    {
        public void ProcessUsers(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                // Static method should not call warning
                var user = UserRepository.GetUserById(id);
                Console.WriteLine(user.Name);
            }
        }
        
        public void ProcessUsersInstance(IEnumerable<int> ids, UserRepository repo)
        {
            foreach (var id in ids)
            {
                // Instance method should call warning
                var user = {|CI0011:repo.GetUserByIdInstance(id)|};
                Console.WriteLine(user.Name);
            }
        }
    }
}
