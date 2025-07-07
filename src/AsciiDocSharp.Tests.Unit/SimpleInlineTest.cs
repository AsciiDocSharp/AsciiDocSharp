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

namespace AsciiDocSharp.Tests.Unit
{
    public class SimpleInlineTest
    {
        [Fact]
        public void ParseSimpleText_ShouldWork()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "Simple text";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.Single(document.Children);
            
            var paragraph = document.Children[0];
            Assert.NotNull(paragraph);
        }

        [Fact]
        public void ParseEmphasisText_ShouldCreateInlineElements()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This is _emphasized_ text.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.Single(document.Children);
            
            var paragraph = document.Children[0];
            Assert.NotNull(paragraph);
            
            // Check if paragraph has children (inline elements)
            Assert.True(paragraph.Children.Count > 0);
        }
    }
}