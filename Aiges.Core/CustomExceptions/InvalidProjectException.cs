using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiges.Core.CustomExceptions
{
    public class InvalidProjectException : Exception
    {
        public InvalidProjectException(string message, Exception innerException) : base(message, innerException) { }
    }
}
