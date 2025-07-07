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
using AsciiDoc.Net.Parser.Implementation;

namespace AsciiDoc.Net.Tests.Unit
{
    public class IncludeDirectiveBasicTests
    {
        [Fact]
        public void IncludeProcessor_Creation_Succeeds()
        {
            var parser = new AsciiDocParser();
            var tokenizer = new AsciiDocTokenizer("");
            var includeProcessor = new IncludeProcessor(parser, tokenizer);
            
            Assert.NotNull(includeProcessor);
        }
        
        [Fact]
        public void ResolveIncludePath_AbsolutePath_ReturnsCorrectPath()
        {
            var parser = new AsciiDocParser();
            var tokenizer = new AsciiDocTokenizer("");
            var includeProcessor = new IncludeProcessor(parser, tokenizer);
            
            var absolutePath = Path.GetFullPath("/test/file.adoc");
            var result = includeProcessor.ResolveIncludePath(absolutePath, "/base");
            
            Assert.Equal(absolutePath, result);
        }
        
        [Fact]
        public void CircularReference_Detection_WorksCorrectly()
        {
            var parser = new AsciiDocParser();
            var tokenizer = new AsciiDocTokenizer("");
            var includeProcessor = new IncludeProcessor(parser, tokenizer);
            
            var filePath = "/test/file.adoc";
            var includeStack = new[] { filePath };
            
            var result = includeProcessor.WouldCreateCircularReference(filePath, includeStack);
            
            Assert.True(result);
        }
        
        [Fact]
        public void CircularReference_DifferentFiles_ReturnsFalse()
        {
            var parser = new AsciiDocParser();
            var tokenizer = new AsciiDocTokenizer("");
            var includeProcessor = new IncludeProcessor(parser, tokenizer);
            
            var filePath1 = "/test/file1.adoc";
            var filePath2 = "/test/file2.adoc";
            var includeStack = new[] { filePath1 };
            
            var result = includeProcessor.WouldCreateCircularReference(filePath2, includeStack);
            
            Assert.False(result);
        }
    }
}