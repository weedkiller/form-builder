﻿using System.Collections.Generic;
using form_builder.Builders;
using form_builder.Enum;
using form_builder.Models;
using form_builder.Models.Elements;
using form_builder.Models.Properties.ElementProperties;
using form_builder.Providers;
using form_builder.Validators;
using Moq;
using Xunit;

namespace form_builder_tests.UnitTests.Validators
{
    public class IsDateBeforeValidatorTests
    {
        Mock<IFormAnswersProvider> _mockFormAnswersProvider = new Mock<IFormAnswersProvider>();

        private Element datePickerelement = new ElementBuilder()
                            .WithType(EElementType.DatePicker)
                            .WithQuestionId("test-element")
                            .WithIsDateAfter("test-comparison-element")
                            .Build();

        public IsDateBeforeValidatorTests()
        {
            var formAnswers = new FormAnswers
            {
                Pages = new List<PageAnswers>
                {
                    new PageAnswers {
                        Answers = new List<Answers> { new Answers() }
                    }
                }
            };

            _mockFormAnswersProvider.Setup(_ => _.GetFormAnswers()).Returns(formAnswers);
        }

        

        [Fact]
        public void Validate_Returns_Valid_IfElemenType_IsNot_DateInputOrDatePicker()
        {
            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            // Arrange
            var texttBoxElement = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithIsDateBefore("test-comparison-element")
                .Build();

            // Act
            var result = isDateBeforeValidator.Validate(texttBoxElement, null, SchemaWithDatePickerElement);

            // Assert
            Assert.True(result.IsValid);
        }
        
        [Fact]
        public void Validate_ShouldReturn_Valid_IfComparisonElement_IsNullOrEmpty()
        {
            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            // Act
            var result = isDateBeforeValidator.Validate(datePickerelement, null, SchemaWithElement(new Element { Properties = new BaseProperty() } ));

            // Assert
            Assert.True(result.IsValid);        
        }

        [Fact]
        public void Validate_Returns_Valid_IfComparisonElemenType_IsNot_DateInputOrDatePicker()
        {
            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            var formSchema = SchemaWithElement(
                                new Textbox
                                {
                                    Properties = new BaseProperty { QuestionId = "test-comparison-element" }
                                });


            // Act
            var result = isDateBeforeValidator.Validate(datePickerelement, null, formSchema);

            // Assert
            Assert.True(result.IsValid);
        }
        
        [Fact]
        public void Validate_ShouldReturn_Valid_IfElement_DoesNotHaveAValue()
        {   
            var formAnswers = new FormAnswers
            {
                Pages = new List<PageAnswers>
                {
                    new PageAnswers {
                        Answers = new List<Answers> { new Answers
                        {
                            QuestionId = "test-comparison-element",
                            Response = "01/01/2021"
                        }}
                    }
                }
            };

            _mockFormAnswersProvider.Setup(_ => _.GetFormAnswers()).Returns(formAnswers);

            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            var viewModel = new Dictionary<string, dynamic>{
                {
                    "test-element",
                    string.Empty
                }
            };

            // Arrange/Act
            var result = isDateBeforeValidator.Validate(datePickerelement, viewModel, SchemaWithDatePickerElement);

            // Assert
            Assert.True(result.IsValid);        
        }

        [Fact]
        public void Validate_ShouldReturn_Valid_IfComparisonElement_DoesNotHaveAValue()
        {   
            var formAnswers = new FormAnswers
            {
                Pages = new List<PageAnswers>
                {
                    new PageAnswers {
                        Answers = new List<Answers> { new Answers
                        {
                            QuestionId = "test-comparison-element"
                        }}
                    }
                }
            };

            _mockFormAnswersProvider.Setup(_ => _.GetFormAnswers()).Returns(formAnswers);

            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            var viewModel = new Dictionary<string, dynamic>{
                {
                    "test-element",
                    "01/01/2020"
                }
            };

            // Arrange/Act
            var result = isDateBeforeValidator.Validate(datePickerelement, viewModel, SchemaWithDatePickerElement);

            // Assert
            Assert.True(result.IsValid);        
        }

        [Fact]
        public void Validate_ShouldReturn_Valid_IfElement_IsValid_OnSamePageSubmission()
        {   
            var formAnswers = new FormAnswers
            {
                Pages = new List<PageAnswers>
                {
                    new PageAnswers {
                        Answers = new List<Answers>()
                    }
                }
            };

            _mockFormAnswersProvider.Setup(_ => _.GetFormAnswers()).Returns(formAnswers);

            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            var viewModel = new Dictionary<string, dynamic>{
                {
                    "test-element",
                    "01/01/2019"
                },
                {
                    "test-comparison-element",
                    "01/01/2020"
                }
            };

            // Arrange/Act
            var result = isDateBeforeValidator.Validate(datePickerelement, viewModel, SchemaWithDatePickerElement);

            // Assert
            Assert.True(result.IsValid);        
        }

        [Fact]
        public void Validate_ShouldReturn_NotValid_IfElement_IsNotValid_OnSamePageSubmission()
        {   
            var formAnswers = new FormAnswers
            {
                Pages = new List<PageAnswers>
                {
                    new PageAnswers {
                        Answers = new List<Answers>()
                    }
                }
            };

            _mockFormAnswersProvider.Setup(_ => _.GetFormAnswers()).Returns(formAnswers);

            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            var viewModel = new Dictionary<string, dynamic>{
                {
                    "test-element",
                    "01/01/2021"
                },
                {
                    "test-comparison-element",
                    "01/01/2020"
                }
            };

            // Arrange/Act
            var result = isDateBeforeValidator.Validate(datePickerelement, viewModel, SchemaWithDatePickerElement);

            // Assert
            Assert.False(result.IsValid);        
        }

        [Fact]
        public void Validate_ShouldReturn_Valid_IfElement_IsValid_OnPreviousPageAnswer()
        {   
            var formAnswers = new FormAnswers
            {
                Pages = new List<PageAnswers>
                {
                    new PageAnswers {
                        Answers = new List<Answers> { new Answers
                        {
                            QuestionId = "test-comparison-element",
                            Response = "01/01/2021"
                        }}
                    }
                }
            };

            _mockFormAnswersProvider.Setup(_ => _.GetFormAnswers()).Returns(formAnswers);

            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            var viewModel = new Dictionary<string, dynamic>{
                {
                    "test-element",
                    "01/01/2020"
                }
            };

            // Arrange/Act
            var result = isDateBeforeValidator.Validate(datePickerelement, viewModel, SchemaWithDatePickerElement);

            // Assert
            Assert.True(result.IsValid);        
        }

        [Fact]
        public void Validate_ShouldReturn_NotValid_IfElement_IsNotValid_OnPreviousPageAnswer()
        {   
            var formAnswers = new FormAnswers
            {
                Pages = new List<PageAnswers>
                {
                    new PageAnswers {
                        Answers = new List<Answers> { new Answers
                        {
                            QuestionId = "test-comparison-element",
                            Response = "01/01/2020"
                        }}
                    }
                }
            };

            _mockFormAnswersProvider.Setup(_ => _.GetFormAnswers()).Returns(formAnswers);

            IsDateBeforeValidator isDateBeforeValidator = new IsDateBeforeValidator(_mockFormAnswersProvider.Object);

            var viewModel = new Dictionary<string, dynamic>{
                {
                    "test-element",
                    "01/01/2021"
                }
            };

            // Arrange/Act
            var result = isDateBeforeValidator.Validate(datePickerelement, viewModel, SchemaWithDatePickerElement);

            // Assert
            Assert.False(result.IsValid);        
        }

        private FormSchema SchemaWithElement(IElement element) => 
            new FormSchema
            {
                Pages = new List<Page>
                {
                    new Page {
                        Elements = new List<IElement>
                        {
                            element
                        }
                    }
                }
            };

        private FormSchema SchemaWithDatePickerElement => 
            SchemaWithElement(new DatePicker
                            {
                                Properties = new BaseProperty { QuestionId = "test-comparison-element" }
                            }) ;
    }
}
