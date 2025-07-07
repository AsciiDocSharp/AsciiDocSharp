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
using AsciiDocSharp;
using AsciiDocSharp.Converters.Html;
using AsciiDocSharp.Core.Implementation;
using AsciiDocSharp.Core.Elements;

namespace AsciiDocSharp.Tests.Unit
{
    public class HtmlConverterTests
    {
        [Fact]
        public void Convert_SimpleDocument_ReturnsHtml()
        {
            // Arrange
            var document = new Document();
            var converter = new HtmlDocumentConverter();

            // Act
            var result = converter.Convert(document);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("<html>", result);
            Assert.Contains("</html>", result);
        }

        [Fact]
        public void Convert_DocumentWithTitle_IncludesTitleInHtml()
        {
            // Arrange
            var header = new DocumentHeader("Test Document", "Author", "author@example.com");
            var document = new Document(header);
            var converter = new HtmlDocumentConverter();

            // Act
            var result = converter.Convert(document);

            // Assert
            Assert.Contains("<title>Test Document</title>", result);
            Assert.Contains("<h1>Test Document</h1>", result);
        }

        [Fact]
        public void AsciiDoc_ToHtml_SimpleText_ReturnsHtml()
        {
            // Arrange
            var content = "Hello World";

            // Act
            var result = AsciiDoc.ToHtml(content);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("<html>", result);
            Assert.Contains("</html>", result);
        }

        [Theory]
        [InlineData("Hello World", "Hello World")]
        [InlineData("Test & Example", "Test &amp; Example")]
        [InlineData("<script>alert('xss')</script>", "&lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;")]
        public void Convert_EscapesHtmlProperly(string input, string expectedEscaped)
        {
            // Arrange
            var paragraph = new Paragraph(input);
            var document = new Document();
            document.AddChild(paragraph);
            var converter = new HtmlDocumentConverter();

            // Act
            var result = converter.Convert(document);

            // Assert
            Assert.Contains(expectedEscaped, result);
        }

        [Fact]
        public void Convert_CodeBlockWithoutLanguage_ReturnsHtmlWithoutLanguageClass()
        {
            // Arrange
            var codeBlock = new CodeBlock("public class Test { }");
            var document = new Document();
            document.AddChild(codeBlock);
            var converter = new HtmlDocumentConverter();

            // Act
            var result = converter.Convert(document);

            // Assert
            Assert.Contains("<pre><code>", result);
            Assert.Contains("public class Test { }", result);
            Assert.Contains("</code></pre>", result);
            Assert.DoesNotContain("class=\"language-", result);
        }

        [Fact]
        public void Convert_CodeBlockWithLanguage_ReturnsHtmlWithLanguageClass()
        {
            // Arrange
            var codeBlock = new CodeBlock("function hello() { console.log('Hello'); }", "javascript");
            var document = new Document();
            document.AddChild(codeBlock);
            var converter = new HtmlDocumentConverter();

            // Act
            var result = converter.Convert(document);

            // Assert
            Assert.Contains("<pre><code class=\"language-javascript\">", result);
            Assert.Contains("function hello()", result);
            Assert.Contains("</code></pre>", result);
        }

        [Theory]
        [InlineData("python", "def hello():", "language-python")]
        [InlineData("java", "public class Test", "language-java")]
        [InlineData("csharp", "public void Method()", "language-csharp")]
        [InlineData("sql", "SELECT * FROM users", "language-sql")]
        public void Convert_CodeBlockWithVariousLanguages_ReturnsCorrectLanguageClass(string language, string code, string expectedClass)
        {
            // Arrange
            var codeBlock = new CodeBlock(code, language);
            var document = new Document();
            document.AddChild(codeBlock);
            var converter = new HtmlDocumentConverter();

            // Act
            var result = converter.Convert(document);

            // Assert
            Assert.Contains($"class=\"{expectedClass}\"", result);
            Assert.Contains(code, result);
        }

        [Fact]
        public void AsciiDoc_ToHtml_CodeBlockWithSourceAttribute_ReturnsHtmlWithLanguage()
        {
            // Arrange
            var content = @"[source,javascript]
----
function hello() {
    console.log('Hello World');
}
----";

            // Act
            var result = AsciiDoc.ToHtml(content);

            // Assert
            Assert.Contains("class=\"language-javascript\"", result);
            Assert.Contains("function hello()", result);
            Assert.Contains("console.log(&#39;Hello World&#39;)", result); // HTML-escaped single quotes
        }
    }
}