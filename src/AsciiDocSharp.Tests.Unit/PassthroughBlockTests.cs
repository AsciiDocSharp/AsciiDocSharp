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
    public class PassthroughBlockTests
    {
        [Fact]
        public void ParsePassthrough_DelimitedBlock_ParsesCorrectly()
        {
            // Arrange
            var input = @"++++
<video poster=""images/movie-reel.png"">
  <source src=""videos/writing-zen.webm"" type=""video/webm"">
</video>
++++";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.Single(document.Elements);
            
            var passthrough = document.Elements.First() as IPassthrough;
            Assert.NotNull(passthrough);
            Assert.Contains("video poster", passthrough.Content);
            Assert.Contains("source src", passthrough.Content);
            Assert.Null(passthrough.Title);
            Assert.Null(passthrough.Substitutions);
        }

        [Fact]
        public void ParsePassthrough_AttributeStyle_ParsesCorrectly()
        {
            // Arrange
            var input = @"[pass]
<del>strike this</del> is marked as deleted.";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.Single(document.Elements);
            
            var passthrough = document.Elements.First() as IPassthrough;
            Assert.NotNull(passthrough);
            Assert.Equal("<del>strike this</del> is marked as deleted.", passthrough.Content);
            Assert.Null(passthrough.Title);
            Assert.Null(passthrough.Substitutions);
        }

        [Fact]
        public void ParsePassthrough_AttributeWithDelimiter_ParsesCorrectly()
        {
            // Arrange
            var input = @"[pass]
++++
<script>
function hello() {
    alert('Hello World!');
}
</script>
++++";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.Single(document.Elements);
            
            var passthrough = document.Elements.First() as IPassthrough;
            Assert.NotNull(passthrough);
            Assert.Contains("<script>", passthrough.Content);
            Assert.Contains("function hello", passthrough.Content);
            Assert.Contains("alert('Hello World!')", passthrough.Content);
        }

        [Fact]
        public void ParsePassthrough_EmptyBlock_ParsesCorrectly()
        {
            // Arrange
            var input = @"++++
++++";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.Single(document.Elements);
            
            var passthrough = document.Elements.First() as IPassthrough;
            Assert.NotNull(passthrough);
            // Note: Empty blocks will contain a newline between the delimiters
            Assert.Equal("\n", passthrough.Content);
        }

        [Fact]
        public void ParsePassthrough_MultilineContent_PreservesFormatting()
        {
            // Arrange
            var input = @"++++
<div class=""custom"">
    <p>Line 1</p>
    <p>Line 2</p>
    
    <p>Line 4 after empty line</p>
</div>
++++";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.Single(document.Elements);
            
            var passthrough = document.Elements.First() as IPassthrough;
            Assert.NotNull(passthrough);
            
            // Verify that formatting and empty lines are preserved
            Assert.Contains("<div class=\"custom\">", passthrough.Content);
            Assert.Contains("<p>Line 1</p>", passthrough.Content);
            Assert.Contains("<p>Line 2</p>", passthrough.Content);
            Assert.Contains("<p>Line 4 after empty line</p>", passthrough.Content);
            // Verify empty lines are preserved (should contain multiple newlines)
            Assert.Contains("\n\n", passthrough.Content);
        }

        [Fact]
        public void ConvertPassthrough_DelimitedBlock_GeneratesRawHtml()
        {
            // Arrange
            var input = @"++++
<div class=""alert alert-info"">
    <strong>Note:</strong> This is raw HTML content.
</div>
++++";

            var parser = new AsciiDocParser();
            var document = parser.Parse(input);
            var converter = new HtmlDocumentConverter();

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"alert alert-info\">", html);
            Assert.Contains("<strong>Note:</strong> This is raw HTML content.", html);
            Assert.DoesNotContain("&lt;div", html); // Should not be HTML escaped
            Assert.DoesNotContain("&gt;", html); // Should not be HTML escaped
        }

        [Fact]
        public void ConvertPassthrough_AttributeStyle_GeneratesRawHtml()
        {
            // Arrange
            var input = @"[pass]
<mark>highlighted text</mark> with <code>inline code</code>";

            var parser = new AsciiDocParser();
            var document = parser.Parse(input);
            var converter = new HtmlDocumentConverter();

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<mark>highlighted text</mark>", html);
            Assert.Contains("<code>inline code</code>", html);
            Assert.DoesNotContain("&lt;mark", html); // Should not be HTML escaped
        }

        [Fact]
        public void ConvertPassthrough_WithTitle_IncludesTitleDiv()
        {
            // Arrange
            var passthrough = new Passthrough("<em>Raw content</em>", "Custom Title");
            var converter = new HtmlDocumentConverter();
            var document = new Document();
            document.AddChild(passthrough);

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"title\">", html);
            Assert.Contains("Custom Title", html);
            Assert.Contains("<em>Raw content</em>", html);
        }

        [Fact]
        public void ConvertPassthrough_ComplexHtmlStructure_PreservesStructure()
        {
            // Arrange
            var input = @"++++
<table class=""custom-table"">
    <thead>
        <tr><th>Column 1</th><th>Column 2</th></tr>
    </thead>
    <tbody>
        <tr><td>Data 1</td><td>Data 2</td></tr>
    </tbody>
</table>
++++";

            var parser = new AsciiDocParser();
            var document = parser.Parse(input);
            var converter = new HtmlDocumentConverter();

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<table class=\"custom-table\">", html);
            Assert.Contains("<thead>", html);
            Assert.Contains("<tbody>", html);
            Assert.Contains("<th>Column 1</th>", html);
            Assert.Contains("<td>Data 1</td>", html);
            
            // Verify structure is preserved without additional paragraph wrapping
            Assert.DoesNotContain("<p><table", html);
        }

        [Fact]
        public void ParsePassthrough_TokenizerRecognizesDelimiters_CreatesCorrectTokens()
        {
            // Arrange
            var input = "++++\nContent\n++++";
            var tokenizer = new AsciiDocTokenizer(input);

            // Act
            var tokens = tokenizer.Tokenize(input).ToList();

            // Assert
            Assert.Contains(tokens, t => t.Type == Parser.TokenType.PassthroughDelimiter && t.Value == "++++");
            Assert.Equal(2, tokens.Count(t => t.Type == Parser.TokenType.PassthroughDelimiter));
        }
    }
}