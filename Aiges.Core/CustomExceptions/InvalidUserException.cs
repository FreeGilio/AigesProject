using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.CustomExceptions
{
    public class InvalidUserException : Exception
    {
        public InvalidUserException() : base("An error occurred due to an invalid user ID.") { }

        public InvalidUserException(string message) : base(message) { }

        public InvalidUserException(string message, Exception innerException) : base(message, innerException) { }
    }
}
