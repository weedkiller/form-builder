using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using form_builder.Enum;
using form_builder.Models;
using form_builder.Models.Elements;
using form_builder.Models.Properties.ElementProperties;

namespace form_builder.Builders
{
    public class ElementBuilder
    {
        private EElementType _type = EElementType.H1;
        private string _lookup = string.Empty;

        private BaseProperty _property = new BaseProperty();

        public Element Build()
        {
            var elementType = typeof(IElement).GetTypeInfo().Assembly
                .GetTypes()
                .FirstOrDefault(type => type.Name == _type.ToString());

            var element = (Element)Activator.CreateInstance(elementType);

            element.Properties = _property;
            element.Lookup = _lookup;

            return element;
        }

        public ElementBuilder WithType(EElementType type)
        {
            _type = type;

            return this;
        }

        public ElementBuilder WithLookup(string lookup)
        {
            _lookup = lookup;

            return this;
        }

        public ElementBuilder WithPropertyText(string propertyText)
        {
            _property.Text = propertyText;

            return this;
        }

        public ElementBuilder WithQuestionId(string questionId)
        {
            _property.QuestionId = questionId;

            return this;
        }

        public ElementBuilder WithTargetMapping(string targetMapping)
        {
            _property.TargetMapping = targetMapping;

            return this;
        }

        public ElementBuilder WithLabel(string label)
        {
            _property.Label = label;

            return this;
        }

        public ElementBuilder WithAddressLabel(string label)
        {
            _property.AddressLabel = label;

            return this;
        }

        public ElementBuilder WithValue(string value)
        {
            _property.Value = value;

            return this;
        }

        public ElementBuilder WithListItems(List<string> listItems)
        {
            _property.ListItems = listItems;

            return this;
        }

        public ElementBuilder WithSource(string source)
        {
            _property.Source = source;

            return this;
        }

        public ElementBuilder WithAltText(string alt)
        {
            _property.AltText = alt;

            return this;
        }

        public ElementBuilder WithMaxLength(int maxLength)
        {
            _property.MaxLength = maxLength;

            return this;
        }

        public ElementBuilder WithOptions(List<Option> options)
        {
            _property.Options = options;

            return this;
        }

        public ElementBuilder WithRestrictFutureDate(bool value, string validMessage = "")
        {
            _property.RestrictFutureDate = value;
            _property.ValidationMessageRestrictFutureDate = validMessage;

            return this;
        }

        public ElementBuilder WithRestrictPastDate(bool value, string validMessage = "")
        {
            _property.RestrictPastDate = value;
            _property.ValidationMessageRestrictPastDate = validMessage;

            return this;
        }

        public ElementBuilder WithRestrictCurrentDate(bool value, string validMessage = "")
        {
            _property.RestrictCurrentDate = value;
            _property.ValidationMessageRestrictCurrentDate = validMessage;

            return this;
        }

        public ElementBuilder WithEmail(bool value)
        {
            _property.Email = value;

            return this;
        }

        public ElementBuilder WithOptional(bool value)
        {
            _property.Optional = value;

            return this;
        }

        public ElementBuilder WithDayValue(string value)
        {
            _property.Day = value;

            return this;
        }

        public ElementBuilder WithMonthValue(string value)
        {
            _property.Month = value;

            return this;
        }

        public ElementBuilder WithYearValue(string value)
        {
            _property.Year = value;
            return this;
        }

        public ElementBuilder WithAddressProvider(string value)
        {
            _property.AddressProvider = value;

            return this;
        }

        public ElementBuilder WithStreetProvider(string value)
        {
            _property.StreetProvider = value;

            return this;
        }

        public ElementBuilder WithOrganisationProvider(string value)
        {
            _property.OrganisationProvider = value;

            return this;
        }

        public ElementBuilder WithNumeric(bool value)
        {
            _property.Numeric = value;

            return this;
        }

        public ElementBuilder WithHint(string value)
        {
            _property.Hint = value;

            return this;
        }

        public ElementBuilder WithRegex(string value)
        {
            _property.Regex = value;

            return this;
        }

        public ElementBuilder WithMax(string value)
        {
            _property.Max = value;

            return this;
        }

        public ElementBuilder WithMin(string value)
        {
            _property.Min = value;

            return this;
        }

        public ElementBuilder WithButtonId(string buttonId)
        {
            _property.ButtonId = buttonId;

            return this;
        }

        public ElementBuilder WithUpperLimitValidationMessage(string message)
        {
            _property.UpperLimitValidationMessage = message;
            
            return this;
        }

        public ElementBuilder WithAcceptedMimeType(string type)
        {
            if (_property.AllowedFileTypes == null)
                _property.AllowedFileTypes = new List<string>();

            _property.AllowedFileTypes.Add(type);

            return this;
        }

        public ElementBuilder WithDocumentType(EDocumentType docType)
        {
            _property.DocumentType = docType;

            return this;
        }

        public ElementBuilder WithCalculationSlugs(SubmitSlug submitSlug)
        {
            if (_property.CalculationSlugs == null)
                _property.CalculationSlugs = new List<SubmitSlug>();

            _property.CalculationSlugs.Add(submitSlug);

            return this;
        }

        public ElementBuilder WithStockportPostcode(bool isStockportPostCode)
        {
            _property.StockportPostcode = isStockportPostCode;

            return this;
        }

        public ElementBuilder WithDisableManualAddress(bool disableManualAddress)
        {
            _property.DisableManualAddress = disableManualAddress;

            return this;
        }

        public ElementBuilder WithNoManualAddressDetailText(string noManualAddressDetailText)
        {
            _property.NoManualAddressDetailText = noManualAddressDetailText;

            return this;
        }
    }
}