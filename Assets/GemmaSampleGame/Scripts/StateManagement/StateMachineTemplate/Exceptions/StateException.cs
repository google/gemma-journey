using System;

[Serializable]
public class StateException : Exception {
  public StateException() { }
  public StateException(string message) : base(message) { }
  public StateException(string message, System.Exception inner) : base(message, inner) { }
  protected StateException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
