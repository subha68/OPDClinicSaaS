namespace Helpers
{
    public static class NumberToWords
    {
        public static string Convert(decimal amount)
        {
            long rupees = (long)Math.Floor(amount);
            int paise = (int)((amount - rupees) * 100);

            string result = $"{ConvertToWords(rupees)} Rupees";

            if (paise > 0)
                result += $" and {ConvertToWords(paise)} Paise";

            return result + " Only";
        }

        private static string ConvertToWords(long number)
        {
            if (number == 0)
                return "Zero";

            string[] units =
            {
                "", "One", "Two", "Three", "Four", "Five",
                "Six", "Seven", "Eight", "Nine", "Ten",
                "Eleven", "Twelve", "Thirteen", "Fourteen",
                "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
            };

            string[] tens =
            {
                "", "", "Twenty", "Thirty", "Forty",
                "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
            };

            if (number < 20)
                return units[number];

            if (number < 100)
                return tens[number / 10] + ((number % 10 > 0) ? " " + units[number % 10] : "");

            if (number < 1000)
                return units[number / 100] + " Hundred " +
                    ((number % 100 > 0) ? ConvertToWords(number % 100) : "");

            if (number < 100000)
                return ConvertToWords(number / 1000) + " Thousand " +
                    ((number % 1000 > 0) ? ConvertToWords(number % 1000) : "");

            if (number < 10000000)
                return ConvertToWords(number / 100000) + " Lakh " +
                    ((number % 100000 > 0) ? ConvertToWords(number % 100000) : "");

            return ConvertToWords(number / 10000000) + " Crore " +
                ((number % 10000000 > 0) ? ConvertToWords(number % 10000000) : "");
        }
    }
}