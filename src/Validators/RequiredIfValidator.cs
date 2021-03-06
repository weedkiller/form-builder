using System.Collections.Generic;
using form_builder.Models;
using form_builder.Models.Elements;

namespace form_builder.Validators
{
    public class RequiredIfValidator : IElementValidator
    {
        public ValidationResult Validate(Element element, Dictionary<string, dynamic> viewModel, FormSchema baseForm)
        {
            if (string.IsNullOrEmpty(element.Properties.RequiredIf))
            {
                return new ValidationResult
                {
                    IsValid = true
                };
            }

            var isValid = true;
            var requiredIf = element.Properties.RequiredIf.Split(':');
            var requiredKey = requiredIf[0];

            dynamic answeredValue = string.Empty;
            if (!viewModel.TryGetValue(requiredKey, out answeredValue))
            {
                return new ValidationResult
                {
                    IsValid = true
                };
            }

            answeredValue = viewModel[requiredKey];

            if (answeredValue == requiredIf[1])
            {
                if (element.Type == Enum.EElementType.Textarea || element.Type == Enum.EElementType.Textbox)
                {
                    if (viewModel[element.Properties.QuestionId] == string.Empty || viewModel[element.Properties.QuestionId] == null) isValid = false;
                }
                else
                {
                    dynamic value = string.Empty;
                    if (viewModel.TryGetValue(element.Properties.QuestionId, out value))
                    {
                        isValid = true;
                        if (value == string.Empty) isValid = false;
                    }
                    else
                    {
                        isValid = false;
                    }
                }
            }

            return new ValidationResult
            {
                IsValid = isValid,
                Message = isValid ? string.Empty
                    : !string.IsNullOrEmpty(element.Properties.RequiredIfValidationMessage)
                    ? element.Properties.RequiredIfValidationMessage : $"Check the { element.Properties.Label} and try again."
            };
        }
    }
}