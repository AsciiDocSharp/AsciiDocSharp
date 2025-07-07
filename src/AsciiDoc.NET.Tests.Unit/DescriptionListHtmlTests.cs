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
using AsciiDoc.Net.Core;
using AsciiDoc.Net.Parser.Implementation;
using AsciiDoc.Net.Converters.Html;
using AsciiDoc.Net.Converters.Core;
using System.Linq;

namespace AsciiDoc.Net.Tests.Unit
{
    public class DescriptionListHtmlTests
    {
        private readonly AsciiDocParser _parser = new AsciiDocParser();
        private readonly HtmlDocumentConverter _converter = new HtmlDocumentConverter();

        [Fact]
        public void Should_Convert_Description_List_To_Html()
        {
            // Arrange
            var input = @"CPU:: The brain of the computer.
RAM:: Random Access Memory.";

            // Act
            var document = _parser.Parse(input);
            var html = _converter.Convert(document);

            // Assert
            Assert.Contains("<dl>", html);
            Assert.Contains("<dt>CPU</dt>", html);
            Assert.Contains("<dd>The brain of the computer.</dd>", html);
            Assert.Contains("<dt>RAM</dt>", html);
            Assert.Contains("<dd>Random Access Memory.</dd>", html);
            Assert.Contains("</dl>", html);
        }

        [Fact]
        public void Should_Escape_Html_In_Description_List()
        {
            // Arrange
            var input = @"HTML:: The &lt;markup&gt; language.
Script:: Uses <script> tags.";

            // Act
            var document = _parser.Parse(input);
            var html = _converter.Convert(document);

            // Assert
            Assert.Contains("<dt>HTML</dt>", html);
            Assert.Contains("<dd>The &amp;lt;markup&amp;gt; language.</dd>", html);
            Assert.Contains("<dt>Script</dt>", html);
            Assert.Contains("<dd>Uses &lt;script&gt; tags.</dd>", html);
        }
    }
}