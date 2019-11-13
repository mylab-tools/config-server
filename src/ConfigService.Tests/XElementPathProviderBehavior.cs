using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ConfigService.Tools;
using Xunit;

namespace ConfigService.Tests
{
    public class XElementPathProviderBehavior
    {
        [Fact]
        public void ShouldProvideXmlPath()
        {
            //Arrange
            var xmlStr = "<root><node1><node2></node2></node1></root>";
            var xmlModel = XDocument.Parse(xmlStr);
            var innerNode = (XElement)((XElement) xmlModel.Root.FirstNode).FirstNode;

            //Act
            var path = XElementPathProvider.Provide(innerNode);

            //Assert
            Assert.Equal("/root/node1/node2", path);
        }
    }
}
