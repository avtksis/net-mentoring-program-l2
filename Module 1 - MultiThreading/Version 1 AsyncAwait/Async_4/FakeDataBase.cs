using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Async_4
{
    public class FakeDataBase
    {
        private readonly List<User> _users = new List<User>()
        {
            new User
            {
                Id = 1,
                Age = 18,
                Name = "John",
                LastName = "Smith"
            }
        };

        public User Read(int userId)
        {
            Thread.Sleep(2000);

            return _users.First(usr => usr.Id == userId);
        }

        public User Create(User user)
        {
            Thread.Sleep(2000);
            _users.Add(user);

            return user;
        }

        public User Update(User user)
        {
            Thread.Sleep(2000);

            var userToEdit = _users.First(usr => usr.Id == user.Id);
            userToEdit.Id = user.Id;
            userToEdit.Age = user.Age;
            userToEdit.Name = user.Name;
            userToEdit.LastName = user.LastName;

            return userToEdit;
        }

        public bool Delete(User user)
        {
            Thread.Sleep(2000);

            return _users.Remove(user);
        }
    }
}