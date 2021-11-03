using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NmuScheduleParser.Models;

namespace NmuScheduleParser
{
    public static class ScheduleParser
    {
        /// <summary>Returns null if the <b>rawHtml</b> is non-html</summary>
        public static async Task<Schedule> GetScheduleAsync(string rawHtml)
        {
            if (string.IsNullOrWhiteSpace(rawHtml)) return null;

            rawHtml = rawHtml.Replace("charset=windows-1251", String.Empty);
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            IDocument document;

            try { document = await context.OpenAsync(r => r.Content(rawHtml)); }
            catch (Exception) { return null; }

            var daySelector = "div.container div.row div.col-md-6:not(.col-xs-12)";
            var days = document.QuerySelectorAll(daySelector);

            return new Schedule { Days = ParseRangeDay(days) };
        }

        private static List<Day> ParseRangeDay(IHtmlCollection<IElement> days)
        {
            var result = new List<Day>();

            for (int i = 0; i < days?.Length; i++)
                result.Add(ParseDay(days[i]));

            return result;
        }

        private static Day ParseDay(IElement rawDay)
        {
            var dateAndDayOfWeek = rawDay.QuerySelector("h4")?.TextContent.Split(" ");
            var date = dateAndDayOfWeek?[0];
            string dayOfWeek = null;
            if (dateAndDayOfWeek?.Length > 1)
                dayOfWeek = dateAndDayOfWeek[1];

            var classes = new List<Class>();
            var rawClasses = rawDay.QuerySelectorAll("tr");

            for (int i = 0; i < rawClasses.Length; i++)
            {
                var rawClass = ParseClass(rawClasses[i]);
                if (rawClass.FirstClass?.Description.Length > 0)
                {
                    classes.Add(rawClass);
                }
            }
            return new Day { Date = date, DayOfWeek = dayOfWeek, Classes = classes };
        }

        private static Class ParseClass(IElement rawClass)
        {
            int.TryParse(rawClass.QuerySelector("td:nth-child(1)")?.TextContent, out var numberClass);
            var endTimeStartIndex = 5;
            var divider = "*|*";
            string timeClass = rawClass.QuerySelector("td:nth-child(2)")?.TextContent.Insert(endTimeStartIndex, divider);
            var startTime = timeClass?.Split(divider)[0].Trim();
            var endTime = timeClass?.Split(divider)[1].Trim();
            ClassInfo firstClass = null;
            ClassInfo secondClass = null;


            var countClassInfo = rawClass.InnerHtml.CountSubstring("class=\"link\"");
            if (countClassInfo < 2)
                firstClass = ParseClassInfo(rawClass.QuerySelector("td:nth-child(3)"));
            else // two variant class
            {
                var classInfo = rawClass.QuerySelector("td:nth-child(3)");
                string tmp = classInfo?.InnerHtml;

                if (classInfo != null)
                {
                    var startIndex = classInfo.InnerHtml.IndexOf("</div>", StringComparison.Ordinal);

                    if (startIndex != -1)
                    {
                        // remove second and parse
                        classInfo.InnerHtml = classInfo.InnerHtml[..(startIndex + 6)]; // 6 = "</div>".Length
                        firstClass = ParseClassInfo(classInfo);

                        // remove first and parse
                        classInfo.InnerHtml = tmp[(startIndex + 6)..]; // 6 = "</div>".Length
                        secondClass = ParseClassInfo(classInfo);
                    }
                    else
                        firstClass = ParseClassInfo(rawClass.QuerySelector("td:nth-child(3)"));
                }
            }

            return new Class
            {
                StartTime = startTime, EndTime = endTime, Number = numberClass,
                FirstClass = firstClass, SecondClass = secondClass
            };
        }

        private static ClassInfo ParseClassInfo(IElement rawClassInfo)
        {
            var classInfoDescription = new List<string>();
            string nameRemote = null;
            if (rawClassInfo.InnerHtml.Contains("class=\"remote_work\""))
            {
                nameRemote = rawClassInfo.QuerySelector("span.remote_work")?.TextContent;
                var endIndexRemote = rawClassInfo.InnerHtml.IndexOf("</span>", StringComparison.Ordinal);
                if (endIndexRemote != -1)
                {
                    rawClassInfo.InnerHtml = rawClassInfo.InnerHtml[(endIndexRemote + 7)..]; // 7 = "</span>".Length
                }
            }

            var divider = "*|*";
            rawClassInfo.InnerHtml = rawClassInfo.InnerHtml.Replace("<br>", divider).Replace(" ауд.", divider + "ауд.");
            var rawResult = rawClassInfo.TextContent.Split(divider);
            for (var index = 0; index < rawResult.Length; index++)
            {
                var item = rawResult[index];
                var tmp = item.Trim();
                if (!string.IsNullOrWhiteSpace(tmp))
                    classInfoDescription.Add(tmp.Replace("////", "//")); // Add and fix url if needed
            }

            return new ClassInfo { Description = classInfoDescription.ToArray(), NameRemote = nameRemote };
        }
    }

    internal static class StringExtensions
    {
        public static int CountSubstring(this string value, string substring)
        {
            if (string.IsNullOrEmpty(substring))
                return 0;
            return (value.Length - value.Replace(substring, string.Empty).Length) / substring.Length;
        }
    }
}
