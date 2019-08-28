using System;

namespace In.SwDoc.Generator
{
    public class DocumentGenerationException : Exception
    {
        public DocumentGenerationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}