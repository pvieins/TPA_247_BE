namespace PVI.Helper
{
    public class ConvertDateTime
    {
        public static DateTime? ConverDateVN(string textValue)
        {
            DateTime dateTime = DateTime.MinValue;
            try
            {
                //mm/dd/yyyy hh:mm:ss MM/DD/YYYY HH:MM
                string from = textValue.Replace("\"", "");// "24/5/2009 3:40:00 AM";                
                dateTime = DateTime.ParseExact(from, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                return dateTime;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static string ConvertSmallDateTimeToString(DateTime? dateTime)
        {
            // Check if dateTime is null
            if (dateTime == null)
            {
                return null;
            }

            // Convert to "dd-MM-yyyy" format
            string formattedDate = dateTime.Value.ToString("dd/MM/yyyy");
            return formattedDate;
        }

        public string ConvertSmalldatetimeToString(DateTime smalldatetimeValue)
        {
            // Define the format you want for the string representation
            string format = "yyyy-MM-dd HH:mm:ss";

            // Convert the DateTime to a string using the specified format
            string result = smalldatetimeValue.ToString(format);

            return result;
        }


        public static DateTime? ConverDateTimeVN(string textValue)
        {
            if (string.IsNullOrWhiteSpace(textValue))
            {
                return null;
            }

            DateTime dateTime = DateTime.MinValue;
            //if (DateTime.TryParse(textValue, out dateTime))
            //{
            //    return dateTime;
            //}

            // If parsing as ISO 8601 fails, try your previous date formats
            try
            {
                string from = textValue.Replace("\"", "");
                if (textValue.Length > 12)
                {
                    dateTime = DateTime.ParseExact(from, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    dateTime = DateTime.ParseExact(from, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }

                return dateTime;
            }
            catch (Exception ex)
            {
                // Handle parsing errors as needed
               
            }

            return null;
        }
        
    }
}
