

public interface ITransition
{
    // Changing states in to which state and what conditions
    IState To { get; }
    IPredicate Condition { get; }
}
