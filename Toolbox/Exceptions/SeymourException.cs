using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Exceptions
{
    class SeymourException : Exception
    {
        public SeymourException(string message) : base(message)
        {
        }
    }
}
