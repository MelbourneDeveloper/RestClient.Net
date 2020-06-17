using System.Collections.Generic;

namespace ApiExamples.Model
{
    public class ApiResult
    {
        public string Data { get; set; }
        public List<string> Messages { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();
    }
}
