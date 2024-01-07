using System.Collections.Generic;

namespace DAL.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public object Data { get; set; }
        public IEnumerable<object> ListOfData { get; set; }

        public string GetFormattedMessages()
        {
            if (Messages != null)
            {
                return string.Join(", ", Messages);
            }
            return string.Empty;
        }
    }
}
