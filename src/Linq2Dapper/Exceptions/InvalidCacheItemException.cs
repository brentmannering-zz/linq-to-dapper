using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Contrib.Linq2Dapper.Exceptions
{
    public class InvalidCacheItemException : Exception
    {
        public InvalidCacheItemException(string message) : base(message)
        {
        }
    }
}
