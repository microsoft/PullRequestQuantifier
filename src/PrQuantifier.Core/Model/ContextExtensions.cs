namespace PrQuantifier.Core.Model
***REMOVED***
    using System.Linq;
    using PrQuantifier.Core.Exceptions;

    public static class ContextExtensions
    ***REMOVED***
        /// <summary>
        /// Validate the context, make sure the thresholds don't overlap,
        /// labels are distinct.
        /// </summary>
        /// <param name="context">The context to validate.</param>
        /// <returns>returns true in case is valid, otherwise false and errors will not be empty.</returns>
        public static Context Validate(this Context context)
        ***REMOVED***
            if (context.Thresholds == null 
                || context.Thresholds.Count == 0)
            ***REMOVED***
                throw new ThresholdException("No labels/thresholds were defined!");
    ***REMOVED***

            // we have to have at least 3 labels/thresholds defined
            if (context.Thresholds.Count < 3)
            ***REMOVED***
                throw new ThresholdException("At least 3 labels/thresholds should be defined!");
    ***REMOVED***

            // check all values are percentiles
            var maxValue = context.Thresholds.Max(t => t.Value);
            var minValue = context.Thresholds.Min(t => t.Value);

            if (maxValue > 100 
                || maxValue < 0 
                || minValue > 100 
                || minValue < 0)
            ***REMOVED***
                throw new ThresholdException("All values should be between 0 and 100!");
    ***REMOVED***

            // validate thresholds are not the same
            var sortedThresholds = context.Thresholds.OrderBy(t => t.Value).ToArray();
            var prev = sortedThresholds[0].Value;

            for (var i = 1; i < sortedThresholds.Length; i++)
            ***REMOVED***
                if (sortedThresholds[i].Value != prev)
                ***REMOVED***
                    prev = sortedThresholds[i].Value;
                    continue;
        ***REMOVED***

                // we don't allow 2 upper bound thresholds to have the same value
                throw new ThresholdException("At least two upper bound thresholds have the same values!");
    ***REMOVED***

            // validate labels are not empty
            if (context.Thresholds.Any(t => string.IsNullOrWhiteSpace(t.Label)))
            ***REMOVED***
                throw new ThresholdException("You can't have empty labels!");
    ***REMOVED***

            // todo regex/paths validation

            return context;
***REMOVED***
***REMOVED***
***REMOVED***
