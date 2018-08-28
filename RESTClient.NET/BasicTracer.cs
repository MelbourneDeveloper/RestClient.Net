using System;
using System.Collections.Generic;

namespace RestClientDotNet
{
    public class BasicTracer : ITracer
    {
        #region Fields
        private readonly List<BasicTraceInfo> _TraceInfos = new List<BasicTraceInfo>();
        private readonly Dictionary<string, DateTime> _OperationStartTimes = new Dictionary<string, DateTime>();
        #endregion

        #region Public Properties
        public List<BasicTraceInfo> TraceInfos => _TraceInfos;
        #endregion

        #region Constructor
        public void Trace(string operationName, OperationState state)
        {
            switch (state)
            {
                case OperationState.Start:
                    _OperationStartTimes.Add(operationName, DateTime.Now);
                    break;
                case OperationState.Complete:
                    _TraceInfos.Add(new BasicTraceInfo { OperationName = operationName, CompleteTime = DateTime.Now, StartTime = _OperationStartTimes[operationName] });
                    break;
            }
        }
        #endregion

        #region Public Methods
        public void Clear()
        {
            _TraceInfos.Clear();
            _OperationStartTimes.Clear();
        }
        #endregion
    }

    public class BasicTraceInfo
    {
        public string OperationName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CompleteTime { get; set; }
    }
}
