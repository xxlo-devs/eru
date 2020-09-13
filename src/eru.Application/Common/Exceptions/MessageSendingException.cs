using System;

namespace eru.Application.Common.Exceptions
{
    public class MessageSendingException : Exception
    {
        public MessageSendingException(string message) : base($"An exception was thrown while sending a message: {message}")
        {
            
        }
    }
}