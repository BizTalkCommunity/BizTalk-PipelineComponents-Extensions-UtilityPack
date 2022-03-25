using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Blogical.Shared.PipelineComponents.Exceptions
{
    [Serializable]
    public class ArchiveException : System.Exception
    {
        public ArchiveException() { }
        public ArchiveException(string message)
            : base(message)
        { }
        public ArchiveException(string message, Exception innerException)
            : base(message, innerException)
        { }
        protected ArchiveException(SerializationInfo serializationInfo, StreamingContext streamingContext){ }
    }

}
