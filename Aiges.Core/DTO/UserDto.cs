using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiges.Core.Models;

namespace Aiges.Core.DTO
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Admin { get; set; }

        public UserDto() { }

        public UserDto(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Password = user.Password;
            Email = user.Email;
            CreatedAt = user.CreatedAt;
            Admin = user.Admin;
        }

    }
}
