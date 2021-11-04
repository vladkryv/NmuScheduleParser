using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using NmuScheduleParser.Models;
using System;
using System.Collections.Generic;

namespace NmuScheduleParser
{
    public static class ScheduleParser
    {
        private const string TempDivider = "*|*";

        /// <summary>Returns null if the <b>rawHtml</b> is non-html</summary>
        public static Schedule GetSchedule(string rawHtml)
        {
            if (string.IsNullOrWhiteSpace(rawHtml)) return null;

            rawHtml = rawHtml.Replace("charset=windows-1251", string.Empty);
            var context = BrowsingContext.New(Configuration.Default);
            var htmlParser = context.GetService<IHtmlParser>();
            var document = htmlParser?.ParseDocument(rawHtml);
            var daySelector = "div.container div.row div.col-md-6:not(.col-xs-12)";
            var days = document?.QuerySelectorAll(daySelector);

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
                    classes.Add(rawClass);
            }
            return new Day { Date = date, DayOfWeek = dayOfWeek, Classes = classes };
        }

        private static Class ParseClass(IElement rawClass)
        {
            int numberClass = int.TryParse(rawClass.QuerySelector("td:nth-child(1)")?.TextContent, out numberClass) ? numberClass : 0;
            const int endTimeStartIndex = 5;
            string timeClass = rawClass.QuerySelector("td:nth-child(2)")?.TextContent.Insert(endTimeStartIndex, TempDivider);
            var durationClass = timeClass?.Split(TempDivider);
            var startTime = durationClass?.Length > 0 ? durationClass[0].Trim() : string.Empty;
            var endTime = durationClass?.Length > 1 ? durationClass[1].Trim() : string.Empty;
            ClassInfo firstClass = null;
            ClassInfo secondClass = null;


            var countClassInfo = rawClass.InnerHtml.CountSubstring("class=\"link\"");
            if (countClassInfo < 2)
                firstClass = ParseClassInfo(rawClass.QuerySelector("td:nth-child(3)"));
            else // two variant class
            {
                var classInfo = rawClass.QuerySelector("td:nth-child(3)");

                if (classInfo != null)
                {
                    string bak = classInfo.InnerHtml;
                    const string endFirstClass = "</div>";
                    var startIndex = classInfo.InnerHtml.IndexOf(endFirstClass, StringComparison.Ordinal);

                    if (startIndex != -1)
                    {
                        // remove second and parse
                        classInfo.InnerHtml = classInfo.InnerHtml[..(startIndex + endFirstClass.Length)];
                        firstClass = ParseClassInfo(classInfo);

                        // remove first and parse
                        classInfo.InnerHtml = bak[(startIndex + endFirstClass.Length)..];
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
                const string endTagRemote = "</span>";
                var endIndexRemote = rawClassInfo.InnerHtml.IndexOf(endTagRemote, StringComparison.Ordinal);
                if (endIndexRemote != -1)
                    rawClassInfo.InnerHtml = rawClassInfo.InnerHtml[(endIndexRemote + endTagRemote.Length)..];
            }

            rawClassInfo.InnerHtml = rawClassInfo.InnerHtml.Replace("<br>", TempDivider).Replace(" ауд.", TempDivider + "ауд.");
            var rawResult = rawClassInfo.TextContent.Split(TempDivider);
            for (var index = 0; index < rawResult.Length; index++)
            {
                var itemDescription = rawResult[index].Trim();
                if (!string.IsNullOrWhiteSpace(itemDescription))
                    classInfoDescription.Add(itemDescription.Replace("////", "//")); // Add and fix url if needed
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
