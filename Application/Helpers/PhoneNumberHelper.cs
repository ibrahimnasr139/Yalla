namespace Application.Helpers
{
    public static class PhoneNumberHelper
    {
        private const string CountryCode = "20";
        public static string? NormalizeEgyptMobile(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw) ||
                !raw.All(c => char.IsDigit(c) || c is '+' or ' ' or '-'))
                return null;

            var digits = new string(raw.Where(char.IsDigit).ToArray());

            if (digits.StartsWith("00"))
                digits = digits[2..];

            if (digits.StartsWith("20"))
                digits = digits[2..];
            else if (digits.StartsWith('0'))
                digits = digits[1..];

            if (digits.Length != 10 || digits[0] != '1' ||
                digits[1] is not ('0' or '1' or '2' or '5'))
                return null;

            return CountryCode + digits;
        }
    }
}