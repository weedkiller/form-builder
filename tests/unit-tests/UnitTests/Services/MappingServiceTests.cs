﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using form_builder.Builders;
using form_builder.Configuration;
using form_builder.Constants;
using form_builder.Enum;
using form_builder.Factories.Schema;
using form_builder.Mappers;
using form_builder.Models;
using form_builder.Models.Elements;
using form_builder.Providers.SchemaProvider;
using form_builder.Providers.StorageProvider;
using form_builder.Services.MappingService;
using form_builder_tests.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Models.Booking.Request;
using StockportGovUK.NetStandard.Models.FileManagement;
using Xunit;

namespace form_builder_tests.UnitTests.Services
{
    public class MappingServiceTests
    {
        private readonly MappingService _service;
        private readonly Mock<ISchemaProvider> _mockSchemaProvider = new Mock<ISchemaProvider>();
        private readonly Mock<IDistributedCacheWrapper> _mockDistrubutedCache = new Mock<IDistributedCacheWrapper>();
        private readonly Mock<IElementMapper> _mockElementMapper = new Mock<IElementMapper>();
        private readonly Mock<ISchemaFactory> _mockSchemaFactory = new Mock<ISchemaFactory>();
        private readonly Mock<ILogger<MappingService>> _mockLogger = new Mock<ILogger<MappingService>>();
        private readonly Mock<IOptions<DistributedCacheExpirationConfiguration>> _mockDistributedCacheExpirationConfiguration = new Mock<IOptions<DistributedCacheExpirationConfiguration>>();

        public MappingServiceTests()
        {
            var element = new ElementBuilder()
               .WithType(EElementType.Textarea)
               .WithQuestionId("test-question")
               .Build();

            var page = new PageBuilder()
                .WithElement(element)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaProvider.Setup(_ => _.Get<FormSchema>(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockDistrubutedCache.Setup(_ => _.GetString(It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(new FormAnswers
                {
                    Pages = new List<PageAnswers>()
                }));

            _mockDistributedCacheExpirationConfiguration.Setup(_ => _.Value).Returns(new DistributedCacheExpirationConfiguration
            {
                FormJson = 1
            });

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _service = new MappingService(_mockDistrubutedCache.Object, _mockElementMapper.Object, _mockSchemaFactory.Object, _mockDistributedCacheExpirationConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Map_ShouldCallCacheProvider_ToGetFormData()
        {
            // Act
            await _service.Map("form", "guid");

            // Assert
            _mockDistrubutedCache.Verify(_ => _.GetString(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Map_ShouldCallCache_ToGetFormSchema()
        {
            // Act
            await _service.Map("form", "guid");

            // Assert
            _mockSchemaFactory.Verify(_ => _.Build(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Map_ShouldReturnEmptyExpandoObject_WhenFormContains_NoValidatableElements()
        {
            // Arrange
            var element = new ElementBuilder()
                 .WithType(EElementType.H1)
                 .WithQuestionId("test-question")
                 .Build();

            var element2 = new ElementBuilder()
                .WithType(EElementType.Img)
                .WithQuestionId("test-img")
                .Build();

            var element3 = new ElementBuilder()
                .WithType(EElementType.Button)
                .WithButtonId("test-button")
                .Build();

            var page = new PageBuilder()
                .WithElement(element)
                .WithElement(element2)
                .WithElement(element3)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            Assert.Empty(resultData as IDictionary<string, object>);
        }

        [Fact]
        public async Task Map_ShouldReturnExpandoObject_WhenFormContains_SingleValidatableElement()
        {
            // Arrange
            var element = new ElementBuilder()
                 .WithType(EElementType.H1)
                 .WithQuestionId("test-question")
                 .Build();

            var element2 = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithQuestionId("text")
                .Build();


            var page = new PageBuilder()
                .WithElement(element)
                .WithElement(element2)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()))
                .Returns(new { });

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            dynamic castResultsData = resultData;

            Assert.NotNull(castResultsData.text);
        }

        [Fact]
        public async Task Map_ShouldReturnExpandoObject_WhenFormContains_MultipleValidatableElementsWithTargetMapping()
        {
            // Arrange
            var element = new ElementBuilder()
                 .WithType(EElementType.H1)
                 .WithQuestionId("test-question")
                 .Build();

            var element2 = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithQuestionId("text")
                .Build();

            var element3 = new ElementBuilder()
                .WithType(EElementType.DatePicker)
                .WithQuestionId("test")
                .WithTargetMapping("customer.datepicker.date")
                .Build();


            var page = new PageBuilder()
                .WithElement(element)
                .WithElement(element2)
                .WithElement(element3)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()))
                .Returns(new { });

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            dynamic castResultsData = resultData;
            var customerObject = castResultsData.customer;

            Assert.NotNull(castResultsData.text);
            Assert.NotNull(castResultsData.customer);
            Assert.NotNull(customerObject.datepicker);
        }

        [Fact]
        public async Task Map_ShouldReturnExpandoObject_WhenFormContains_ValidatableElementWithComplexTargetMapping()
        {
            // Arrange
            var element = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithTargetMapping("one.two.three.four.five.six")
                .WithQuestionId("text")
                .Build();

            var page = new PageBuilder()
                .WithElement(element)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()))
                .Returns(new { });

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockDistrubutedCache.Setup(_ => _.GetString(It.IsAny<string>()))
               .Returns(JsonConvert.SerializeObject(new FormAnswers
               {
                   Pages = new List<PageAnswers>()
               }));

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            dynamic castResultsData = resultData;
            var oneObject = castResultsData.one;
            var twoObject = oneObject.two;
            var threeObject = twoObject.three;
            var fourObject = threeObject.four;
            var fiveObject = fourObject.five;

            Assert.NotNull(oneObject);
            Assert.NotNull(twoObject);
            Assert.NotNull(threeObject);
            Assert.NotNull(fourObject);
            Assert.NotNull(fiveObject);
            Assert.NotNull(fiveObject.six);
        }

        [Fact]
        public async Task Map_ShouldReturnExpandoObject_WhenFormContains_MutipleValidatableElementsWithComplexTargetMapping()
        {
            // Arrange
            var element = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithTargetMapping("one.two.three.four.five.six")
                .WithQuestionId("text")
                .Build();

            var element2 = new ElementBuilder()
                .WithType(EElementType.Textarea)
                .WithTargetMapping("one.two.three.four.value")
                .WithQuestionId("text")
                .Build();

            var page = new PageBuilder()
                .WithElement(element)
                .WithElement(element2)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockDistrubutedCache.Setup(_ => _.GetString(It.IsAny<string>()))
               .Returns(JsonConvert.SerializeObject(new FormAnswers
               {
                   Pages = new List<PageAnswers>()
               }));

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()))
                .Returns(new { });

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            dynamic castResultsData = resultData;
            var oneObject = castResultsData.one;
            var twoObject = oneObject.two;
            var threeObject = twoObject.three;
            var fourObject = threeObject.four;
            var fiveObject = fourObject.five;

            Assert.NotNull(oneObject);
            Assert.NotNull(twoObject);
            Assert.NotNull(threeObject);
            Assert.NotNull(fourObject);
            Assert.NotNull(fourObject.value);
            Assert.NotNull(fiveObject);
            Assert.NotNull(fiveObject.six);
        }


        [Theory]
        [InlineData(EElementType.FileUpload)]
        [InlineData(EElementType.MultipleFileUpload)]
        public async Task Map_ShouldReturnEmptyExpandoObject_WhenNullResponse_ForFile(EElementType type)
        {
            // Arrange
            var element = new ElementBuilder()
                .WithType(type)
                .WithQuestionId("file")
                .Build();

            var page = new PageBuilder()
                .WithElement(element)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockDistrubutedCache.Setup(_ => _.GetString(It.IsAny<string>()))
               .Returns(JsonConvert.SerializeObject(new FormAnswers
               {
                   Pages = new List<PageAnswers>()
               }));

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()))
                .Returns(null);

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            dynamic castResultsData = resultData;

            Assert.Empty(castResultsData);
        }


        [Theory]
        [InlineData(EElementType.FileUpload)]
        [InlineData(EElementType.MultipleFileUpload)]
        public async Task Map_ShouldReturnExpandoObject_WithSingleFile(EElementType type)
        {
            // Arrange
            var element = new ElementBuilder()
                .WithType(type)
                .WithQuestionId("file")
                .Build();

            var element2 = new ElementBuilder()
                .WithType(type)
                .WithQuestionId("filetwo")
                .Build();

            var page = new PageBuilder()
                .WithElement(element)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockDistrubutedCache.Setup(_ => _.GetString(It.IsAny<string>()))
               .Returns(JsonConvert.SerializeObject(new FormAnswers
               {
                   Pages = new List<PageAnswers>()
               }));

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.Is<IElement>(x => x.Properties.QuestionId == "file"), It.IsAny<FormAnswers>()))
                .Returns(null);

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()))
                .Returns(new List<File> { new File() });

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            dynamic castResultsData = resultData;

            Assert.Single(castResultsData.file);
            Assert.NotNull(castResultsData);
        }

        [Theory]
        [InlineData(EElementType.FileUpload)]
        [InlineData(EElementType.MultipleFileUpload)]
        public async Task Map_ShouldReturnExpandoObject_WithTwoFiles_WithSameMapping(EElementType type)
        {
            // Arrange
            var element = new ElementBuilder()
                .WithType(type)
                .WithQuestionId("file")
                .WithTargetMapping("file")
                .Build();

            var element2 = new ElementBuilder()
                .WithType(type)
                .WithQuestionId("filetwo")
                .WithTargetMapping("file")
                .Build();

            var element3 = new ElementBuilder()
                .WithType(type)
                .WithQuestionId("filethree")
                .WithTargetMapping("file")
                .Build();

            var page = new PageBuilder()
                .WithElement(element)
                .WithElement(element2)
                .WithElement(element3)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockDistrubutedCache.Setup(_ => _.GetString(It.IsAny<string>()))
               .Returns(JsonConvert.SerializeObject(new FormAnswers
               {
                   Pages = new List<PageAnswers>()
               }));

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.Is<IElement>(x => x.Properties.QuestionId == "file"), It.IsAny<FormAnswers>()))
                .Returns(new List<File> { new File() });

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.Is<IElement>(x => x.Properties.QuestionId == "filetwo"), It.IsAny<FormAnswers>()))
                .Returns(null);

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.Is<IElement>(x => x.Properties.QuestionId == "filethree"), It.IsAny<FormAnswers>()))
                .Returns(new List<File> { new File() });

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            dynamic castResultsData = resultData;

            Assert.Equal(2, castResultsData.file.Count);
            Assert.NotNull(castResultsData);
            Assert.NotNull(castResultsData.file);
        }

        [Fact]
        public async Task Map_ShouldReturnExpandoObject_WithAdditionalFormAnswersData()
        {
            // Arrange
            var element = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithQuestionId("textbox")
                .Build();

            var page = new PageBuilder()
                .WithElement(element)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockDistrubutedCache.Setup(_ => _.GetString(It.IsAny<string>()))
               .Returns(JsonConvert.SerializeObject(new FormAnswers
               {
                   Pages = new List<PageAnswers>(),
                   AdditionalFormData = new Dictionary<string, object> { { "additional", "answerData" } }
               }));

            _mockElementMapper.Setup(_ => _.GetAnswerValue(It.Is<IElement>(x => x.Properties.QuestionId == "textbox"), It.IsAny<FormAnswers>()))
                .Returns("textbox answer");

            // Act
            var result = await _service.Map("form", "guid");

            // Assert
            var resultData = Assert.IsType<ExpandoObject>(result.Data);
            dynamic castResultsData = resultData;

            Assert.NotNull(castResultsData);
            Assert.NotNull(castResultsData.textbox);
            Assert.NotNull(castResultsData.additional);
            Assert.Equal("answerData", castResultsData.additional);
        }

        [Fact]
        public async Task MapBookingRequest_ShouldReturn_ValidBookingRequest()
        {
            // Arrange
            var bookingGuid = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-2);
            var startTime = DateTime.Today.Add(new TimeSpan(1, 0, 0));
            var expectedValue = startDate.Date.Add(startTime.TimeOfDay);
            var customerFirstnameElement = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithTargetMapping("customer.firstname")
                .Build();

            var page = new PageBuilder()
                .WithElement(customerFirstnameElement)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            var element = new ElementBuilder()
                .WithType(EElementType.Booking)
                .WithQuestionId("booking")
                .WithAppointmentType(bookingGuid)
                .Build();

            var viewModel = new Dictionary<string, object> {
                { $"{element.Properties.QuestionId}-{BookingConstants.APPOINTMENT_DATE}", startDate.ToString() },
                { $"{element.Properties.QuestionId}-{BookingConstants.APPOINTMENT_START_TIME}", startTime.ToString() }
            };

            // Act
            var result = await _service.MapBookingRequest("guid", element, viewModel, "form");

            // Assert
            Assert.IsType<BookingRequest>(result);
            Assert.Equal(bookingGuid, result.AppointmentId);
            Assert.Equal(expectedValue, result.StartDateTime);
            _mockSchemaFactory.Verify(_ => _.Build(It.IsAny<string>()), Times.Once);
            _mockElementMapper.Verify(_ => _.GetAnswerValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()), Times.Once);
        }

        [Fact]
        public async Task MapBookingRequest_ShouldReturn_ValidBookingRequest_WithAddress()
        {
            // Arrange
            var bookingGuid = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-2);
            var startTime = DateTime.Today.Add(new TimeSpan(1, 0, 0));
            var expectedValue = startDate.Date.Add(startTime.TimeOfDay);
            var customerFirstnameElement = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithQuestionId("firstname")
                .WithTargetMapping("customer.firstname")
                .Build();

            var addressElement = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithQuestionId("address")
                .Build();

            var behaviour = new BehaviourBuilder()
                .WithBehaviourType(EBehaviourType.GoToPage)
                .WithPageSlug("page-continue")
                .Build();

            var page = new PageBuilder()
                .WithElement(addressElement)
                .WithElement(customerFirstnameElement)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .WithBehaviour(behaviour)
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            _mockElementMapper.Setup(_ => _.GetAnswerStringValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()))
                .Returns("Address 1");

            _mockDistrubutedCache.Setup(_ => _.GetString(It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(new FormAnswers
                {
                    Pages = new List<PageAnswers>
                    {
                        new PageAnswers
                        {
                            PageSlug = "page-one",
                            Answers = new List<Answers>
                            {
                                new Answers
                                {
                                    QuestionId = "address",
                                    Response = "Address 1"
                                },
                                new Answers
                                {
                                    QuestionId = "firstname",
                                    Response = "firstname"
                                }
                            }
                        }
                    }
                }));

            var element = new ElementBuilder()
                .WithType(EElementType.Booking)
                .WithQuestionId("booking")
                .WithCustomerAddressId("address")
                .WithAppointmentType(bookingGuid)
                .Build();

            var viewModel = new Dictionary<string, object> {
                { $"{element.Properties.QuestionId}-{BookingConstants.APPOINTMENT_DATE}", startDate.ToString() },
                { $"{element.Properties.QuestionId}-{BookingConstants.APPOINTMENT_START_TIME}", startTime.ToString() }
            };

            // Act
            var result = await _service.MapBookingRequest("guid", element, viewModel, "form");

            // Assert
            Assert.IsType<BookingRequest>(result);
            Assert.Equal(bookingGuid, result.AppointmentId);
            Assert.Equal(expectedValue, result.StartDateTime);
            Assert.Equal("Address 1", result.Customer.Address);
            _mockSchemaFactory.Verify(_ => _.Build(It.IsAny<string>()), Times.Once);
            _mockElementMapper.Verify(_ => _.GetAnswerValue(It.IsAny<IElement>(), It.IsAny<FormAnswers>()), Times.Once);
            _mockElementMapper.Verify(_ => _.GetAnswerStringValue(addressElement, It.IsAny<FormAnswers>()), Times.Once);
        }

        [Fact]
        public async Task MapBookingRequest_Should_ThrowApplicationException_WhenAppointmentDate_NotFoundInViewModel()
        {
            // Arrange
            var bookingGuid = Guid.NewGuid();
            var startTime = DateTime.Today.Add(new TimeSpan(1, 0, 0));
            var customerFirstnameElement = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithTargetMapping("customer.firstname")
                .Build();

            var page = new PageBuilder()
                .WithElement(customerFirstnameElement)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            var element = new ElementBuilder()
                .WithType(EElementType.Booking)
                .WithQuestionId("booking")
                .WithAppointmentType(bookingGuid)
                .Build();

            var viewModel = new Dictionary<string, object> {
                { $"{element.Properties.QuestionId}-{BookingConstants.APPOINTMENT_START_TIME}", startTime.ToString() }
            };

            // Act
            var result = await Assert.ThrowsAsync<ApplicationException>(() => _service.MapBookingRequest("guid", element, viewModel, "testform"));

            // Assert
            Assert.Equal("MappingService::GetStartDateTime, Booking request viewmodel for form testform does not contain required booking start date", result.Message);
        }

        [Fact]
        public async Task MapBookingRequest_Should_ThrowApplicationException_WhenAppointmentTime_NotFoundInViewModel()
        {
            // Arrange
            var bookingGuid = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-2);
            var customerFirstnameElement = new ElementBuilder()
                .WithType(EElementType.Textbox)
                .WithTargetMapping("customer.firstname")
                .Build();

            var page = new PageBuilder()
                .WithElement(customerFirstnameElement)
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            var element = new ElementBuilder()
                .WithType(EElementType.Booking)
                .WithQuestionId("booking")
                .WithAppointmentType(bookingGuid)
                .Build();

            var viewModel = new Dictionary<string, object> {
                { $"{element.Properties.QuestionId}-{BookingConstants.APPOINTMENT_DATE}", startDate.ToString() }
            };

            // Act
            var result = await Assert.ThrowsAsync<ApplicationException>(() => _service.MapBookingRequest("guid", element, viewModel, "testform"));

            // Assert
            Assert.Equal("MappingService::GetStartDateTime, Booking request viewmodel for form testform does not contain required booking start time", result.Message);
        }

        [Fact]
        public async Task MapBookingRequest_Should_ThrowApplicationException_WhenCustomerObject_IsNotFound()
        {
            // Arrange
            var bookingGuid = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-2);
            var startTime = DateTime.Today.Add(new TimeSpan(1, 0, 0));

            var page = new PageBuilder()
                .WithValidatedModel(true)
                .WithPageSlug("page-one")
                .Build();

            var schema = new FormSchemaBuilder()
                .WithPage(page)
                .Build();

            _mockSchemaFactory.Setup(_ => _.Build(It.IsAny<string>()))
                .ReturnsAsync(schema);

            var element = new ElementBuilder()
                .WithType(EElementType.Booking)
                .WithQuestionId("booking")
                .WithAppointmentType(bookingGuid)
                .Build();

            var viewModel = new Dictionary<string, object> {
                { $"{element.Properties.QuestionId}-{BookingConstants.APPOINTMENT_DATE}", startDate.ToString() },
                { $"{element.Properties.QuestionId}-{BookingConstants.APPOINTMENT_START_TIME}", startTime.ToString() }
            };

            // Act
            var result = await Assert.ThrowsAsync<ApplicationException>(() => _service.MapBookingRequest("guid", element, viewModel, "testform"));

            // Assert
            Assert.Equal("MappingService::GetCustomerDetails, Booking request form data for form base-url does not contain required customer object", result.Message);
        }
    }
}
