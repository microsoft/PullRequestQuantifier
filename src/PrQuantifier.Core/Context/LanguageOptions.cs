namespace PrQuantifier.Core.Context
{
    public sealed class LanguageOptions
    {
        /// <summary>
        /// Gets  or sets a value indicating whether gets or sets of we should ignore spaces or not when we quantify.
        /// </summary>
        public bool IgnoreSpaces { get; set; }

        /// <summary>
        /// Gets  or sets a value indicating whether we should ignore comments or not when we quantify.
        /// </summary>
        public bool IgnoreComments { get; set; }

        /// <summary>
        /// Gets  or sets a value indicating whether we should ignore code block separator or not when we quantify.
        /// </summary>
        public bool IgnoreCodeBlockSeparator { get; set; }
    }
}
