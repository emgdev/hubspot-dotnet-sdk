namespace HubSpot.Converters {
    public class IntTypeConverter : ITypeConverter
    {
        public bool TryConvertTo(string value, out object result)
        {
            result = default;

            if (value == null)
            {
                return true;
            }

            if (int.TryParse(value, out var output))
            {
                result = output;
                return true;
            }

            return false;
        }

        public bool TryConvertFrom(object value, out string result)
        {
            if (value == null)
            {
                result = null;
                return true;
            }

            if (value is int num)
            {
                result = num.ToString("D");
                return true;
            }

            if (int.TryParse(value.ToString(), out int numberResult))
            {
                result = numberResult.ToString("D");
                return true;
            }

            result = null;
            return false;
        }
    }
}