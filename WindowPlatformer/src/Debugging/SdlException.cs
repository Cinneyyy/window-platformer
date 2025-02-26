using System;

namespace src.Debugging;

[Serializable]
public class SdlException : Exception
{
    public SdlException() { }
    public SdlException(string message) : base(message) { }
    public SdlException(string message, Exception inner) : base(message, inner) { }

    [Obsolete("Je ne sais pas")]
    protected SdlException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}