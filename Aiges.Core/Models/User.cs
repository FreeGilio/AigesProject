using Aiges.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Admin {  get; set; }

        public User() { }

        public User(UserDto userDto)
        {
            Id = userDto.Id;
            Username = userDto.Username;
            Password = userDto.Password;
            Email = userDto.Email;
            CreatedAt = userDto.CreatedAt;
            Admin = userDto.Admin;
        }

        public static List<User> ConvertToUsers(List<UserDto> userDtos)
        {

            List<User> users = new List<User>();

            try
            {
                foreach (UserDto userDto in userDtos)
                {
                    users.Add(new User(userDto));
                }
            }
            catch (Exception ex)
            {

            }


            return users;
        }
    }
}
