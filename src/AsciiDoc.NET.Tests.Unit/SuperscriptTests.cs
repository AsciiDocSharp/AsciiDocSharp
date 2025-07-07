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

using Xunit;
using AsciiDoc.Net.Core;
using AsciiDoc.Net.Core.Elements;
using AsciiDoc.Net.Core.Implementation;
using AsciiDoc.Net.Parser.Implementation;
using AsciiDoc.Net.Converters.Html;
using System.Linq;

namespace AsciiDoc.Net.Tests.Unit
{
    public class SuperscriptTests
    {
        private readonly AsciiDocParser _parser = new AsciiDocParser();

        [Fact]
        public void Should_Parse_Simple_Superscript()
        {
            // Arrange
            var input = "The formula is E=mc^2^ where c is the speed of light.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            // Should have: Text("The formula is E=mc") + Superscript("2") + Text(" where c is the speed of light.")
            Assert.Equal(3, paragraph.Children.Count);
            
            var superscript = Assert.IsAssignableFrom<ISuperscript>(paragraph.Children[1]);
            Assert.Equal("2", superscript.Text);
        }

        [Fact]
        public void Should_Parse_Multiple_Superscripts()
        {
            // Arrange
            var input = "This is the 1^st^ example and 2^nd^ test.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            // Should have multiple elements including two superscripts
            var superscripts = paragraph.Children.OfType<ISuperscript>().ToList();
            Assert.Equal(2, superscripts.Count);
            Assert.Equal("st", superscripts[0].Text);
            Assert.Equal("nd", superscripts[1].Text);
        }

        [Fact]
        public void Should_Convert_Superscript_To_Html()
        {
            // Arrange
            var input = "The area is 5^2^ square meters.";
            var converter = new HtmlDocumentConverter();

            // Act
            var document = _parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<sup>2</sup>", html);
            Assert.Contains("The area is 5<sup>2</sup> square meters.", html);
        }

        [Fact]
        public void Should_Handle_Superscript_With_Multiple_Characters()
        {
            // Arrange
            var input = "The scientific notation is 10^23^ atoms.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            var superscript = paragraph.Children.OfType<ISuperscript>().First();
            Assert.Equal("23", superscript.Text);
        }

        [Fact]
        public void Should_Not_Parse_Unclosed_Superscript()
        {
            // Arrange
            var input = "This has an unclosed superscript ^test without closing.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            // Should not contain any superscript elements
            var superscripts = paragraph.Children.OfType<ISuperscript>().ToList();
            Assert.Empty(superscripts);
        }
    }
}