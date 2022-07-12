using System;
using SerenityTask.API.Extensions;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.UnitTests.Services.Builders
{
    internal class UserBuilder
    {
        private Guid _id;

        private string _email;

        private string _username;

        private string _passwordHash;

        private string _name;

        private bool _isEmailConfirmed;

        internal UserBuilder()
        {
            _id = Guid.NewGuid();
            _email = "default@email.com";
            _username = "defaultUsername";
            _name = "DefaultName";
            _passwordHash = "defaultPasswordHash".GetPasswordHash();
            _isEmailConfirmed = false;
        }

        internal User Build()
        {
            return new User
            {
                Id = _id,
                Email = _email,
                Username = _username,
                Name = _name,
                PasswordHash = _passwordHash,
                IsEmailConfirmed = _isEmailConfirmed
            };
        }

        internal UserBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        internal UserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        internal UserBuilder WithUsername(string username)
        {
            _username = username;
            return this;
        }

        internal UserBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal UserBuilder WithPasswordHash(string password)
        {
            _passwordHash = password.GetPasswordHash();
            return this;
        }

        internal UserBuilder WithEmailConfirmedFlag(bool isEmailConfirmed)
        {
            _isEmailConfirmed = isEmailConfirmed;
            return this;
        }
    }
}
