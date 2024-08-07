using System;
using System.Runtime.Serialization;

namespace ModelViewerWPF;

[Serializable]
public class ModelViewerException : Exception
{
    public ModelViewerException() { }

    public ModelViewerException(string message)
        : base(message) { }

    public ModelViewerException(string message, Exception inner)
        : base(message, inner) { }

    protected ModelViewerException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
