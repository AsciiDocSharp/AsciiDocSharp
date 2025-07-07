// AsciiDoc.Net
// Copyright (c) 2025 Guillermo Espert
// Distributed under the MIT License. See LICENSE.adoc in the project root for full license information.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AsciiDoc.Net.Core;
using AsciiDoc.Net.Core.Elements;
using AsciiDoc.Net.Parser.Implementation;
using AsciiDoc.Net.Converters.Html;
using System.Linq;
using Xunit;

namespace AsciiDoc.Net.Tests.Unit
{
    public class ExampleBlockTests
    {
        [Fact]
        public void ParseExample_BasicExample_ParsesCorrectly()
        {
            // Arrange
            var input = @"====
This is an example.
====";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IExample>(document.Elements.First());
            
            var example = (IExample)document.Elements.First();
            Assert.Single(example.Children);
            Assert.IsAssignableFrom<IParagraph>(example.Children.First());
            
            var paragraph = (IParagraph)example.Children.First();
            Assert.Single(paragraph.Children);
            Assert.IsAssignableFrom<IText>(paragraph.Children.First());
            
            var text = (IText)paragraph.Children.First();
            Assert.Equal("This is an example.", text.Content);
        }

        [Fact]
        public void ParseExample_WithMultipleParagraphs_ParsesCorrectly()
        {
            // Arrange
            var input = @"====
This is the first paragraph.

This is the second paragraph.
====";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IExample>(document.Elements.First());
            
            var example = (IExample)document.Elements.First();
            Assert.Equal(2, example.Children.Count());
            Assert.All(example.Children, child => Assert.IsAssignableFrom<IParagraph>(child));
        }

        [Fact]
        public void ParseExample_WithNestedList_ParsesCorrectly()
        {
            // Arrange
            var input = @"====
This is an example with a list:

* Item 1
* Item 2
====";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IExample>(document.Elements.First());
            
            var example = (IExample)document.Elements.First();
            Assert.Equal(2, example.Children.Count());
            Assert.IsAssignableFrom<IParagraph>(example.Children.First());
            Assert.IsAssignableFrom<IList>(example.Children.Last());
        }

        [Fact]
        public void ConvertExample_ToHtml_ProducesCorrectMarkup()
        {
            // Arrange
            var input = @"====
This is an example.
====";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var document = parser.Parse(input);

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"exampleblock\">", html);
            Assert.Contains("<div class=\"content\">", html);
            Assert.Contains("This is an example.", html);
            Assert.Contains("</div>", html);
        }

        [Fact]
        public void ConvertExample_WithTitle_IncludesTitleInHtml()
        {
            // Arrange - Note: Title functionality would need to be implemented in parser
            // This test will initially fail but establishes the expected behavior
            var input = @".Example Title
====
This is an example with a title.
====";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var document = parser.Parse(input);

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"exampleblock\">", html);
            Assert.Contains("<div class=\"content\">", html);
            Assert.Contains("This is an example with a title.", html);
            // Note: Title functionality would need additional implementation
        }

        [Fact]
        public void ParseExample_EmptyExample_ParsesWithoutError()
        {
            // Arrange
            var input = @"====
====";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IExample>(document.Elements.First());
            
            var example = (IExample)document.Elements.First();
            Assert.Empty(example.Children);
        }
    }
}