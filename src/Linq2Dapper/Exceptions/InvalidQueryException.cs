using System;

namespace Dapper.Contrib.Linq2Dapper.Exceptions
{
    public class InvalidQueryException : Exception
    {
        private readonly string _message;

        public InvalidQueryException(string message)
        {
            _message = message + " ";
        }

        public override string Message
        {
            get
            {
                return "The client query is invalid: " + _message;
            }
        }
    }
}
