namespace PullRequestQuantifier.GitHub.Client.Telemetry
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;
    using Microsoft.ApplicationInsights.Metrics;

    public sealed class AppTelemetry : IAppTelemetry, IDisposable
    ***REMOVED***
        private readonly TelemetryClient appInsights;

        private readonly string ns;

        public AppTelemetry(
            string @namespace,
            TelemetryClient appInsights)
        ***REMOVED***
            ns = @namespace;
            this.appInsights = appInsights;
***REMOVED***

        public string OperationId
        ***REMOVED***
            get ***REMOVED*** return Activity.Current?.Id; ***REMOVED***
***REMOVED***

        /// <summary>
        /// Record a single metric, with zero or more dimensions.
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="dimensions">Zero or more dimensions.</param>
        public void RecordMetric(
            string name,
            long value,
            params (string key, string value)[] dimensions)
        ***REMOVED***
            RecordMetric((name, value), dimensions);
***REMOVED***

        /// <summary>
        /// Record a single metric, with zero or more dimensions.
        /// </summary>
        /// <param name="metric"> The metric to be recorded.</param>
        /// <param name="dimensions">Zero or more dimensions.</param>
        public void RecordMetric((string name, long value) metric, params (string key, string value)[] dimensions)
        ***REMOVED***
            ValidateDimensions(dimensions);

            bool success = true;
            switch (dimensions.Length)
            ***REMOVED***
                case 0:
                    Record0(metric);
                    break;
                case 1:
                    success = Record1(metric, dimensions);
                    break;
                case 2:
                    success = Record2(metric, dimensions);
                    break;
                case 3:
                    success = Record3(metric, dimensions);
                    break;
                case 4:
                    success = Record4(metric, dimensions);
                    break;
                case 5:
                    success = Record5(metric, dimensions);
                    break;
                case 6:
                    success = Record6(metric, dimensions);
                    break;
                case 7:
                    success = Record7(metric, dimensions);
                    break;
                case 8:
                    success = Record8(metric, dimensions);
                    break;
                case 9:
                    success = Record9(metric, dimensions);
                    break;
                case 10:
                    success = Record10(metric, dimensions);
                    break;
                default:
                    success = Record10(metric, dimensions);
                    RecordNotSupported(metric, dimensions);
                    break;
    ***REMOVED***

            if (!success)
            ***REMOVED***
                appInsights.TrackTrace(
                    "Failed to track metric!",
                    SeverityLevel.Error,
                    new Dictionary<string, string>
                    ***REMOVED***
                        ***REMOVED*** "Namespace", ns ***REMOVED***,
                        ***REMOVED*** "MetricName", metric.name ***REMOVED***
            ***REMOVED***);
    ***REMOVED***
***REMOVED***

        /// <summary>
        /// A helper to create <see cref="IDisposable"/> from caller class
        /// Example:
        /// using (_metrics.StartOperation("methodName")) ***REMOVED******REMOVED***
        /// using (_metrics.StartOperation("methodName", parentOperationId)) ***REMOVED******REMOVED***
        ///
        /// if parentId is not passed in, it checks static instance AsyncLocal to find current activity,
        /// which is stored as local copy for each call (ExecutionContext) within a process.
        /// When parentId is not found, it assumes current call is the root.
        /// </summary>
        /// <typeparam name="T">Give an operation telemetry type.</typeparam>
        /// <param name="caller">The caller.</param>
        /// <param name="parentOperationId">Parent operation id.</param>
        /// <param name="operationName">Operation name.</param>
        /// <returns>returns an OperationScope.</returns>
        public OperationScope<T> StartOperation<T>(
            object caller,
            string parentOperationId = null,
            [CallerMemberName] string operationName = "")
            where T : OperationTelemetry, new()
        ***REMOVED***
            operationName = (caller == null) ? operationName : caller.GetType().Name + "." + operationName;

            return OperationScope<T>.StartOperation<T>(parentOperationId, operationName, appInsights);
***REMOVED***

        public void TrackDependency(
            string dependencyTypeName,
            string target,
            string dependencyName,
            string data,
            DateTimeOffset startTime,
            TimeSpan duration,
            string resultCode,
            bool success)
        ***REMOVED***
            appInsights.TrackDependency(dependencyTypeName, target, dependencyName, data, startTime, duration, resultCode, success);
***REMOVED***

        public void TrackEvent(string eventName, IDictionary<string, string> properties)
        ***REMOVED***
            appInsights.TrackEvent(eventName, properties);
***REMOVED***

        public void Dispose()
        ***REMOVED***
            appInsights?.Flush();
            Task.Delay(10000).Wait();
***REMOVED***

        private static void ValidateDimensions((string key, string value)[] dimensions)
        ***REMOVED***
            if ((dimensions == null) || (dimensions.Length == 0))
            ***REMOVED***
                return;
    ***REMOVED***

            var keys = new HashSet<string>();

            for (int i = 0; i < dimensions.Length; ++i)
            ***REMOVED***
                if (keys.Contains(dimensions[i].key))
                ***REMOVED***
                    throw new ArgumentException(
                        $"Duplicate key `***REMOVED***dimensions[i].key***REMOVED***` not allowed for metric dimensions!");
        ***REMOVED***

                if (string.IsNullOrEmpty(dimensions[i].value))
                ***REMOVED***
                    dimensions[i].value = "?";
        ***REMOVED***

                keys.Add(dimensions[i].key);
    ***REMOVED***
***REMOVED***

        private bool Record10((string n, long v) m, (string k, string v)[] d)
        ***REMOVED***
            var gm = appInsights.GetMetric(new MetricIdentifier(ns, m.n, d[0].k, d[1].k, d[2].k, d[3].k, d[4].k, d[5].k, d[6].k, d[7].k, d[8].k, d[9].k));
            return gm.TrackValue(m.v, d[0].v, d[1].v, d[2].v, d[3].v, d[4].v, d[5].v, d[6].v, d[7].v, d[8].v, d[9].v);
***REMOVED***

        private bool Record9((string n, long v) m, (string k, string v)[] d)
        ***REMOVED***
            var gm = appInsights.GetMetric(new MetricIdentifier(ns, m.n, d[0].k, d[1].k, d[2].k, d[3].k, d[4].k, d[5].k, d[6].k, d[7].k, d[8].k));
            return gm.TrackValue(m.v, d[0].v, d[1].v, d[2].v, d[3].v, d[4].v, d[5].v, d[6].v, d[7].v, d[8].v);
***REMOVED***

        private bool Record8((string n, long v) m, (string k, string v)[] d)
        ***REMOVED***
            var gm = appInsights.GetMetric(new MetricIdentifier(ns, m.n, d[0].k, d[1].k, d[2].k, d[3].k, d[4].k, d[5].k, d[6].k, d[7].k));
            return gm.TrackValue(m.v, d[0].v, d[1].v, d[2].v, d[3].v, d[4].v, d[5].v, d[6].v, d[7].v);
***REMOVED***

        private bool Record7((string n, long v) m, (string k, string v)[] d)
        ***REMOVED***
            var gm = appInsights.GetMetric(new MetricIdentifier(ns, m.n, d[0].k, d[1].k, d[2].k, d[3].k, d[4].k, d[5].k, d[6].k));
            return gm.TrackValue(m.v, d[0].v, d[1].v, d[2].v, d[3].v, d[4].v, d[5].v, d[6].v);
***REMOVED***

        private bool Record6((string n, long v) m, (string k, string v)[] d)
        ***REMOVED***
            var gm = appInsights.GetMetric(new MetricIdentifier(ns, m.n, d[0].k, d[1].k, d[2].k, d[3].k, d[4].k, d[5].k));
            return gm.TrackValue(m.v, d[0].v, d[1].v, d[2].v, d[3].v, d[4].v, d[5].v);
***REMOVED***

        private bool Record5((string n, long v) m, (string k, string v)[] d)
        ***REMOVED***
            var gm = appInsights.GetMetric(new MetricIdentifier(ns, m.n, d[0].k, d[1].k, d[2].k, d[3].k, d[4].k));
            return gm.TrackValue(m.v, d[0].v, d[1].v, d[2].v, d[3].v, d[4].v);
***REMOVED***

        private bool Record4((string n, long v) m, (string k, string v)[] d)
        ***REMOVED***
            var gm = appInsights.GetMetric(new MetricIdentifier(ns, m.n, d[0].k, d[1].k, d[2].k, d[3].k));
            return gm.TrackValue(m.v, d[0].v, d[1].v, d[2].v, d[3].v);
***REMOVED***

        private bool Record3((string name, long value) metric, (string key, string value)[] d)
        ***REMOVED***
            var m = appInsights.GetMetric(new MetricIdentifier(ns, metric.name, d[0].key, d[1].key, d[2].key));
            return m.TrackValue(metric.value, d[0].value, d[1].value, d[2].value);
***REMOVED***

        private bool Record2((string name, long value) metric, (string key, string value)[] dimensions)
        ***REMOVED***
            var m = appInsights.GetMetric(new MetricIdentifier(ns, metric.name, dimensions[0].key, dimensions[1].key));
            return m.TrackValue(metric.value, dimensions[0].value, dimensions[1].value);
***REMOVED***

        private bool Record1((string name, long value) metric, (string key, string value)[] dimensions)
        ***REMOVED***
            var m = appInsights.GetMetric(new MetricIdentifier(ns, metric.name, dimensions[0].key));
            return m.TrackValue(metric.value, dimensions[0].value);
***REMOVED***

        private void Record0((string name, long value) metric)
        ***REMOVED***
            var m = appInsights.GetMetric(new MetricIdentifier(ns, metric.name));
            m.TrackValue(metric.value);
***REMOVED***

        private void RecordNotSupported((string name, long value) metric, (string key, string value)[] dimensions)
        ***REMOVED***
            var m = appInsights.GetMetric(new MetricIdentifier(ns, "TooManyMetricDimensions", "MetricName", "DimensionsCount"));
            m.TrackValue(1, ns + metric.name, dimensions.Length.ToString());
***REMOVED***
***REMOVED***
***REMOVED***
