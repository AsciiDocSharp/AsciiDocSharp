// AsciiDoc.NET
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
using AsciiDoc.NET.Parser.Implementation;
using AsciiDoc.NET.Core;
using Xunit.Abstractions;

namespace AsciiDoc.NET.Tests.Unit
{
    public class DebugParserTests
    {
        private readonly ITestOutputHelper _output;

        public DebugParserTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Debug_ParseSimpleHeader()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = "= Document Title";

            // Act
            var result = parser.Parse(content);

            // Assert and Debug
            _output.WriteLine($"Document: {result != null}");
            _output.WriteLine($"Header: {result?.Header != null}");
            _output.WriteLine($"Header Title: '{result?.Header?.Title}'");
            _output.WriteLine($"Elements Count: {result?.Elements?.Count ?? 0}");
            
            for (int i = 0; i < (result?.Elements?.Count ?? 0); i++)
            {
                var element = result.Elements[i];
                _output.WriteLine($"Element {i}: Type={element.GetType().Name}, ElementType={element.ElementType}");
                
                if (element is IParagraph para)
                    _output.WriteLine($"  Paragraph Text: '{para.Text}'");
                if (element is ISection section)
                    _output.WriteLine($"  Section Title: '{section.Title}', Level: {section.Level}");
            }

            Assert.NotNull(result);
        }

        [Fact]
        public void Debug_ParseMultipleLines()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var content = "= Document Title\n\nThis is a paragraph.";

            // Act
            var result = parser.Parse(content);

            // Assert and Debug
            _output.WriteLine($"Document: {result != null}");
            _output.WriteLine($"Header: {result?.Header != null}");
            _output.WriteLine($"Header Title: '{result?.Header?.Title}'");
            _output.WriteLine($"Elements Count: {result?.Elements?.Count ?? 0}");
            
            for (int i = 0; i < (result?.Elements?.Count ?? 0); i++)
            {
                var element = result.Elements[i];
                _output.WriteLine($"Element {i}: Type={element.GetType().Name}, ElementType={element.ElementType}");
                
                if (element is IParagraph para)
                    _output.WriteLine($"  Paragraph Text: '{para.Text}'");
                if (element is ISection section)
                    _output.WriteLine($"  Section Title: '{section.Title}', Level: {section.Level}");
            }

            Assert.NotNull(result);
        }
    }
}