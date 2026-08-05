using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Resources.NPlusOneQuery.ParameterTypes
{
    public class User { }

    public class UserRepository
    {
        public User GetByName(string name) => null;
        public User GetByData(byte[] data) => null;
        public User GetByTags(string[] tags) => null;
    }

    public class UserService
    {
        private readonly UserRepository _repo = new UserRepository();

        public void Process(IEnumerable<string> names, IEnumerable<byte[]> datas, IEnumerable<string[]> tagsList)
        {
            foreach (var name in names)
            {
                // string is not considered a bulk parameter, SHOULD be a warning
                {|CI0011:_repo.GetByName(name)|};
            }

            foreach (var data in datas)
            {
                // byte[] is not considered a bulk parameter, SHOULD be a warning
                {|CI0011:_repo.GetByData(data)|};
            }

            foreach (var tags in tagsList)
            {
                // string[] is considered a bulk parameter, SHOULD NOT be a warning
                _repo.GetByTags(tags);
            }
        }
    }
}
