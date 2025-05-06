using System;

public class ChangeStateFailedException : StateMachineException {
  public ChangeStateFailedException() {

  }

  public ChangeStateFailedException(string message) : base(message) {

  }

  public ChangeStateFailedException(string message, Exception inner) : base(message, inner) {

  }
}
