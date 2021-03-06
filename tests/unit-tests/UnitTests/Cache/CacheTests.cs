﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using form_builder.Configuration;
using form_builder.Enum;
using form_builder.Models;
using form_builder.Providers.SchemaProvider;
using form_builder.Providers.StorageProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace form_builder_tests.UnitTests.Cache
{
    public class CacheTests
    {
        private readonly form_builder.Cache.Cache _cache;
        private readonly Mock<IDistributedCacheWrapper> _mockDistributedCacheWrapper = new Mock<IDistributedCacheWrapper>();
        private readonly Mock<ISchemaProvider> _mockSchemaProvider = new Mock<ISchemaProvider>();
        private readonly Mock<IOptions<DistributedCacheConfiguration>> _mockDistributedCacheSettings = new Mock<IOptions<DistributedCacheConfiguration>>();
        private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();
        private const string _applicationVersion = "v2";
        public CacheTests()
        {
            _mockConfiguration.Setup(_ => _["ApplicationVersion"]).Returns(_applicationVersion);


            _mockDistributedCacheSettings.Setup(_ => _.Value).Returns(new DistributedCacheConfiguration
            {
                UseDistributedCache = true
            });

            _cache = new form_builder.Cache.Cache(_mockDistributedCacheWrapper.Object, _mockSchemaProvider.Object, _mockDistributedCacheSettings.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetFromCacheOrDirectlyFromSchemaAsync_ShouldCallSchemaProvider_WhenUseDistributedCache_IsFalse()
        {
            // Arrange
            _mockDistributedCacheSettings.Setup(_ => _.Value).Returns(new DistributedCacheConfiguration
            {
                UseDistributedCache = false
            });

            // Act
            await _cache.GetFromCacheOrDirectlyFromSchemaAsync<FormSchema>("form", 10, ESchemaType.FormJson);

            // Assert
            _mockSchemaProvider.Verify(_ => _.Get<FormSchema>(It.IsAny<string>()), Times.Once);
            _mockDistributedCacheWrapper.Verify(_ => _.GetString(It.IsAny<string>()), Times.Never);
            _mockDistributedCacheWrapper.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task TaskGetFromCacheOrDirectlyFromSchemaAsync_ShouldCallSchemaProvider_WhenMinutesIsZero()
        {
            // act
            await _cache.GetFromCacheOrDirectlyFromSchemaAsync<FormSchema>("", 0, ESchemaType.FormJson);

            // Assert
            _mockSchemaProvider.Verify(_ => _.Get<FormSchema>(It.IsAny<string>()), Times.Once);
            _mockDistributedCacheWrapper.Verify(_ => _.GetString(It.IsAny<string>()), Times.Never);
            _mockDistributedCacheWrapper.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task TaskGetFromCacheOrDirectlyFromSchemaAsync_ShouldReturnDataWhenFoundInCache()
        {
            // Arrange

            var _pages = new List<Page>();
            var schema = new FormSchema() { Pages = _pages };
            _mockDistributedCacheWrapper.Setup(_ => _.GetString(It.IsAny<string>()))
                .Returns(Newtonsoft.Json.JsonConvert.SerializeObject(schema));

            // Act
            var result = await _cache.GetFromCacheOrDirectlyFromSchemaAsync<FormSchema>("testform", 10, ESchemaType.FormJson);

            // Assert
            _mockDistributedCacheWrapper.Verify(_ => _.GetString(It.Is<string>(x => x == $"form-json-{_applicationVersion}-testform")), Times.Once);
            _mockSchemaProvider.Verify(_ => _.Get<FormSchema>(It.IsAny<string>()), Times.Never);
            _mockDistributedCacheWrapper.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.IsType<FormSchema>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaskGetFromCacheOrDirectlyFromSchemaAsync_ShouldCheckForData_InCache_Then_CallAndSetDataUsingSchema()
        {
            // Arrange
            var _pages = new List<Page>();
            var schema = new FormSchema() { Pages = _pages };
            var minutes = 10;
            _mockSchemaProvider.Setup(_ => _.Get<FormSchema>(It.IsAny<string>()))
                .ReturnsAsync(schema);

            // Act
            var result = await _cache.GetFromCacheOrDirectlyFromSchemaAsync<FormSchema>("testform", minutes, ESchemaType.FormJson);

            // Assert
            _mockDistributedCacheWrapper.Verify(_ => _.GetString(It.Is<string>(x => x == $"form-json-{_applicationVersion}-testform")), Times.Once);
            _mockSchemaProvider.Verify(_ => _.Get<FormSchema>(It.IsAny<string>()), Times.Once);
            _mockDistributedCacheWrapper.Verify(_ => _.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.Is<int>(x => x == minutes), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<FormSchema>(result);
            Assert.NotNull(result);
        }
    }
}