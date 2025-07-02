// AsciiDoc.NET
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

using AsciiDoc.NET.Core.Implementation;
using AsciiDoc.NET.Parser.Implementation;
using AsciiDoc.NET.Converters.Html;
using AsciiDoc.NET.Converters.Core;
using Xunit;

namespace AsciiDoc.NET.Tests.Unit
{
    public class HighlightTextTests
    {
        [Fact]
        public void Parser_Should_Parse_Highlight_Text()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This is #highlighted text# in a paragraph.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.Equal(3, paragraph.Children.Count);
            
            Assert.IsType<Text>(paragraph.Children[0]);
            var highlight = Assert.IsType<Highlight>(paragraph.Children[1]);
            Assert.Equal("highlighted text", highlight.Text);
            Assert.IsType<Text>(paragraph.Children[2]);
        }

        [Fact]
        public void Parser_Should_Handle_Multiple_Highlights()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This has #first highlight# and #second highlight# text.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.Equal(5, paragraph.Children.Count);
            
            var firstHighlight = Assert.IsType<Highlight>(paragraph.Children[1]);
            Assert.Equal("first highlight", firstHighlight.Text);
            
            var secondHighlight = Assert.IsType<Highlight>(paragraph.Children[3]);
            Assert.Equal("second highlight", secondHighlight.Text);
        }

        [Fact]
        public void HtmlConverter_Should_Convert_Highlight_To_Mark_Element()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions();
            var highlight = new Highlight("highlighted content");

            // Act
            var document = new Document();
            var paragraph = new Paragraph();
            paragraph.AddChild(highlight);
            document.AddChild(paragraph);
            var html = converter.Convert(document, options);

            // Assert
            Assert.Contains("<mark>highlighted content</mark>", html);
        }

        [Fact]
        public void HtmlConverter_Should_Escape_Html_In_Highlight_Text()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions();
            var highlight = new Highlight("<script>alert('xss')</script>");

            // Act
            var document = new Document();
            var paragraph = new Paragraph();
            paragraph.AddChild(highlight);
            document.AddChild(paragraph);
            var html = converter.Convert(document, options);

            // Assert
            Assert.Contains("<mark>&lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;</mark>", html);
        }

        [Fact]
        public void Parser_Should_Handle_Highlight_With_Other_Formatting()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This has *bold*, #highlight#, and _italic_ text.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.Equal(7, paragraph.Children.Count);
            
            Assert.IsType<Strong>(paragraph.Children[1]);
            Assert.IsType<Highlight>(paragraph.Children[3]);
            Assert.IsType<Emphasis>(paragraph.Children[5]);
        }
    }
}