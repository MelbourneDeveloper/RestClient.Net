namespace CF.RESTClientDotNet
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
