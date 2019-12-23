using System.Collections.Generic;

namespace form_builder.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, string> ToNormaliseDictionary(this Dictionary<string, string[]> formData)
        {
            var normalisedFormData = new Dictionary<string, string>();
            foreach (var item in formData)
            {
                if (item.Value.Length == 1)
                {
                    if (item.Key.EndsWith("-address") && !string.IsNullOrEmpty(item.Value[0]))
                    {
                        string[] addressDetails = item.Value[0].Split('|');
                        if (!string.IsNullOrEmpty(addressDetails[0]))
                        {
                            normalisedFormData.Add($"{item.Key}", addressDetails[0]);
                        }
                        if (!string.IsNullOrEmpty(addressDetails[1]))
                        {
                            normalisedFormData.Add($"{item.Key}-description", addressDetails[1]);
                        }
                    }
                    else if (item.Key.EndsWith("-streetaddress") && !string.IsNullOrEmpty(item.Value[0]))
                    {
                        string[] streetDetails = item.Value[0].Split('|');
                        if (!string.IsNullOrEmpty(streetDetails[0]))
                        {
                            normalisedFormData.Add($"{item.Key}", streetDetails[0]);
                        }
                        if (!string.IsNullOrEmpty(streetDetails[1]))
                        {
                            normalisedFormData.Add($"{item.Key}-description", streetDetails[1]);
                        }
                    }
                    else
                    {
                        normalisedFormData.Add(item.Key, item.Value[0]);
                    }
                }
                else
                {
                    normalisedFormData.Add(item.Key, string.Join(", ", item.Value));
                }
            }

            return normalisedFormData;
        }
    }
}