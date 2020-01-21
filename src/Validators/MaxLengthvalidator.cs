using System.Collections.Generic;
using form_builder.Models.Elements;

namespace form_builder.Validators
{
    public class MaxLengthValidator : IElementValidator
    {
        public ValidationResult Validate(Element element, Dictionary<string, string> viewModel)
        {
            if (!viewModel.ContainsKey(element.Properties.QuestionId))
            {
                return new ValidationResult
                {
                    IsValid = true
                };
            }

            var value = viewModel.ContainsKey(element.Properties.QuestionId) ? viewModel[element.Properties.QuestionId] : "";
           
            
            if(!string.IsNullOrEmpty(value) && value.Length > element.Properties.MaxLength)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = $"{element.Properties.Label} has a maximum length of {element.Properties.MaxLength}"
                };
            }            

            return new ValidationResult
            {
                IsValid = true
            };
        }
    }
}