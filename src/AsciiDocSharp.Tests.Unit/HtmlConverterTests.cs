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
using AsciiDocSharp.Converters.Core;
using AsciiDocSharp.Core.Implementation;
using AsciiDocSharp.Core.Elements;

namespace AsciiDocSharp.Tests.Unit
{
    public class HtmlConverterTests
    {
        [Fact]
        public void Convert_SimpleDocument_WithDefaultOptions_ReturnsFlatHtml()
        {
            // Arrange
            var document = new Document();
            var converter = new HtmlDocumentConverter();

            // Act
            var result = converter.Convert(document);

            // Assert
            Assert.NotNull(result);
            // Default behavior should be flat output (no DOCTYPE, html, body tags)
            Assert.DoesNotContain("<!DOCTYPE html>", result);
            Assert.DoesNotContain("<html>", result);
            Assert.DoesNotContain("</html>", result);
            Assert.DoesNotContain("<body>", result);
            Assert.DoesNotContain("</body>", result);
        }

        [Fact]
        public void Convert_SimpleDocument_WithFullDocumentOption_ReturnsFullHtml()
        {
            // Arrange
            var document = new Document();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = true };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("<html>", result);
            Assert.Contains("</html>", result);
            Assert.Contains("<body>", result);
            Assert.Contains("</body>", result);
        }

        [Fact]
        public void Convert_DocumentWithTitle_FlatOutput_IncludesH1ButNotTitle()
        {
            // Arrange
            var header = new DocumentHeader("Test Document", "Author", "author@example.com");
            var document = new Document(header);
            var converter = new HtmlDocumentConverter();

            // Act
            var result = converter.Convert(document);

            // Assert
            // In flat output, we get the h1 but not the HTML title tag
            Assert.DoesNotContain("<title>Test Document</title>", result);
            Assert.Contains("<h1>Test Document</h1>", result);
            Assert.Contains("<div class=\"author\">Author</div>", result);
        }

        [Fact]
        public void Convert_DocumentWithTitle_FullOutput_IncludesBothTitleAndH1()
        {
            // Arrange
            var header = new DocumentHeader("Test Document", "Author", "author@example.com");
            var document = new Document(header);
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = true };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            // In full output, we get both HTML title tag and h1
            Assert.Contains("<title>Test Document</title>", result);
            Assert.Contains("<h1>Test Document</h1>", result);
            Assert.Contains("<div class=\"author\">Author</div>", result);
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

    /// <summary>
    /// Tests for the OutputFullDocument feature that controls whether to output
    /// complete HTML documents or just the content elements (flat output).
    /// </summary>
    public class OutputFullDocumentTests
    {
        [Fact]
        public void ConverterOptions_DefaultOutputFullDocument_IsFalse()
        {
            // Arrange & Act
            var options = new ConverterOptions();

            // Assert
            Assert.False(options.OutputFullDocument);
        }

        [Fact]
        public void Convert_FlatOutput_DoesNotIncludeDocumentWrapper()
        {
            // Arrange
            var document = new Document();
            var paragraph = new Paragraph("Hello World");
            document.AddChild(paragraph);
            
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = false };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            Assert.NotNull(result);
            Assert.DoesNotContain("<!DOCTYPE html>", result);
            Assert.DoesNotContain("<html>", result);
            Assert.DoesNotContain("<head>", result);
            Assert.DoesNotContain("<body>", result);
            Assert.DoesNotContain("</body>", result);
            Assert.DoesNotContain("</html>", result);
            
            // Should contain the actual content
            Assert.Contains("<p>Hello World</p>", result);
        }

        [Fact]
        public void Convert_FullDocumentOutput_IncludesCompleteHtmlStructure()
        {
            // Arrange
            var document = new Document();
            var paragraph = new Paragraph("Hello World");
            document.AddChild(paragraph);
            
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = true };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("<html>", result);
            Assert.Contains("<head>", result);
            Assert.Contains("<body>", result);
            Assert.Contains("</body>", result);
            Assert.Contains("</html>", result);
            
            // Should contain the actual content within the body
            Assert.Contains("<p>Hello World</p>", result);
        }

        [Fact]
        public void Convert_FullDocumentWithTitle_IncludesTitleInHead()
        {
            // Arrange
            var header = new DocumentHeader("My Document", "John Doe", "john@example.com");
            var document = new Document(header);
            var paragraph = new Paragraph("Content goes here");
            document.AddChild(paragraph);
            
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = true };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            Assert.Contains("<title>My Document</title>", result);
            Assert.Contains("<h1>My Document</h1>", result);
            Assert.Contains("<div class=\"author\">John Doe</div>", result);
        }

        [Fact]
        public void Convert_FlatOutputWithTitle_DoesNotIncludeTitleTag()
        {
            // Arrange
            var header = new DocumentHeader("My Document", "John Doe", "john@example.com");
            var document = new Document(header);
            var paragraph = new Paragraph("Content goes here");
            document.AddChild(paragraph);
            
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = false };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            Assert.DoesNotContain("<title>My Document</title>", result);
            Assert.Contains("<h1>My Document</h1>", result);
            Assert.Contains("<div class=\"author\">John Doe</div>", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Convert_PrettyPrintWithBothOutputModes_WorksCorrectly(bool outputFullDocument)
        {
            // Arrange
            var document = new Document();
            var paragraph = new Paragraph("Test content");
            document.AddChild(paragraph);
            
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions 
            { 
                OutputFullDocument = outputFullDocument,
                PrettyPrint = true
            };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<p>Test content</p>", result);
            
            if (outputFullDocument)
            {
                Assert.Contains("<!DOCTYPE html>", result);
                // Pretty print should include formatting
                Assert.Contains("\n", result);
            }
            else
            {
                Assert.DoesNotContain("<!DOCTYPE html>", result);
            }
        }

        [Fact]
        public void Convert_ComplexDocumentFlatOutput_ReturnsOnlyContent()
        {
            // Arrange
            var header = new DocumentHeader("Complex Doc", "Author", "author@test.com");
            var document = new Document(header);
            
            // Add various elements
            var section = new Section("Section 1", 1);
            var paragraph = new Paragraph("Some text with _emphasis_ and *strong*.");
            var codeBlock = new CodeBlock("console.log('test');", "javascript");
            
            document.AddChild(section);
            section.AddChild(paragraph);
            section.AddChild(codeBlock);
            
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = false };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            // Should contain all the content elements
            Assert.Contains("<h1>Complex Doc</h1>", result);
            Assert.Contains("<div class=\"author\">Author</div>", result);
            Assert.Contains("<h2>Section 1</h2>", result);
            Assert.Contains("<p>Some text", result);
            Assert.Contains("class=\"language-javascript\"", result);
            
            // Should NOT contain document wrapper
            Assert.DoesNotContain("<!DOCTYPE html>", result);
            Assert.DoesNotContain("<html>", result);
            Assert.DoesNotContain("<body>", result);
        }

        [Fact]
        public void Convert_ComplexDocumentFullOutput_ReturnsCompleteDocument()
        {
            // Arrange
            var header = new DocumentHeader("Complex Doc", "Author", "author@test.com");
            var document = new Document(header);
            
            // Add various elements
            var section = new Section("Section 1", 1);
            var paragraph = new Paragraph("Some text with _emphasis_ and *strong*.");
            var codeBlock = new CodeBlock("console.log('test');", "javascript");
            
            document.AddChild(section);
            section.AddChild(paragraph);
            section.AddChild(codeBlock);
            
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = true };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            // Should contain all the content elements
            Assert.Contains("<h1>Complex Doc</h1>", result);
            Assert.Contains("<div class=\"author\">Author</div>", result);
            Assert.Contains("<h2>Section 1</h2>", result);
            Assert.Contains("<p>Some text", result);
            Assert.Contains("class=\"language-javascript\"", result);
            
            // Should contain complete document structure
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("<html>", result);
            Assert.Contains("<head>", result);
            Assert.Contains("<title>Complex Doc</title>", result);
            Assert.Contains("<body>", result);
            Assert.Contains("</body>", result);
            Assert.Contains("</html>", result);
        }

        [Fact]
        public void Convert_EmptyDocumentFlatOutput_ReturnsEmptyString()
        {
            // Arrange
            var document = new Document();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = false };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result.Trim());
        }

        [Fact]
        public void Convert_EmptyDocumentFullOutput_ReturnsMinimalHtmlStructure()
        {
            // Arrange
            var document = new Document();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { OutputFullDocument = true };

            // Act
            var result = converter.Convert(document, options);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("<html>", result);
            Assert.Contains("<head>", result);
            Assert.Contains("<body>", result);
            Assert.Contains("</body>", result);
            Assert.Contains("</html>", result);
        }

        [Fact]
        public void AsciiDoc_ToHtml_WithFlatOutputOption_ReturnsFlatContent()
        {
            // Arrange
            var content = "= Test\n\nHello World";
            var options = new ConverterOptions { OutputFullDocument = false };

            // Act
            var result = AsciiDoc.ToHtml(content, options);

            // Assert
            Assert.DoesNotContain("<!DOCTYPE html>", result);
            Assert.DoesNotContain("<html>", result);
            Assert.Contains("<h1>Test</h1>", result);
            Assert.Contains("<p>Hello World</p>", result);
        }

        [Fact]
        public void AsciiDoc_ToHtml_WithFullDocumentOption_ReturnsCompleteDocument()
        {
            // Arrange
            var content = "= Test\n\nHello World";
            var options = new ConverterOptions { OutputFullDocument = true };

            // Act
            var result = AsciiDoc.ToHtml(content, options);

            // Assert
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("<html>", result);
            Assert.Contains("<title>Test</title>", result);
            Assert.Contains("<h1>Test</h1>", result);
            Assert.Contains("<p>Hello World</p>", result);
        }

        [Fact]
        public void AsciiDoc_ToHtml_DefaultBehavior_ReturnsFullDocument()
        {
            // Arrange
            var content = "= Test\n\nHello World";

            // Act
            var result = AsciiDoc.ToHtml(content);

            // Assert
            // The parameterless static method should maintain backward compatibility
            // and return full HTML documents
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("<html>", result);
            Assert.Contains("<title>Test</title>", result);
            Assert.Contains("<h1>Test</h1>", result);
            Assert.Contains("<p>Hello World</p>", result);
        }
    }
}