using System.Runtime.Serialization;

namespace ThGameMgr.Lite.Exceptions
{
    internal class ProcessNotFoundException : Exception
    {
        public ProcessNotFoundException() : base() { }
        public ProcessNotFoundException(string message) : base(message) { }
        public ProcessNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected ProcessNotFoundException(SerializationInfo info, StreamingContext context) { }
    }
}
