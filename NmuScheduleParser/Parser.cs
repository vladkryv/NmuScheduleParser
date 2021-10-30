using AngleSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NmuScheduleParser
{
    public class Parser
    {
        public async Task<Schedule> GetSchedule(string rawHtmlCode)
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            var document = await context.OpenAsync(r => r.Content(rawHtmlCode));

            var cellSelector = "div.container div.row div.col-md-6:not(.col-xs-12)";
            var cells = document.QuerySelectorAll(cellSelector);
            
            return null;
        }
    }
}
