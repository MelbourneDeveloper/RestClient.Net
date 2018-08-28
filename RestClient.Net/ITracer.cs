namespace RestClientDotNet
{
    public interface ITracer
    {
        void Trace(string operationName, OperationState state);
    }

    public enum OperationState
    {
        Start,
        Complete
    }
}
