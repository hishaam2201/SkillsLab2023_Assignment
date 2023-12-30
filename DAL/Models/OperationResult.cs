using System.Collections.Generic;

namespace DAL.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; } = new List<string>();

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
