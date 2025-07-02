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

using System.Linq;
using Xunit;
using AsciiDoc.NET.Parser.Implementation;
using AsciiDoc.NET.Core;
using AsciiDoc.NET.Core.Elements;
using AsciiDoc.NET.Converters.Html;

namespace AsciiDoc.NET.Tests.Unit
{
    public class FootnoteTests
    {
        [Fact]
        public void Footnote_ShouldParseBasicFootnote()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = @"This is some text footnote:[This is a footnote.] with a reference.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.True(document.Elements.Count > 0);
            
            var paragraph = document.Elements.OfType<IParagraph>().FirstOrDefault();
            Assert.NotNull(paragraph);
            
            var footnote = paragraph.Children.OfType<IFootnote>().FirstOrDefault();
            Assert.NotNull(footnote);
            Assert.Equal("This is a footnote.", footnote.Text);
            Assert.False(footnote.IsReference);
        }

        [Fact]
        public void Footnote_ShouldParseNamedFootnote()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = @"This is some text footnote:note1[This is a named footnote.] with a reference.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            var paragraph = document.Elements.OfType<IParagraph>().FirstOrDefault();
            Assert.NotNull(paragraph);
            
            var footnote = paragraph.Children.OfType<IFootnote>().FirstOrDefault();
            Assert.NotNull(footnote);
            Assert.Equal("note1", footnote.Id);
            Assert.Equal("This is a named footnote.", footnote.Text);
            Assert.False(footnote.IsReference);
        }

        [Fact]
        public void Footnote_ShouldParseFootnoteReference()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = @"This is some text footnote:note1[] with a reference.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            var paragraph = document.Elements.OfType<IParagraph>().FirstOrDefault();
            Assert.NotNull(paragraph);
            
            var footnote = paragraph.Children.OfType<IFootnote>().FirstOrDefault();
            Assert.NotNull(footnote);
            Assert.Equal("note1", footnote.Id);
            Assert.Empty(footnote.Text);
            Assert.True(footnote.IsReference);
        }

        [Fact]
        public void Footnote_ShouldConvertToHtml()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var input = @"This is some text footnote:[This is a footnote.] with a reference.";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("class=\"footnote\"", html);
            Assert.Contains("<sup>", html);
            Assert.Contains("</sup>", html);
        }
    }
}