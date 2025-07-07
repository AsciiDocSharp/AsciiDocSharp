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

using System;
using System.IO;
using Xunit;
using AsciiDoc.Net.Parser;
using AsciiDoc.Net.Parser.Implementation;
using AsciiDoc.Net.Converters.Html;
using AsciiDoc.Net.Converters.Core;
using AsciiDoc.Net.Core;

namespace AsciiDoc.Net.Tests.Unit
{
    public class IncludeDirectiveIntegrationTests
    {
        private readonly string _testDirectory;

        public IncludeDirectiveIntegrationTests()
        {
            // Create a temporary test directory
            _testDirectory = Path.Combine(Path.GetTempPath(), "AsciiDocIncludeIntegrationTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public void ParseDocument_WithIncludeDirective_IncludesContent()
        {
            // Create included file
            var includeFile = Path.Combine(_testDirectory, "included.adoc");
            File.WriteAllText(includeFile, "This is included content.\n\nAnother paragraph from include.");
            
            // Create main file with include directive
            var mainFile = Path.Combine(_testDirectory, "main.adoc");
            var mainContent = @"= Main Document

This is the main document.

include::included.adoc[]

This is after the include.";
            File.WriteAllText(mainFile, mainContent);
            
            // Parse the main document
            var parser = new AsciiDocParser();
            var tokenizer = new AsciiDocTokenizer(mainContent);
            var context = new ParseContext(tokenizer, mainFile);
            
            var elements = new System.Collections.Generic.List<IDocumentElement>();
            while (context.CurrentToken.Type != TokenType.EndOfFile)
            {
                var element = parser.ParseElement(context);
                if (element != null)
                {
                    elements.Add(element);
                }

                context.Advance();
            }
            
            // Should have more elements due to include
            Assert.True(elements.Count >= 3); // Document header + main content + included content
        }

        [Fact]
        public void FullWorkflow_MainWithInclude_ConvertsToHtml()
        {
            // Create included file
            var includeFile = Path.Combine(_testDirectory, "chapter1.adoc");
            File.WriteAllText(includeFile, @"== Chapter One

This is the first chapter.

=== Section 1.1

Some content in section 1.1.");
            
            // Create main file with include directive
            var mainFile = Path.Combine(_testDirectory, "book.adoc");
            var mainContent = @"= My Book

Welcome to my book.

include::chapter1.adoc[]

That was chapter one.";
            File.WriteAllText(mainFile, mainContent);
            
            // Parse and convert to HTML
            var parser = new AsciiDocParser();
            var document = parser.ParseFile(mainFile);
            
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions();
            var html = converter.Convert(document, options);
            
            // Verify HTML contains included content
            Assert.Contains("Chapter One", html);
            Assert.Contains("first chapter", html);
            Assert.Contains("Section 1.1", html);
            Assert.Contains("That was chapter one", html);
        }

        [Fact]
        public void ParseDocument_WithCircularInclude_ThrowsException()
        {
            // Create files with circular reference
            var file1 = Path.Combine(_testDirectory, "circular1.adoc");
            var file2 = Path.Combine(_testDirectory, "circular2.adoc");
            
            File.WriteAllText(file1, @"= File 1

This is file 1.

include::circular2.adoc[]");
            
            File.WriteAllText(file2, @"= File 2

This is file 2.

include::circular1.adoc[]");
            
            var parser = new AsciiDocParser();
            
            // Should throw ParseException for circular reference when using ParseFile (which processes includes)
            var exception = Assert.Throws<ParseException>(() => parser.ParseFile(file1));
            Assert.Contains("Circular include detected", exception.Message);
        }

        [Fact]
        public void ParseDocument_WithOptionalInclude_HandlesNonExistent()
        {
            var mainContent = @"= Main Document

Before include.

include::nonexistent.adoc[optional=true]

After include.";
            
            var parser = new AsciiDocParser();
            
            // Should not crash for missing optional include
            var document = parser.Parse(mainContent);
            Assert.NotNull(document);
            Assert.True(document.Elements.Count >= 2); // Before and after paragraphs
        }

        [Fact]
        public void ParseDocument_WithLineFilteredInclude_IncludesSpecificLines()
        {
            // Create file with multiple lines
            var includeFile = Path.Combine(_testDirectory, "multiline.adoc");
            var content = @"Line 1 - ignore
Line 2 - include
Line 3 - include  
Line 4 - ignore
Line 5 - ignore";
            File.WriteAllText(includeFile, content);
            
            var mainContent = $@"= Main Document

include::{Path.GetFileName(includeFile)}[lines=2..3]

End of document.";
            
            var parser = new AsciiDocParser();
            var options = new ParserOptions(_testDirectory);
            var document = parser.Parse(mainContent, options);
            
            // Convert to HTML to check content
            var converter = new HtmlDocumentConverter();
            var html = converter.Convert(document);
            
            Assert.Contains("Line 2 - include", html);
            Assert.Contains("Line 3 - include", html);
            Assert.DoesNotContain("Line 1 - ignore", html);
            Assert.DoesNotContain("Line 4 - ignore", html);
        }

        [Fact]
        public void ParseDocument_WithTaggedInclude_IncludesTaggedContent()
        {
            // Create file with tagged content
            var includeFile = Path.Combine(_testDirectory, "tagged.adoc");
            var content = @"This content is not tagged.

// tag::important[]
This is important content that should be included.

Very important paragraph.
// end::important[]

This content is also not tagged.

// tag::other[]
This is other tagged content.
// end::other[]

More untagged content.";
            File.WriteAllText(includeFile, content);
            
            var mainContent = $@"= Main Document

include::{Path.GetFileName(includeFile)}[tags=important]

End of document.";
            
            var parser = new AsciiDocParser();
            var options = new ParserOptions(_testDirectory);
            var document = parser.Parse(mainContent, options);
            
            // Convert to HTML to check content
            var converter = new HtmlDocumentConverter();
            var html = converter.Convert(document);
            
            Assert.Contains("important content that should be included", html);
            Assert.Contains("Very important paragraph", html);
            Assert.DoesNotContain("This content is not tagged", html);
            Assert.DoesNotContain("This is other tagged content", html);
        }

        private void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}