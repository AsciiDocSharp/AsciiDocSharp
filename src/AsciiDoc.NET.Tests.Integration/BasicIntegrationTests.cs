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
using AsciiDoc.Net;
using AsciiDoc.Net.Converters.Html;
using System.IO;

namespace AsciiDoc.Net.Tests.Integration
{
    public class BasicIntegrationTests
    {
        [Fact]
        public void EndToEnd_SimpleDocument_ProducesHtml()
        {
            // Arrange
            var asciiDocContent = @"== Getting Started

This is a simple paragraph.

Here's another paragraph.";

            // Act
            var htmlResult = AsciiDoc.ToHtml(asciiDocContent);

            // Assert
            Assert.NotNull(htmlResult);
            Assert.Contains("<!DOCTYPE html>", htmlResult);
            Assert.Contains("<h3>Getting Started</h3>", htmlResult);
            Assert.Contains("<p>This is a simple paragraph.</p>", htmlResult);
            Assert.Contains("<p>Here&#39;s another paragraph.</p>", htmlResult);
            Assert.Contains("</html>", htmlResult);
        }

        [Fact]
        public void ProcessorWithOptions_MinifiedOutput_ProducesCompactHtml()
        {
            // Arrange
            var content = "== Test Header\n\nTest paragraph.";
            var options = AsciiDocOptions.CreateMinified();
            var processor = new AsciiDocProcessor();
            var converter = new HtmlDocumentConverter();

            // Act
            var result = processor.ProcessText(content, converter, options);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.DoesNotContain("\n    ", result); // Should not have pretty indentation
        }

        [Fact]
        public void ProcessorWithCustomConverter_DifferentOutputFormat_Works()
        {
            // Arrange
            var content = "== Test\n\nParagraph.";
            var processor = new AsciiDocProcessor();
            var htmlConverter = new HtmlDocumentConverter();

            // Act
            var document = processor.ParseFromText(content);
            var htmlResult = processor.ConvertDocument(document, htmlConverter);

            // Assert
            Assert.NotNull(document);
            Assert.NotNull(htmlResult);
            Assert.Contains("Test", htmlResult);
        }

        [Fact]
        public void HtmlConverter_HandlesSpecialCharacters_EscapesProperly()
        {
            // Arrange
            var content = "Paragraph with <tags> & \"quotes\" and 'apostrophes'.";
            var processor = new AsciiDocProcessor();
            var converter = new HtmlDocumentConverter();

            // Act
            var document = processor.ParseFromText(content);
            var result = processor.ConvertDocument(document, converter);

            // Assert
            Assert.Contains("&lt;tags&gt;", result);
            Assert.Contains("&amp;", result);
            Assert.Contains("&quot;quotes&quot;", result);
            Assert.Contains("&#39;apostrophes&#39;", result);
        }
    }
}