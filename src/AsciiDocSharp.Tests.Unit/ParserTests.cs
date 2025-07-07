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
using AsciiDocSharp.Parser.Implementation;
using AsciiDocSharp.Core;
using AsciiDocSharp.Core.Elements;

namespace AsciiDocSharp.Tests.Unit
{
    public class ParserTests
    {
        [Fact]
        public void Parse_EmptyString_ThrowsArgumentException()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => parser.Parse(content));
        }

        [Fact]
        public void Parse_SimpleHeader_ReturnsDocumentWithTitle()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = "= Document Title";

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Header);
            Assert.Equal("Document Title", result.Header.Title);
        }

        [Fact]
        public void Parse_SimpleParagraph_ReturnsDocumentWithParagraph()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = "This is a simple paragraph.";

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Elements);
            Assert.IsType<Core.Implementation.Paragraph>(result.Elements[0]);
            var paragraph = (IParagraph)result.Elements[0];
            Assert.Equal("This is a simple paragraph.", paragraph.Text);
        }

        [Theory]
        [InlineData("= Level 1 Header", 1)]
        [InlineData("== Level 2 Header", 2)]
        [InlineData("=== Level 3 Header", 3)]
        [InlineData("==== Level 4 Header", 4)]
        public void Parse_Headers_ReturnsCorrectLevel(string content, int expectedLevel)
        {
            // Arrange
            var parser = new AsciiDocParser();

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            if (expectedLevel == 1)
            {
                Assert.NotNull(result.Header);
                Assert.Equal("Level 1 Header", result.Header.Title);
            }
            else
            {
                Assert.Single(result.Elements);
                Assert.IsType<Core.Implementation.Section>(result.Elements[0]);
                var section = (ISection)result.Elements[0];
                Assert.Equal(expectedLevel, section.Level);
            }
        }

        [Fact]
        public void Parse_BasicCodeBlock_ReturnsCodeBlockWithoutLanguage()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = @"----
public class Test
{
    public void Method() { }
}
----";

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Elements);
            Assert.IsType<Core.Implementation.CodeBlock>(result.Elements[0]);
            var codeBlock = (ICodeBlock)result.Elements[0];
            Assert.Contains("public class Test", codeBlock.Content);
            Assert.Null(codeBlock.Language);
        }

        [Fact]
        public void Parse_CodeBlockWithLanguageTag_ReturnsCodeBlockWithLanguage()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = @"----java
public class Test
{
    public void Method() { }
}
----";

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Elements);
            Assert.IsType<Core.Implementation.CodeBlock>(result.Elements[0]);
            var codeBlock = (ICodeBlock)result.Elements[0];
            Assert.Contains("public class Test", codeBlock.Content);
            Assert.Equal("java", codeBlock.Language);
        }

        [Fact]
        public void Parse_CodeBlockWithSourceAttribute_ReturnsCodeBlockWithLanguage()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = @"[source,javascript]
----
function hello() {
    console.log('Hello World');
}
----";

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Elements);
            Assert.IsType<Core.Implementation.CodeBlock>(result.Elements[0]);
            var codeBlock = (ICodeBlock)result.Elements[0];
            Assert.Contains("function hello()", codeBlock.Content);
            Assert.Equal("javascript", codeBlock.Language);
        }

        [Fact]
        public void Parse_CodeBlockWithSourceAttributeNoLanguage_ReturnsCodeBlockWithoutLanguage()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = @"[source]
----
Some generic code here
without specific language
----";

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Elements);
            Assert.IsType<Core.Implementation.CodeBlock>(result.Elements[0]);
            var codeBlock = (ICodeBlock)result.Elements[0];
            Assert.Contains("Some generic code here", codeBlock.Content);
            Assert.Null(codeBlock.Language);
        }

        [Theory]
        [InlineData("python", "def hello():\n    print('Hello World')")]
        [InlineData("csharp", "public class Test\n{\n    public void Method() { }\n}")]
        [InlineData("sql", "SELECT * FROM users WHERE active = 1")]
        public void Parse_CodeBlockWithVariousLanguages_ReturnsCorrectLanguage(string language, string code)
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = $@"[source,{language}]
----
{code}
----";

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Elements);
            Assert.IsType<Core.Implementation.CodeBlock>(result.Elements[0]);
            var codeBlock = (ICodeBlock)result.Elements[0];
            Assert.Contains(code.Split('\n')[0], codeBlock.Content);
            Assert.Equal(language, codeBlock.Language);
        }

        [Fact]
        public void Parse_AttributeBlockWithoutCodeBlock_ReturnsParagraph()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = "[note]\nThis is just a note, not a code block.";

            // Act
            var result = parser.Parse(content);

            // Assert
            Assert.NotNull(result);
            // The parser should create one paragraph for the text content
            // The [note] attribute block without a following code block becomes a paragraph element
            Assert.Single(result.Elements);
            Assert.IsType<Core.Implementation.Paragraph>(result.Elements[0]);
            
            // Verify the paragraph contains the expected text
            var paragraph = (IParagraph)result.Elements[0];
            Assert.Contains("This is just a note", paragraph.Text);
        }
    }
}