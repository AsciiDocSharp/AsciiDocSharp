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
using AsciiDoc.NET.Core;
using AsciiDoc.NET.Core.Elements;
using AsciiDoc.NET.Core.Implementation;
using AsciiDoc.NET.Parser.Implementation;
using AsciiDoc.NET.Converters.Html;
using System.Linq;

namespace AsciiDoc.NET.Tests.Unit
{
    public class SubscriptTests
    {
        private readonly AsciiDocParser _parser = new AsciiDocParser();

        [Fact]
        public void Should_Parse_Simple_Subscript()
        {
            // Arrange
            var input = "The chemical formula for water is H~2~O.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            // Should have: Text("The chemical formula for water is H") + Subscript("2") + Text("O.")
            Assert.Equal(3, paragraph.Children.Count);
            
            var subscript = Assert.IsAssignableFrom<ISubscript>(paragraph.Children[1]);
            Assert.Equal("2", subscript.Text);
        }

        [Fact]
        public void Should_Parse_Multiple_Subscripts()
        {
            // Arrange
            var input = "The array elements x~i~ and y~j~ are important.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            // Should have multiple elements including two subscripts
            var subscripts = paragraph.Children.OfType<ISubscript>().ToList();
            Assert.Equal(2, subscripts.Count);
            Assert.Equal("i", subscripts[0].Text);
            Assert.Equal("j", subscripts[1].Text);
        }

        [Fact]
        public void Should_Convert_Subscript_To_Html()
        {
            // Arrange
            var input = "The molecule CO~2~ is carbon dioxide.";
            var converter = new HtmlDocumentConverter();

            // Act
            var document = _parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<sub>2</sub>", html);
            Assert.Contains("The molecule CO<sub>2</sub> is carbon dioxide.", html);
        }

        [Fact]
        public void Should_Handle_Subscript_With_Multiple_Characters()
        {
            // Arrange
            var input = "The chemical formula C~6~H~12~O~6~ represents glucose.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            var subscripts = paragraph.Children.OfType<ISubscript>().ToList();
            Assert.Equal(3, subscripts.Count);
            Assert.Equal("6", subscripts[0].Text);
            Assert.Equal("12", subscripts[1].Text);
            Assert.Equal("6", subscripts[2].Text);
        }

        [Fact]
        public void Should_Not_Parse_Unclosed_Subscript()
        {
            // Arrange
            var input = "This has an unclosed subscript ~test without closing.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            // Should not contain any subscript elements
            var subscripts = paragraph.Children.OfType<ISubscript>().ToList();
            Assert.Empty(subscripts);
        }

        [Fact]
        public void Should_Handle_Mixed_Super_And_Subscript()
        {
            // Arrange
            var input = "The equation x^2^ + y~i~ = z is complex.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var paragraph = Assert.IsAssignableFrom<IParagraph>(document.Elements.First());
            
            var superscripts = paragraph.Children.OfType<ISuperscript>().ToList();
            var subscripts = paragraph.Children.OfType<ISubscript>().ToList();
            
            Assert.Single(superscripts);
            Assert.Single(subscripts);
            Assert.Equal("2", superscripts[0].Text);
            Assert.Equal("i", subscripts[0].Text);
        }
    }
}