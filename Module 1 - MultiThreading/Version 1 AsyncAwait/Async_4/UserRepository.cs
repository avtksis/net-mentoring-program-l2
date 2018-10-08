using System;
using System.Threading.Tasks;

namespace Async_4
{
    public class UserRepository
    {
        private readonly FakeDataBase _fakeDataBase = new FakeDataBase();

        public Task<User> CreateAsync(User user)
        {
            return Task.Run(() => _fakeDataBase.Create(user));
        }

        public Task<User> ReadAsync(User user)
        {
            return Task.Run(() => _fakeDataBase.Read());
        }
    }
}