namespace PrQuantifier.Core.Exceptions
{
    using System;

    public sealed class ThresholdException : ArgumentException
    {
        public ThresholdException()
            : base()
        {
        }

        public ThresholdException(string message)
            : base(message)
        {
        }

        public ThresholdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
