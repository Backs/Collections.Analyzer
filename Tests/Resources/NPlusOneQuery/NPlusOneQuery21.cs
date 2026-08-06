using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Resources.NPlusOneQuery
{
    public class User { }

    public class UserRepository
    {
        public User ReadWithCustomSuffix(int id) => null;
        public User ReadBatch(int id) => null;
    }

    public class TestClass
    {
        public void Process(IEnumerable<int> ids, UserRepository repo)
        {
            foreach (var id in ids)
            {
                // If CustomSuffix is configured as bulk in config, there should be NO warning here
                repo.ReadWithCustomSuffix(id);
                
                // If Batch is NOT in config, there SHOULD be a warning here (as Read is a data access prefix)
                {|CI0011:repo.ReadBatch(id)|};
            }
        }
    }
}
