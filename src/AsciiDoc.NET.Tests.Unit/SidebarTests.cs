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

using Xunit;
using AsciiDoc.Net.Core.Elements;
using AsciiDoc.Net.Core.Implementation;
using AsciiDoc.Net.Parser.Implementation;
using AsciiDoc.Net.Parser;
using AsciiDoc.Net.Converters.Html;
using System.Linq;

namespace AsciiDoc.Net.Tests.Unit
{
    public class SidebarTests
    {
        private readonly AsciiDocParser _parser = new AsciiDocParser();

        [Fact]
        public void Should_Parse_Simple_Sidebar()
        {
            // Arrange
            var input = @"****
This is sidebar content.
****";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var sidebar = Assert.IsAssignableFrom<ISidebar>(document.Elements.First());
            Assert.Null(sidebar.Title);
            Assert.Single(sidebar.Children);
        }

        [Fact]
        public void Should_Parse_Sidebar_With_Multiple_Paragraphs()
        {
            // Arrange
            var input = @"****
First paragraph.

Second paragraph.
****";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var sidebar = Assert.IsAssignableFrom<ISidebar>(document.Elements.First());
            Assert.Equal(2, sidebar.Children.Count);
        }

        [Fact]
        public void Should_Convert_Sidebar_To_Html()
        {
            // Arrange
            var input = @"****
This is sidebar content.
****";
            var converter = new HtmlDocumentConverter();

            // Act
            var document = _parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"sidebarblock\">", html);
            Assert.Contains("<div class=\"content\">", html);
            Assert.Contains("This is sidebar content.", html);
        }

        [Fact]
        public void Should_Tokenize_Sidebar_Delimiters()
        {
            // Arrange
            var input = "****";
            var tokenizer = new AsciiDocTokenizer(input);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.SidebarDelimiter, token.Type);
            Assert.Equal("****", token.Value);
        }
    }
}