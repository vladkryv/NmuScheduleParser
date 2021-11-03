using System.Collections.Generic;
using System.Text;

namespace NmuScheduleParser.Models
{
    public class Schedule
    {
        /// <summary>Returns an empty list if there are no days</summary>
        public List<Day> Days { get; set; }
        
        public override string ToString()
        {
            if (Days?.Count > 0)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < Days?.Count; i++)
                {
                    sb.Append(Days[i]);
                    if (i < Days.Count - 1)
                        sb.Append("\n\n");
                }

                return sb.ToString();
            }
            return string.Empty;
        }
    }
}