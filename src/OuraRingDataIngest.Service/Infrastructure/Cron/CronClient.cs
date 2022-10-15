using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NCrontab;

namespace OuraRingDataIngest.Service.Infrastructure.Cron.CronClient
{
    public class CronClient : ICronClient
    {
        public async Task<CronInfo> AwaitNext(CancellationToken stoppingToken)
        {
            var s = CrontabSchedule.Parse(Environment.GetEnvironmentVariable("CRON"));
            var schedule = s.GetNextOccurrences(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1)).ToList();
            var next = s.GetNextOccurrence(DateTime.Now);
            var timespan = schedule[1] - schedule[0];
            var delay = next - DateTime.Now;
            await Task.Delay(delay, stoppingToken);

            return new CronInfo
            {
                Next = next,
                TimeSpan = timespan,
                Schedule = schedule,
                StartQueryDate = DateTime.SpecifyKind(DateTime.Now.Subtract(timespan), DateTimeKind.Local),
                EndQueryDate = DateTime.Now
            };
        }
    }

    public class CronInfo
    {
        public List<DateTime> Schedule { get; set; }
        public DateTime Next { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public DateTime? StartQueryDate { get; set; }
        public DateTime? EndQueryDate { get; set; }
    }
}