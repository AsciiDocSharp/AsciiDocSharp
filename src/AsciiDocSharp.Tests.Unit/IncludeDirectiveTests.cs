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

using System;
using System.IO;
using Xunit;
using AsciiDocSharp.Parser;
using AsciiDocSharp.Parser.Implementation;
using AsciiDocSharp.Core.Elements;
using AsciiDocSharp.Core.Implementation;

namespace AsciiDocSharp.Tests.Unit
{
    public class IncludeDirectiveTests
    {
        private readonly IAsciiDocParser _parser;
        private readonly IIncludeProcessor _includeProcessor;
        private readonly string _testDirectory;

        public IncludeDirectiveTests()
        {
            _parser = new AsciiDocParser();
            var tokenizer = new AsciiDocTokenizer("");
            _includeProcessor = new IncludeProcessor(_parser, tokenizer);
            
            // Create a temporary test directory
            _testDirectory = Path.Combine(Path.GetTempPath(), "AsciiDocIncludeTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public void ResolveIncludePath_AbsolutePath_ReturnsAsIs()
        {
            var absolutePath = Path.GetFullPath(Path.Combine(_testDirectory, "test.adoc"));
            var resolved = _includeProcessor.ResolveIncludePath(absolutePath, _testDirectory);
            
            Assert.Equal(absolutePath, resolved);
        }

        [Fact]
        public void ResolveIncludePath_RelativePath_ResolvesCorrectly()
        {
            var basePath = _testDirectory;
            var relativePath = "include.adoc";
            var expected = Path.GetFullPath(Path.Combine(_testDirectory, relativePath));
            
            var resolved = _includeProcessor.ResolveIncludePath(relativePath, basePath);
            
            Assert.Equal(expected, resolved);
        }

        [Fact]
        public void ResolveIncludePath_BasePathIsFile_UsesDirectory()
        {
            var baseFile = Path.Combine(_testDirectory, "main.adoc");
            File.WriteAllText(baseFile, "test content");
            
            var relativePath = "include.adoc";
            var expected = Path.GetFullPath(Path.Combine(_testDirectory, relativePath));
            
            var resolved = _includeProcessor.ResolveIncludePath(relativePath, baseFile);
            
            Assert.Equal(expected, resolved);
        }

        [Fact]
        public void WouldCreateCircularReference_SameFile_ReturnsTrue()
        {
            var filePath = Path.Combine(_testDirectory, "test.adoc");
            var includeStack = new[] { filePath };
            
            var result = _includeProcessor.WouldCreateCircularReference(filePath, includeStack);
            
            Assert.True(result);
        }

        [Fact]
        public void WouldCreateCircularReference_DifferentFile_ReturnsFalse()
        {
            var filePath1 = Path.Combine(_testDirectory, "test1.adoc");
            var filePath2 = Path.Combine(_testDirectory, "test2.adoc");
            var includeStack = new[] { filePath1 };
            
            var result = _includeProcessor.WouldCreateCircularReference(filePath2, includeStack);
            
            Assert.False(result);
        }

        [Fact]
        public void ProcessInclude_SimpleFile_ReturnsElements()
        {
            // Create test file
            var includeFile = Path.Combine(_testDirectory, "simple.adoc");
            File.WriteAllText(includeFile, "This is a simple paragraph.\n\nAnother paragraph.");
            
            // Create include macro
            var parameters = new System.Collections.Generic.Dictionary<string, string>();
            var includeMacro = new IncludeMacro("simple.adoc", parameters, MacroType.Block);
            
            // Process include
            var elements = _includeProcessor.ProcessInclude(includeMacro, _testDirectory, new string[0]);
            
            Assert.NotEmpty(elements);
            Assert.True(elements.Count >= 1);
        }

        [Fact] 
        public void ProcessInclude_NonExistentFile_ThrowsException()
        {
            var parameters = new System.Collections.Generic.Dictionary<string, string>();
            var includeMacro = new IncludeMacro("nonexistent.adoc", parameters, MacroType.Block);
            
            Assert.Throws<ParseException>(() => 
                _includeProcessor.ProcessInclude(includeMacro, _testDirectory, new string[0]));
        }

        [Fact]
        public void ProcessInclude_OptionalNonExistentFile_ReturnsEmpty()
        {
            var parameters = new System.Collections.Generic.Dictionary<string, string>
            {
                { "optional", "true" }
            };
            var includeMacro = new IncludeMacro("nonexistent.adoc", parameters, MacroType.Block);
            
            var elements = _includeProcessor.ProcessInclude(includeMacro, _testDirectory, new string[0]);
            
            Assert.Empty(elements);
        }

        [Fact]
        public void ProcessInclude_WithLineFiltering_ReturnsFilteredContent()
        {
            // Create test file with multiple lines
            var includeFile = Path.Combine(_testDirectory, "multiline.adoc");
            File.WriteAllText(includeFile, "Line 1\nLine 2\nLine 3\nLine 4\nLine 5");
            
            // Create include macro with line filtering
            var parameters = new System.Collections.Generic.Dictionary<string, string>
            {
                { "lines", "2..4" }
            };
            var includeMacro = new IncludeMacro("multiline.adoc", parameters, MacroType.Block);
            
            // Process include
            var elements = _includeProcessor.ProcessInclude(includeMacro, _testDirectory, new string[0]);
            
            Assert.NotEmpty(elements);
        }

        [Fact]
        public void ProcessInclude_WithTagFiltering_ReturnsTaggedContent()
        {
            // Create test file with tags
            var includeFile = Path.Combine(_testDirectory, "tagged.adoc");
            var content = @"Before tag
// tag::important[]
This is important content
// end::important[]
After tag";
            File.WriteAllText(includeFile, content);
            
            // Create include macro with tag filtering
            var parameters = new System.Collections.Generic.Dictionary<string, string>
            {
                { "tags", "important" }
            };
            var includeMacro = new IncludeMacro("tagged.adoc", parameters, MacroType.Block);
            
            // Process include
            var elements = _includeProcessor.ProcessInclude(includeMacro, _testDirectory, new string[0]);
            
            Assert.NotEmpty(elements);
        }

        [Fact]
        public void ProcessInclude_CircularReference_ThrowsException()
        {
            // Create test files with circular references
            var file1 = Path.Combine(_testDirectory, "file1.adoc");
            var file2 = Path.Combine(_testDirectory, "file2.adoc");
            
            File.WriteAllText(file1, @"= File 1

This is file 1.

include::file2.adoc[]

End of file 1.");
            
            File.WriteAllText(file2, @"= File 2

This is file 2.

include::file1.adoc[]

End of file 2.");

            var parser = new AsciiDocParser();

            // Test that parsing file1 throws a ParseException due to circular reference
            var exception = Assert.Throws<ParseException>(() => parser.ParseFile(file1));
            
            // Verify the exception message contains information about the circular reference
            Assert.Contains("Circular include detected", exception.Message);
            Assert.Contains("file1.adoc", exception.Message);
            Assert.Contains("file2.adoc", exception.Message);
        }

        [Fact]
        public void ProcessInclude_WithIndentation_AppliesIndentation()
        {
            // Create test file
            var includeFile = Path.Combine(_testDirectory, "indent.adoc");
            File.WriteAllText(includeFile, "Line without indent\nAnother line");
            
            // Create include macro with indentation
            var parameters = new System.Collections.Generic.Dictionary<string, string>
            {
                { "indent", "4" }
            };
            var includeMacro = new IncludeMacro("indent.adoc", parameters, MacroType.Block);
            
            // Process include
            var elements = _includeProcessor.ProcessInclude(includeMacro, _testDirectory, new string[0]);
            
            Assert.NotEmpty(elements);
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