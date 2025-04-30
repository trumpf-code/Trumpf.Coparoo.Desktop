using System;

namespace Trumpf.Coparoo.Desktop.CompactClassResolver
{
    /// <summary>  
    /// Thrown when a dependency cannot be resolved.  
    /// </summary>  
    public class ResolutionFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionFailedException"/> class.
        /// </summary>
        public ResolutionFailedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionFailedException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ResolutionFailedException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionFailedException"/> class 
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ResolutionFailedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}