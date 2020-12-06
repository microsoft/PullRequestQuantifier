namespace PrQuantifier.Core.Model
{
    using System.Collections.Generic;

    public sealed class Context
    {
        /// <summary>
        /// Default constructor of Context;
        /// </summary>
        public Context()
        {
            Thresholds = new List<Threshold>();
        }

        /// <summary>
        /// Will have a list of expressions (paths,files similar to git ignore)
        /// to filter out all other files not part of the allowed list.
        /// If this will be empty everything will be allowed.
        /// </summary>
        public List<string> Allow { get; set; }

        /// <summary>
        /// Will have a list of expressions (paths,files similar to git ignore)
        /// to filter out all other files part of the ignored list.
        /// This is a reverse for the allowed. The idea to have both allowed and ignored is to allow users,
        /// in case not all files extensions or paths are known, to only allow specific paths/extensions
        /// (for example cc,cs extensions) in which case the ignore will be treat it like an empty list.
        /// </summary>
        public List<string> Ignore { get; set; }

        /// <summary>
        /// If empty all operations will be considered,
        /// otherwise if something specified.
        /// </summary>
        public List<GitOperationType> GitOperationType { get; set; }

        /// <summary>
        /// Thresholds for this model.
        /// </summary>
        public List<Threshold> Thresholds { get; set; }
    }
}
