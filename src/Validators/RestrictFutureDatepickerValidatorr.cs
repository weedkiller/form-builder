﻿using System;
using System.Collections.Generic;
using form_builder.Models.Elements;
using form_builder.Enum;


namespace form_builder.Validators
{
    public class RestrictFutureDatepickerValidator :IElementValidator
    {
        public ValidationResult Validate(Element element, Dictionary<string, string> viewModel)
        {
            if (!element.Properties.RestrictFutureDate || element.Type != EElementType.DatePicker || element.Properties.Optional)
            {
                return new ValidationResult
                {
                    IsValid = true
                };
            }

            var value = viewModel.ContainsKey(element.Properties.QuestionId) ? viewModel[element.Properties.QuestionId]:null;
            

            var isValidDate = DateTime.TryParse(value, out _);

            if (!isValidDate)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = !string.IsNullOrEmpty(element.Properties.CustomValidationMessage) ? element.Properties.CustomValidationMessage : "Check the date and try again"
                };
            }

            var date = DateTime.Today;

            var dateOutput = DateTime.Parse(value);

            if (dateOutput > date)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = !string.IsNullOrEmpty(element.Properties.ValidationMessageRestrictFutureDate) ? element.Properties.ValidationMessageRestrictFutureDate : "Check the date and try again"
                };
            }

            return new ValidationResult
            {
                IsValid = true,
                Message = string.Empty
            };
        }


    }
}
