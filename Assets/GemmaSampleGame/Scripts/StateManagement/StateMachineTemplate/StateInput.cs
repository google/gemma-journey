namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
  public abstract class StateInput
  {
    public static InputType MyConvertTo<InputType>(StateInput input) where InputType : StateInput
    {
      return (InputType)input;
    }
  }
}
