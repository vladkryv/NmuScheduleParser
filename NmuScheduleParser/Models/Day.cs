using System.Collections.Generic;
using System.Text;

namespace NmuScheduleParser.Models
{
    public class Day
    {
        public string Date { get; set; }
        public string DayOfWeek { get; set; }

        public List<Class> Classes { get; set; }

        public override string ToString()
        {
            if (Classes?.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append(Date);
                if (!string.IsNullOrWhiteSpace(DayOfWeek))
                {
                    sb.Append(" (");
                    sb.Append(DayOfWeek);
                    sb.Append(')');
                }
                sb.Append('\n');
                for (int i = 0; i < Classes.Count; i++)
                {
                    sb.Append('\n');
                    sb.Append(Classes[i].ToString());
                }
                return sb.ToString();
            }
            return string.Empty;
        }
    }
}