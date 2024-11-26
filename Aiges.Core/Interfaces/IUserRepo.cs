using Aiges.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.Interfaces
{
    public interface IUserRepo
    {
        public UserDto AttemptLogin(string? email, string? password);

        List<UserDto> GetAllUsers();

        //public bool RegisterUser(string name, string email, string password);

        public UserDto GetUserDtoById(int id);
    }
}
