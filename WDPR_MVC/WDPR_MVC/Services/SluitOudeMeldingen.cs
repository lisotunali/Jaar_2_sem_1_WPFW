using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WDPR_MVC.Data;

namespace WDPR_MVC.Services
{
    public class SluitOudeMeldingen : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<SluitOudeMeldingen> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public SluitOudeMeldingen(ILogger<SluitOudeMeldingen> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sluit Oude Meldingen Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation(
                "Sluit Oude Meldingen Service is working. Count: {Count}", count);

            SluitInactieveMeldingen();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sluit Oude Meldingen Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void SluitInactieveMeldingen()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<MyContext>();

                DateTime current = DateTime.Now.AddDays(-30);
                var meldingoud = _context.Meldingen
                    .Where(l => !l.IsClosed)
                    .Where(m => m.DatumAangemaakt < current && !m.IsClosed).ToList();

                foreach (var m in meldingoud)
                {
                    // Check de laatste comment datum?
                    var commentOokOud = m.Comments.OrderByDescending(x => x.DatumAangemaakt).FirstOrDefault();

                    if (commentOokOud == null || commentOokOud?.DatumAangemaakt < current)
                    {
                        m.IsClosed = true;
                    }
                }

                _context.SaveChanges();
            }
        }
    }
}
