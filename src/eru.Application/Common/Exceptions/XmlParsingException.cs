using System;
using System.Reflection;

namespace eru.Application.Common.Exceptions
{
    public class XmlParsingException : Exception
    {
        public XmlParsingException(MemberInfo type, string message) :
            base($"Given xml file is not a valid xml file or cannot be parsed into {type.Name}. {message}")
        {
        }
    }
}