﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NmuScheduleParser;

namespace NSParserExample
{
    internal class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			MainAsync().GetAwaiter().GetResult();
		}

		static async Task MainAsync()
		{
			var htmlRaw = await DownloadHtmlSchedule(@"http://nmu.npu.edu.ua", "42ІПЗ");
			var schedule = await ScheduleParser.GetScheduleAsync(htmlRaw);
			/* 
				Schedule:
					List<Day> Days

				Day: 
					string Date
					string DayOfWekk
					List<Class> Classes

				Class:
					string StartTime
					string EndTime
					ClassInfo FirstClass
					ClassInfo SecondClass

				ClassInfo:
					string[] Description
					string NameRemote
			*/

			Console.WriteLine(schedule);
			int classCount = 0;
			foreach (var day in schedule?.Days)
			{
				classCount += day.Classes.Count;
			}
			var classAllMinutes = classCount * 80;
			Console.WriteLine("\nClass count: {0}, It's {1} hours and {2} minutes", classCount, classAllMinutes / 60, classAllMinutes % 60);

            Console.WriteLine("\nPress any key...");
			Console.ReadKey();
		}

		static async Task<string> DownloadHtmlSchedule(string domain, string groupName, string startDate = "", string endDate = "")
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Encoding EncodingWin1251 = Encoding.GetEncoding("windows-1251");
			groupName = HttpUtility.UrlEncode(groupName, EncodingWin1251);

			var bodyParam = string.Format("group={0}&sdate={1}&edate={2}", groupName, startDate, endDate);
			var body = Encoding.UTF8.GetBytes(bodyParam);
            HttpClient client = new HttpClient { BaseAddress = new Uri(domain) };

            //n=700 should be as url parameter, otherwise it doesn't work
            var response = await client.PostAsync(@$"cgi-bin/timetable.cgi?n=700", new ByteArrayContent(body));

			var responseContentBytes = await response.Content.ReadAsByteArrayAsync();
			return EncodingWin1251.GetString(responseContentBytes);
		}
	}
}