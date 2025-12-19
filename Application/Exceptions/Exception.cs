using System;

namespace LangSaver.Application.Exceptions
{
    public class TranslationFailedException : Exception
    {

        public TranslationFailedException(string message) : base(message)
        {
            
        }
    }
    public class NotExistingWordException : Exception
    {

        public NotExistingWordException(string message) : base(message)
        {
            
        }
    }
}