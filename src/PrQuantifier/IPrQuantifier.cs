namespace PrQuantifier
{
    public interface IPrQuantifier
    {
        /// <summary>
        /// Quantifies based on the input.
        /// </summary>
        /// <param name="quantifierInput">Input quantifier.</param>
        /// <returns>returns a quantifier result.</returns>
        QuantifierResult Quantify(QuantifierInput quantifierInput);
    }
}
