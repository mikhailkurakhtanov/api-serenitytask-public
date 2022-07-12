using SerenityTask.API.Models.Entities;
using System;
using Task = SerenityTask.API.Models.Entities.Task;

namespace SerenityTask.UnitTests.Services.Builders
{
    internal class TaskBuilder
    {
        private long _id;

        private string _name;

        private DateTime _creationDate;

        private User _user;

        internal TaskBuilder()
        {
            _id = 1;
            _name = "DefaultName";
            _creationDate = DateTime.UtcNow;
            _user = new UserBuilder().Build();
        }

        internal Task Build()
        {
            return new Task
            {
                Id = _id,
                Name = _name,
                CreationDate = _creationDate,
                User = _user
            };
        }

        internal TaskBuilder WithId(long id)
        {
            _id = id;
            return this;
        }

        internal TaskBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal TaskBuilder WithCreationDate(DateTime creationDate)
        {
            _creationDate = creationDate;
            return this;
        }

        internal TaskBuilder WithUserId(User user)
        {
            _user = user;
            return this;
        }
    }
}
