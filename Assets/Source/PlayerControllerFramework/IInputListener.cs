public interface IInputListener<TAction> where TAction : struct
{
    public bool ValidateAction(TAction action);
    public void Listen(TAction action);
    
}
