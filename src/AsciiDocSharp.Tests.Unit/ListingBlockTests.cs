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

using AsciiDocSharp.Core;
using AsciiDocSharp.Core.Elements;
using AsciiDocSharp.Core.Implementation;
using AsciiDocSharp.Parser.Implementation;
using AsciiDocSharp.Converters.Html;
using System.Linq;
using Xunit;

namespace AsciiDocSharp.Tests.Unit
{
    public class ListingBlockTests
    {
        [Fact]
        public void ParseListing_DelimitedListingBlock_ParsesCorrectly()
        {
            // Arrange
            var input = @"----
function hello() {
    console.log('Hello, World!');
}
----";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<ICodeBlock>(document.Elements.First());
            
            // Note: ---- without attributes creates a code block, not listing
            var codeBlock = (ICodeBlock)document.Elements.First();
            Assert.Contains("function hello()", codeBlock.Content);
            Assert.Contains("console.log('Hello, World!');", codeBlock.Content);
        }

        [Fact]
        public void ParseListing_ListingWithAttribute_ParsesCorrectly()
        {
            // Arrange
            var input = @"[listing]
function hello() {
    console.log('Hello, World!');
}";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IListing>(document.Elements.First());
            
            var listing = (IListing)document.Elements.First();
            Assert.Contains("function hello()", listing.Content);
            Assert.Contains("console.log('Hello, World!');", listing.Content);
        }

        [Fact]
        public void ParseListing_ListingWithAttributeAndDelimiter_ParsesCorrectly()
        {
            // Arrange
            var input = @"[listing]
----
function hello() {
    console.log('Hello, World!');
}
----";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IListing>(document.Elements.First());
            
            var listing = (IListing)document.Elements.First();
            Assert.Contains("function hello()", listing.Content);
            Assert.Contains("console.log('Hello, World!');", listing.Content);
        }

        [Fact]
        public void ParseListing_ListingWithLineBreaks_PreservesFormatting()
        {
            // Arrange
            var input = @"[listing]
----
Line one
Line two

Line four after blank line
----";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var listing = (IListing)document.Elements.First();
            
            // Verify line breaks are preserved
            var lines = listing.Content.Split('\n');
            Assert.Equal(4, lines.Length);
            Assert.Equal("Line one", lines[0]);
            Assert.Equal("Line two", lines[1]);
            Assert.Equal("", lines[2]); // Empty line preserved
            Assert.Equal("Line four after blank line", lines[3]);
        }

        [Fact]
        public void ParseListing_EmptyListingBlock_ParsesWithoutError()
        {
            // Arrange
            var input = @"[listing]
----
----";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IListing>(document.Elements.First());
            
            var listing = (IListing)document.Elements.First();
            Assert.Empty(listing.Content);
        }

        [Fact]
        public void ConvertListing_DelimitedListingBlock_GeneratesCorrectHtml()
        {
            // Arrange
            var input = @"[listing]
----
function hello() {
    console.log('Hello, World!');
}
----";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"listingblock\">", html);
            Assert.Contains("<div class=\"content\">", html);
            Assert.Contains("<pre>", html);
            Assert.Contains("function hello()", html);
            Assert.Contains("console.log(&#39;Hello, World!&#39;);", html);
            Assert.Contains("</pre>", html);
            Assert.Contains("</div>", html);
        }

        [Fact]
        public void ConvertListing_ListingWithSpecialCharacters_EscapesHtml()
        {
            // Arrange
            var input = @"[listing]
----
<script>alert('test')</script>
function test() { return ""hello & goodbye""; }
----";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("&lt;script&gt;", html);
            Assert.Contains("&amp;", html);
            Assert.Contains("&quot;", html);
            Assert.DoesNotContain("<script>", html);
        }

        [Fact]
        public void ConvertListing_ListingWithAttributeStyle_ProducesCorrectHtml()
        {
            // Arrange
            var input = @"[listing]
Simple listing text
with multiple lines";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"listingblock\">", html);
            Assert.Contains("Simple listing text", html);
            Assert.Contains("with multiple lines", html);
        }

        [Fact]
        public void ConvertListing_ListingWithLanguage_IncludesLanguageClass()
        {
            // Arrange - Create listing with language directly since we don't have source attribute parsing yet
            var listing = new Listing("console.log('hello');", "javascript", "Example");
            var converter = new HtmlDocumentConverter();
            var document = new Document();
            document.AddChild(listing);

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"listingblock\">", html);
            Assert.Contains("<div class=\"title\">", html);
            Assert.Contains("Example", html);
            Assert.Contains("class=\"language-javascript\"", html);
            Assert.Contains("console.log(&#39;hello&#39;);", html);
        }

        [Fact]
        public void ParseListing_ListingAttributeWithoutDelimiter_ParsesTextAsListing()
        {
            // Arrange
            var input = @"[listing]
This is simple listing content
without delimiters";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IListing>(document.Elements.First());
            
            var listing = (IListing)document.Elements.First();
            Assert.Contains("This is simple listing content", listing.Content);
            Assert.Contains("without delimiters", listing.Content);
        }
    }
}