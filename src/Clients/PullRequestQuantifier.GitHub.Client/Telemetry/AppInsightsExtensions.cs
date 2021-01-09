namespace PullRequestQuantifier.GitHub.Client.Telemetry
***REMOVED***
    using System;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.ApplicationInsights;

    public static class AppInsightsExtensions
    ***REMOVED***
        /// <summary>
        /// Adds components for Application Performance Monitoring for a WebHost.
        /// This includes Application Insights support for:
        ///    - ILogger<T>: logs
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
        ***REMOVED***
            var instrumentationKey = GetInstrumentationKey(configuration);
            var options = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
            ***REMOVED***
                InstrumentationKey = instrumentationKey,
                AddAutoCollectedMetricExtractor = true,
                EnableAdaptiveSampling = false,
                DependencyCollectionOptions = ***REMOVED*** EnableLegacyCorrelationHeadersInjection = true ***REMOVED***,
                RequestCollectionOptions =
                ***REMOVED***
                    InjectResponseHeaders = true,
                    TrackExceptions = true
        ***REMOVED***
    ***REMOVED***;

            services.AddApplicationInsightsTelemetry(options);
            services.AddApmCommon(configuration, name, instrumentationKey);

            return services;
***REMOVED***

        private static IServiceCollection AddApmCommon(
            this IServiceCollection services,
            IConfiguration configuration,
            string name,
            string instrumentationKey)
        ***REMOVED***
            services.AddLogging(configuration, instrumentationKey);
            services.AddMetrics(name);

            return services;
***REMOVED***

        private static IServiceCollection AddLogging(
            this IServiceCollection services,
            IConfiguration configuration,
            string instrumentationKey)
        ***REMOVED***
            services.AddLogging(builder =>
            ***REMOVED***
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
    ***REMOVED***);

            return services;
***REMOVED***

        private static string GetInstrumentationKey(IConfiguration config)
        ***REMOVED***
            var instrumentationKey = config.GetSection("ApplicationInsights:InstrumentationKey").Value;
            return instrumentationKey;
***REMOVED***

        private static IServiceCollection AddMetrics(
            this IServiceCollection services,
            string applicationName)
        ***REMOVED***
            services.TryAddSingleton<IAppTelemetry>((IServiceProvider sp) =>
            ***REMOVED***
                return new AppTelemetry(
                    applicationName,
                    sp.GetRequiredService<TelemetryClient>());
    ***REMOVED***);

            return services;
***REMOVED***
***REMOVED***
***REMOVED***
