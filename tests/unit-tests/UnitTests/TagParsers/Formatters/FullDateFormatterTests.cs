using System;
using form_builder.TagParsers.Formatters;
using Xunit;

namespace form_builder_tests.UnitTests.TagParsers.Formatters
{
    public class FullDateFormatterTests
    {
        private FullDateFormatter _formatter = new FullDateFormatter();

        [Fact]
        public void Parse_ShouldReturn_CorrectDateTimeFormat()
        {
            Assert.Equal("Wednesday 1 January 2020", _formatter.Parse(new DateTime(2020, 1, 1).ToString()));
        }

    }
}