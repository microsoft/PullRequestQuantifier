using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Coral.PolicyCompliance.Service.Tests")]

namespace PullRequestQuantifier.Repository.Service
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    internal sealed class WorkerService : BackgroundService
    {
        public WorkerService()
        {
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Run(() => { }, stoppingToken);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
