﻿using form_builder.Validators;
using form_builder_tests.Builders;
using System.Collections.Generic;
using Xunit;

namespace form_builder_tests.UnitTests.Validators
{
    public class NumericValueElementValidatorTests
    {
        private readonly NumericValueElementValidator _validator = new NumericValueElementValidator();

        [Fact]
        public void Validate_ShouldReturnTrueValidationResult_WhenElementNotNumeric()
        {
            var element = new ElementBuilder()
                .WithQuestionId("tets-id")
                .WithNumeric(false)
                .Build();

            var result = _validator.Validate(element, null);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_ShouldReturnTrueValidationResult_WhenElementValidNumber()
        {
            var element = new ElementBuilder()
                .WithQuestionId("tets-id")
                .WithNumeric(true)
                .Build();

            var viewModel = new Dictionary<string, string>();
            viewModel.Add("tets-id", "123");

            var result = _validator.Validate(element, viewModel);

            Assert.True(result.IsValid);
            Assert.Equal("", result.Message);
        }

        [Fact]
        public void Validate_ShouldReturnFalseValidationResult_WhenElementNotValidNumber()
        {
            var label = "Test label";
            var element = new ElementBuilder()
                .WithQuestionId("tets-id")
                .WithNumeric(true)
                .WithLabel(label)
                .Build();

            var viewModel = new Dictionary<string, string>();
            viewModel.Add("tets-id", "a123");

            var result = _validator.Validate(element, viewModel);

            Assert.False(result.IsValid);
            Assert.Equal($"{label} must be a number", result.Message);
        }
    }
}
