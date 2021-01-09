namespace PullRequestQuantifier.GitHub.Client
***REMOVED***
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Program
    ***REMOVED***
        public static void Main(string[] args)
        ***REMOVED***
            CreateHostBuilder(args).Build().Run();
***REMOVED***

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                ***REMOVED***
                    webBuilder.UseStartup<Startup>();
        ***REMOVED***)
                .ConfigureServices((context, serviceCollection) =>
                ***REMOVED***
                    serviceCollection.AddHealthChecks();
                    serviceCollection.RegisterServices(context.Configuration);
        ***REMOVED***);
***REMOVED***
***REMOVED***
