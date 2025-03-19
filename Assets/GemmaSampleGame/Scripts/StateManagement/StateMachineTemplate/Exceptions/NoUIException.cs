// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
