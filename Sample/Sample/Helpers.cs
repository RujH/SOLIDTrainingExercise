using System;

namespace Sample
{
    /// <summary>
    /// DIP implemented for this class
    /// </summary>
    public class Helpers : IHelpers
    {
        public decimal RoundUp(decimal input, int places)
        {
            decimal multiplier = Convert.ToDecimal(Math.Pow(10, Convert.ToDouble(places)));
            return Math.Ceiling(input * multiplier) / multiplier;
        }
    }
}
