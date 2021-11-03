using System.Text;

namespace NmuScheduleParser.Models
{
    public class ClassInfo
    {
        /// <summary>Discipline name, teacher, audience, meeting links, etc.\nReturns empty if none</summary>
        public string[] Description { get; set; }

        /// <summary>Example: дистанційно, змішана. Returns: null if non-remote; empty if error parse</summary>
        public string NameRemote { get; set; }

        public override string ToString()
        {
            if (Description?.Length > 0)
            {
                var textIndent = "        ";
                var sb = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(NameRemote))
                {
                    sb.Append(textIndent);
                    sb.Append("🏠 ");
                    sb.Append(NameRemote);
                    sb.Append('\n');
                }
                for (int i = 0; i < Description.Length; i++)
                {
                    sb.Append(textIndent);
                    sb.Append(Description[i]);
                    sb.Append('\n'); 
                }
                return sb.ToString();
            }

            return string.Empty;
        }
    }
}