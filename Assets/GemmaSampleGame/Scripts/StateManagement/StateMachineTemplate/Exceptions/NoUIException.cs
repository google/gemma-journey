using System;

[System.Serializable]
public class NoUIException : System.Exception {
  public NoUIException() { }
  public NoUIException(string message) : base(message) { }
  public NoUIException(string message, System.Exception inner) : base(message, inner) { }
  public NoUIException(Type stateType, Type expectedUserInterfaceType) : base(FormatMessage(stateType, expectedUserInterfaceType)) { }
  public NoUIException(Type stateType, Type expectedUserInterfaceType, string message) : base(FormatMessage(stateType, expectedUserInterfaceType, message)) { }
  public NoUIException(Type stateType, Type expectedUserInterfaceType, string message, System.Exception inner) : base(FormatMessage(stateType, expectedUserInterfaceType, message), inner) { }

  private static string FormatMessage(Type stateType, Type expectedUserInterfaceType) {
    return string.Format("{1} not presented in {0}", stateType, expectedUserInterfaceType);
  }

  private static string FormatMessage(Type stateType, Type expectedUserInterfaceType, string message) {
    return string.Format("{1} not presented in {0}: {2}", stateType, expectedUserInterfaceType, message);
  }
}
