namespace PullRequestQuantifier.Common.Azure.Telemetry
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.ApplicationInsights;

    [ExcludeFromCodeCoverage]
    public static class AppInsightsExtensions
    {
        /// <summary>
        /// Adds components for Application Performance Monitoring for a WebHost.
        /// This includes Application Insights support for:
        ///    - ILogger.<T>: logs
        ///    - IAppTelemetry: metrics
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <param name="name">The application name to tag logs/metrics with</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddApmForWebHost(
            this IServiceCollection services,
            IConfiguration configuration,
            string name)
        {
            var instrumentationKey = GetInstrumentationKey(configuration);
            var options = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
            {
                InstrumentationKey = instrumentationKey,
                AddAutoCollectedMetricExtractor = true,
                EnableAdaptiveSampling = false,
                DependencyCollectionOptions = { EnableLegacyCorrelationHeadersInjection = true },
                RequestCollectionOptions =
                {
                    InjectResponseHeaders = true,
                    TrackExceptions = true
                }
            };

            services.AddApplicationInsightsTelemetry(options);
            services.AddApmCommon(configuration, name, instrumentationKey);

            return services;
        }

        private static IServiceCollection AddApmCommon(
            this IServiceCollection services,
            IConfiguration configuration,
            string name,
            string instrumentationKey)
        {
            services.AddLogging(configuration, instrumentationKey);
            services.AddMetrics(name);

            return services;
        }

        private static IServiceCollection AddLogging(
            this IServiceCollection services,
            IConfiguration configuration,
            string instrumentationKey)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();

                var loggingConfig = configuration.GetSection("Logging");
                builder.AddConfiguration(loggingConfig);

                builder.AddConsole();

                builder.AddApplicationInsights(instrumentationKey);

                var logLevelConfig = loggingConfig.GetSection("LogLevel");
                builder
                    .AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information)
                    .AddFilter<ApplicationInsightsLoggerProvider>(
                        "Microsoft.AspNetCore.Mvc",
                        logLevelConfig.GetValue("Microsoft.AspNetCore.Mvc", LogLevel.Warning))
                    .AddFilter<ApplicationInsightsLoggerProvider>(
                        "Microsoft.AspNetCore.Hosting.Internal.WebHost",
                        logLevelConfig.GetValue("Microsoft.AspNetCore.Hosting.Internal.WebHost", LogLevel.Warning));
            });

            return services;
        }

        private static string GetInstrumentationKey(IConfiguration config)
        {
            var instrumentationKey = config.GetSection("ApplicationInsights:InstrumentationKey").Value;
            return instrumentationKey;
        }

        private static IServiceCollection AddMetrics(
            this IServiceCollection services,
            string applicationName)
        {
            services.TryAddSingleton<IAppTelemetry>((IServiceProvider sp) =>
            {
                return new AppTelemetry(
                    applicationName,
                    sp.GetRequiredService<TelemetryClient>());
            });

            return services;
        }
    }
}
