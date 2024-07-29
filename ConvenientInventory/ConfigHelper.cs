namespace ConvenientInventory
{
    public static class ConfigHelper
    {
        public const string QuickStackRange_Default = "5";
        private const int QuickStackRange_DefaultInt = 5;
        private const int QuickStackRange_MinInt = 1;
        private const int QuickStackRange_MaxInt = 15;
        private const int QuickStackRange_LocationInt = 16;
        private const int QuickStackRange_GlobalInt = 17;
        private const string QuickStackRange_Location = "Location";
        private const string QuickStackRange_Global = "Global";

        /// <summary>
        /// Parses a config value for <see cref="ModConfig.QuickStackRange"/> and returns this value as an <see langword="int"/>
        /// constrained to the set of expected values.
        /// </summary>
        /// <param name="value">The config value to parse.</param>
        /// <returns>The parsed value, as an <see langword="int"/>.</returns>
        public static int ParseQuickStackRangeFromConfig(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                if (intValue < QuickStackRange_MinInt)
                {
                    intValue = QuickStackRange_MinInt;
                }
                else if (intValue > QuickStackRange_MaxInt)
                {
                    intValue = QuickStackRange_MaxInt;
                }

                // Return constrained int tile range.
                return intValue;
            }

            // Return int corresponding to the respective string value.
            return value switch
            {
                QuickStackRange_Location => QuickStackRange_LocationInt,
                QuickStackRange_Global => QuickStackRange_GlobalInt,
                _ => QuickStackRange_DefaultInt,
            };
        }

        /// <summary>
        /// Accepts an underlying int value for <see cref="ModConfig.QuickStackRange"/> and returns a corresponding string description of the value.
        /// </summary>
        /// <param name="value">The underlying int value to format.</param>
        /// <returns>The corresponding string description.</returns>
        public static string FormatQuickStackRange(int value)
        {
            if (QuickStackRange_MinInt <= value && value <= QuickStackRange_MaxInt)
            {
                // Return int tile range as string.
                return value.ToString();
            }

            // Return string corresponding to the respective underlying int value.
            return value switch
            {
                QuickStackRange_LocationInt => QuickStackRange_Location,
                QuickStackRange_GlobalInt => QuickStackRange_Global,
                _ => QuickStackRange_Default,
            };
        }
    }
}
