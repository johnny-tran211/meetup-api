using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI.Filters
{
    public class TimeTrackFilter : IActionFilter
    {
        private Stopwatch _stopwatch;
        private readonly ILogger<TimeTrackFilter> _logger;
        public TimeTrackFilter(ILogger<TimeTrackFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();

            var milliseconds = _stopwatch.ElapsedMilliseconds;
            var action = context.ActionDescriptor.DisplayName;

            _logger.LogInformation($"Action [{action}], executed in: {milliseconds} milliseconds");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }
    }
}
