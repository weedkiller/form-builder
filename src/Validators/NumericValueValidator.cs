using System.Collections.Generic;
using form_builder.Models.Elements;

namespace form_builder.Validators
{
    public class NumericValueValidator : IElementValidator
    {
        public ValidationResult Validate(Element element, Dictionary<string, string> viewModel)
        {
            if (!element.Properties.Numeric || !viewModel.ContainsKey(element.Properties.QuestionId))
            {
                return new ValidationResult
                {
                    IsValid = true
                };
            }

            var value = viewModel[element.Properties.QuestionId];

            if(string.IsNullOrEmpty(value) && element.Properties.Optional)
            {
                return new ValidationResult
                {
                    IsValid = true
                };
            }

            var isValid = int.TryParse(value, out int output);

            if (!isValid)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = $"{element.Properties.Label} must be a number"
                };
            }

            if (!string.IsNullOrEmpty(element.Properties.Max) && !string.IsNullOrEmpty(element.Properties.Min))
            {
                var max = int.Parse(element.Properties.Max);
                var min = int.Parse(element.Properties.Min);

                if (output > max || output < min)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = $"{element.Properties.Label} must be between {min} and {max} inclusive"
                    };
                }

            }

            if (!string.IsNullOrEmpty(element.Properties.Max))
            {
                var max = int.Parse(element.Properties.Max);

                if (output > max)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = $"{element.Properties.Label} must be less than or equal to {max}"
                    };
                }

            }

            if (!string.IsNullOrEmpty(element.Properties.Min))
            {
                var min = int.Parse(element.Properties.Min);

                if (output < min)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = $"{element.Properties.Label} must be greater than or equal to {min}"
                    };
                }

            }

            return new ValidationResult
            {
                IsValid = true
            };
        }
    }
}