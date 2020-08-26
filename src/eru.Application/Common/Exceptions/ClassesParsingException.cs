using System;

namespace eru.Application.Common.Exceptions
{
    public class ClassesParsingException : Exception
    {
        public ClassesParsingException() : base ("Class must have a year number!")
        {
            
        }
    }
}