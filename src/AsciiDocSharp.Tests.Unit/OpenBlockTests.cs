// AsciiDocSharp
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
using AsciiDocSharp.Core;
using AsciiDocSharp.Core.Elements;
using AsciiDocSharp.Core.Implementation;
using AsciiDocSharp.Parser.Implementation;
using AsciiDocSharp.Parser;
using AsciiDocSharp.Converters.Html;
using System.Linq;
using AsciiDocList = AsciiDocSharp.Core.IList;

namespace AsciiDocSharp.Tests.Unit
{
    public class OpenBlockTests
    {
        private readonly AsciiDocParser _parser = new AsciiDocParser();

        [Fact]
        public void Should_Parse_Simple_Open_Block()
        {
            // Arrange
            var input = @"--
This is open block content.
--";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var open = Assert.IsAssignableFrom<IOpen>(document.Elements.First());
            Assert.Null(open.Title);
            Assert.Null(open.MasqueradeType);
            Assert.Single(open.Children);
        }

        [Fact]
        public void Should_Parse_Open_Block_With_Multiple_Paragraphs()
        {
            // Arrange
            var input = @"--
First paragraph in open block.

Second paragraph in open block.
--";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var open = Assert.IsAssignableFrom<IOpen>(document.Elements.First());
            Assert.Equal(2, open.Children.Count);
            Assert.All(open.Children, child => Assert.IsAssignableFrom<IParagraph>(child));
        }

        [Fact]
        public void Should_Parse_Open_Block_With_Mixed_Content()
        {
            // Arrange
            var input = @"--
A paragraph.

* List item 1
* List item 2

Another paragraph.
--";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var open = Assert.IsAssignableFrom<IOpen>(document.Elements.First());
            Assert.Equal(2, open.Children.Count);
            
            Assert.IsAssignableFrom<IParagraph>(open.Children[0]);
            Assert.IsAssignableFrom<AsciiDocList>(open.Children[1]);
        }

        [Fact]
        public void Should_Parse_Empty_Open_Block()
        {
            // Arrange
            var input = @"--
--";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var open = Assert.IsAssignableFrom<IOpen>(document.Elements.First());
            Assert.Empty(open.Children);
        }

        [Fact]
        public void Should_Convert_Simple_Open_Block_To_Html()
        {
            // Arrange
            var input = @"--
This is open block content.
--";
            var converter = new HtmlDocumentConverter();

            // Act
            var document = _parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"openblock\">", html);
            Assert.Contains("<div class=\"content\">", html);
            Assert.Contains("This is open block content.", html);
            Assert.Contains("</div>", html);
        }

        [Fact]
        public void Should_Convert_Open_Block_With_Mixed_Content_To_Html()
        {
            // Arrange
            var input = @"--
A paragraph.

* List item 1
* List item 2
--";
            var converter = new HtmlDocumentConverter();

            // Act
            var document = _parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"openblock\">", html);
            Assert.Contains("<div class=\"content\">", html);
            Assert.Contains("<p>A paragraph.</p>", html);
            Assert.Contains("<ul>", html);
            Assert.Contains("<li>List item 1</li>", html);
            Assert.Contains("<li>List item 2</li>", html);
        }

        [Fact]
        public void Should_Tokenize_Open_Block_Delimiters()
        {
            // Arrange
            var input = "--";
            var tokenizer = new AsciiDocTokenizer(input);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.OpenDelimiter, token.Type);
            Assert.Equal("--", token.Value);
        }

        [Fact]
        public void Should_Parse_Open_Block_With_Code_Content()
        {
            // Arrange
            var input = @"--
Here is some code:

    def hello():
        print(""Hello world"")

That was code.
--";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var open = Assert.IsAssignableFrom<IOpen>(document.Elements.First());
            Assert.Equal(4, open.Children.Count);
        }

        [Fact]
        public void Should_Handle_Nested_Block_Content_In_Open_Block()
        {
            // Arrange
            var input = @"--
First paragraph.

____
This is a block quote inside an open block.
____

Last paragraph.
--";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var open = Assert.IsAssignableFrom<IOpen>(document.Elements.First());
            Assert.Equal(3, open.Children.Count);
            
            Assert.IsAssignableFrom<IParagraph>(open.Children[0]);
            Assert.IsAssignableFrom<IBlockQuote>(open.Children[1]);
            Assert.IsAssignableFrom<IParagraph>(open.Children[2]);
        }

        [Fact]
        public void Should_Convert_Empty_Open_Block_To_Html()
        {
            // Arrange
            var input = @"--
--";
            var converter = new HtmlDocumentConverter();

            // Act
            var document = _parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"openblock\">", html);
            Assert.Contains("<div class=\"content\">", html);
            // Should have opening and closing content div even if empty
            Assert.Matches(@"<div class=""content"">\s*</div>", html);
        }
    }
}