using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.Exceptions
{
    class SeymourException : Exception
    {
        public SeymourException(string message) : base(message)
        {
        }
    }
}
