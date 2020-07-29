using System;
using System.Reflection;

namespace eru.Application.Common.Exceptions
{
    public class ParsingException : Exception
    {
        public ParsingException(MemberInfo type) :
            base($"Given xml file is not a valid {type.Name} file or couldn't be parsed.")
        {
        }
    }
}