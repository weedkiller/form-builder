using System;
using System.Collections.Generic;
using form_builder.Constants;
using form_builder.Enum;
using form_builder.Models;
using form_builder.Models.Elements;
using form_builder.Providers;

namespace form_builder.Validators
{
    public class IsDateAfterValidator : IElementValidator
    {
        private readonly IFormAnswersProvider _formAnswersProvider;

        public IsDateAfterValidator(IFormAnswersProvider formAnswersProvider)
        {
            _formAnswersProvider = formAnswersProvider;
        }

        public ValidationResult Validate(Element currentElement, Dictionary<string, dynamic> viewModel, FormSchema baseForm)
        {
            IElement comparisonElement  = baseForm.GetElement(currentElement.Properties.IsDateAfter);
            
            // Check that all referenced element are valid for this validator type
            if(!IsValidatorRelevant(currentElement, comparisonElement))
                return new ValidationResult { IsValid = true };

            // Check that the currrent element being validated has a value - it could be optional
            DateTime? currentElementValue = GetElementValue(currentElement, viewModel);
            if(!currentElementValue.HasValue)
                return new ValidationResult { IsValid = true };

            // The comparison value could be in either the current page answers or answers provided on previous pages
            // First check the current page submission (viewModel) for relevant values
            DateTime? comparisonElementValue = GetElementValue(comparisonElement, viewModel);
            if(!comparisonElementValue.HasValue)
            {
                // If there is no valid value check previous providedanswers
                FormAnswers answers = _formAnswersProvider.GetFormAnswers();
                comparisonElementValue = GetElementValue(comparisonElement, answers);
            }

            // if the comparisonElement has a value - it could be optional (this maybe covered by a "requiredIf" validator)
            if(!comparisonElementValue.HasValue)
                return new ValidationResult { IsValid = true };

            // If what we're trying to compare doesn't contain a valid date assume success for now - perhaps this should really be a failure?
            if(currentElementValue > comparisonElementValue) 
                return new ValidationResult { IsValid = true };

            return new ValidationResult {
                IsValid = false,
                Message = !string.IsNullOrEmpty(currentElement.Properties.IsDateAfterValidationMessage) 
                                ? currentElement.Properties.IsDateAfterValidationMessage 
                                : string.Format(ValidationConstants.IS_DATE_AFTER_VALIDATOR_DEFAULT, currentElement.Properties.IsDateAfter)
            };
        }

        private bool IsValidatorRelevant(IElement element, IElement comparisonElement)
        {
            if (element.Type != EElementType.DatePicker && element.Type != EElementType.DateInput)
                return false;
            
            if(comparisonElement == null)
                return false;

            if (comparisonElement.Type != EElementType.DatePicker && comparisonElement.Type != EElementType.DateInput)
                return false;

            if ((string.IsNullOrEmpty(element.Properties.IsDateAfter)))
                return false;

            return true;
        }        
        
        private DateTime? GetElementValue(IElement element, Dictionary<string, dynamic> viewModel)
        {   
            if(element.Type == EElementType.DatePicker)
                return DatePicker.GetDate(viewModel, element.Properties.QuestionId);

            if(element.Type == EElementType.DateInput)
                return DateInput.GetDate(viewModel, element.Properties.QuestionId);

            return new DateTime();
        }

        private DateTime? GetElementValue(IElement element,  FormAnswers formAnswers)
        {            
            if(element.Type == EElementType.DatePicker)
                return DatePicker.GetDate(formAnswers, element.Properties.QuestionId);

            if(element.Type == EElementType.DateInput)
                return DateInput.GetDate(formAnswers, element.Properties.QuestionId);

            return new DateTime();
        }
    }
}