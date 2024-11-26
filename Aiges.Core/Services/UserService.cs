using Aiges.Core.DTO;
using Aiges.Core.Interfaces;
using Aiges.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Services
{
    public class UserService
    {

        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            this._userRepo = userRepo;
        }


        public User Login(string email, string password)
        {
            try
            {
                UserDto userDto = _userRepo.AttemptLogin(email, password);

                if (userDto != null)
                {
                    return new User(userDto);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<User> GetAllUsers()
        {
            List<UserDto> users = _userRepo.GetAllUsers();
            return User.ConvertToUsers(users);
        }    

        public User GetUserById(int? userid)
        {
            UserDto userDto = _userRepo.GetUserDtoById(userid.Value);

            return new User(userDto);
        }
    }
}
