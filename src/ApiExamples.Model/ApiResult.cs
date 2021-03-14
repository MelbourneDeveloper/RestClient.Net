using System.Collections.Generic;

#pragma warning disable CA1002
#pragma warning disable CS8618

namespace ApiExamples.Model
{
    public class ApiResult
    {
        public string Data { get; set; }
        public List<string> Messages { get; } = new();
        public List<string> Errors { get; } = new();
    }
}
