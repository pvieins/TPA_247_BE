

namespace PVI.Helper
{
    public class UpdateProperties
    {
        public void UpdateProperty(object source, object target, List<string> dateTimePropertiesToConvert)
        {
            var sourceProperties = source.GetType().GetProperties();
            var targetProperties = target.GetType().GetProperties();

            // Define a list of property names that should be converted to DateTime


            foreach (var sourceProperty in sourceProperties)
            {
                var targetProperty = targetProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);

                if (targetProperty != null)
                {
                    var sourceValue = sourceProperty.GetValue(source);

                    // Check if the property name is in the list of properties to convert
                    if (dateTimePropertiesToConvert.Contains(sourceProperty.Name) && sourceValue != null)
                    {

                        DateTime? dateTimeValue = ConvertDateTime.ConverDateTimeVN(sourceValue.ToString());

                        targetProperty.SetValue(target, dateTimeValue);
                    }
                    else if (sourceValue != null) // Check if sourceValue is not null
                    {

                        targetProperty.SetValue(target, sourceValue);
                    }
                }
            }
        }

        public void AddEntityContent(WordToPdfRequest wordPdfRequest, string keyCode, string value)
        {
            var obj = new EntityContent
            {
                KeyCode = keyCode,
                Value = value
            };

            wordPdfRequest.ListData.Add(obj);
        }
    }
}
