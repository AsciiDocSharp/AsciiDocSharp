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

using System.Linq;
using Xunit;
using AsciiDocSharp.Parser.Implementation;
using AsciiDocSharp.Core.Elements;
using AsciiDocSharp.Converters.Html;

namespace AsciiDocSharp.Tests.Unit
{
    public class TableOfContentsTests
    {
        [Fact]
        public void TableOfContents_ShouldParseBasicTocMacro()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = @"toc::[]

= Document Title
== Section One
== Section Two";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.True(document.Elements.Count > 0);
            
            var toc = document.Elements.OfType<ITableOfContents>().FirstOrDefault();
            Assert.NotNull(toc);
            Assert.Equal("Table of Contents", toc.Title);
            Assert.Equal(3, toc.MaxDepth);
        }

        [Fact]
        public void TableOfContents_ShouldParseWithCustomTitle()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = @"toc::[title=""Contents""]

= Document Title";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            var toc = document.Elements.OfType<ITableOfContents>().FirstOrDefault();
            Assert.NotNull(toc);
            Assert.Equal("Contents", toc.Title);
        }

        [Fact]
        public void TableOfContents_ShouldParseWithCustomLevels()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = @"toc::[levels=2]

= Document Title";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            var toc = document.Elements.OfType<ITableOfContents>().FirstOrDefault();
            Assert.NotNull(toc);
            Assert.Equal(2, toc.MaxDepth);
        }

        [Fact]
        public void TableOfContents_ShouldConvertToHtml()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var input = @"toc::[]

= Document Title
== Section One
== Section Two";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("div class=\"toc\"", html);
            Assert.Contains("Table of Contents", html);
        }
    }
}