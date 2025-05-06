using System;

public class StateMachineException : Exception {
  public StateMachineException() : base() {

  }

  public StateMachineException(string message) : base(message) {

  }

  public StateMachineException(string message, Exception inner) : base(message, inner) {

  }
}
