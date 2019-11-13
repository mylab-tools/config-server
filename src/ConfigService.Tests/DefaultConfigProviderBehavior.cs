using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigService.Services;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace ConfigService.Tests
{
    public class DefaultConfigProviderBehavior
    {
        private readonly ITestOutputHelper _output;
        private DefaultConfigProvider _service;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultConfigProviderBehavior"/>
        /// </summary>
        public DefaultConfigProviderBehavior(ITestOutputHelper output)
        {
            _output = output;

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "ConfigFiles");

            _output.WriteLine("Current config directory: " + basePath);

            _service = new DefaultConfigProvider(basePath);
        }

        [Fact]
        public void ShouldProvideClientList()
        {
            //Arrange
            

            //Act
            var list = _service.GetConfigList().ToArray();

            //Assert
            Assert.Equal(2, list.Length);
            Assert.Contains("foo", list);
            Assert.Contains("bar", list);
        }

        [Fact]
        public void ShouldProvideConfigContent()
        {
            //Arrange

            //Act
            var fooConfig = TestTools.Foo;

            //Assert
            Assert.NotNull(fooConfig);
            Assert.Equal("FooVal", fooConfig.FooParam);
        }

        [Fact]
        public void ShouldLoadInnerNodes()
        {
            //Arrange
            var fooConfig = TestTools.Foo;

            //Act


            //Assert
            Assert.NotNull(fooConfig.InnerObject);
            Assert.Equal("BarVal", fooConfig.InnerObject.Bar);
        }
    }
}
