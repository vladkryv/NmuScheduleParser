using System.Collections.Generic;

namespace NmuScheduleParser
{
    public class Schedule
    {
        public List<Day> Days { get; internal set; }
    }

    public class Day
    {
        public string WeekName { get; internal set; }
        public string Date { get; internal set; }
        public List<Class> Classes { get; internal set; }
    }

    public class Class
    {
        public int Number { get; internal set; }
        public string StartTime { get; internal set; }
        public string EndTime { get; internal set; }
        public ClassInfo FirstClass { get; internal set; }
        public ClassInfo SecondClass { get; internal set; }

    }

    public class ClassInfo
    {
        public string Name { get; internal set; }
        public string Teacher { get; internal set; }
        public string Classroom { get; internal set; }
        public string UrlLink { get; internal set; }
    }
}