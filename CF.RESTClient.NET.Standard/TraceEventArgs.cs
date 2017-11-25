using System;

namespace CF.RESTClientDotNet
{
    public class TraceEventArgs : EventArgs 
    {
        public string OperationName { get; set; }
        public DateTime Occurred { get; }
        public OperationState State { get; }
        //public string QueryString { get; }

        public TraceEventArgs(string operationName, OperationState state)
        {
            Occurred = DateTime.Now;
            OperationName = operationName;
            State = state;
            //QueryString = queryString;
        }

        public enum OperationState
        {
            Start,
            Complete
        }
    }
}
