using System.Text;

namespace NmuScheduleParser.Models
{
    /// <summary>A student <b>class</b> is two academic hours of classes in a row.</summary>
    public class Class
    {
        public int Number { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public ClassInfo FirstClass { get; set; }

        /// <summary>Returns null if none.</summary>
        public ClassInfo SecondClass { get; set; }

        public override string ToString()
        {
            if (FirstClass != null)
            {
                var textIndent = "    ";
                var sb = new StringBuilder();
                sb.Append(textIndent);
                sb.Append(Number);
                sb.Append(" class (");
                sb.Append(StartTime);
                sb.Append(" - ");
                sb.Append(EndTime);
                sb.Append(")\n");
                sb.Append(FirstClass.ToString());
                if (SecondClass != null)
                {
                    sb.Append(textIndent);
                    sb.Append(textIndent);
                    sb.Append("------------------------------------------\n");
                    sb.Append(SecondClass.ToString());
                }
                return sb.ToString();
            }
            return "";
        }
    }
}