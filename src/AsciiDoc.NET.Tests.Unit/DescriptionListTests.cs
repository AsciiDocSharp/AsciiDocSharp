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
using AsciiDoc.NET.Parser.Implementation;
using System.Linq;

namespace AsciiDoc.NET.Tests.Unit
{
    public class DescriptionListTests
    {
        private readonly AsciiDocParser _parser = new AsciiDocParser();

        [Fact]
        public void Should_Parse_Simple_Description_List()
        {
            // Arrange
            var input = @"CPU:: The brain of the computer.
RAM:: Random Access Memory for temporary storage.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var descriptionList = Assert.IsAssignableFrom<IDescriptionList>(document.Elements.First());
            Assert.Equal(ListType.Definition, descriptionList.Type);
            Assert.Equal(2, descriptionList.Children.Count());

            var firstItem = Assert.IsAssignableFrom<IDescriptionListItem>(descriptionList.Children.First());
            Assert.Equal("CPU", firstItem.Term);
            Assert.Equal("The brain of the computer.", firstItem.Description);

            var secondItem = Assert.IsAssignableFrom<IDescriptionListItem>(descriptionList.Children.Skip(1).First());
            Assert.Equal("RAM", secondItem.Term);
            Assert.Equal("Random Access Memory for temporary storage.", secondItem.Description);
        }

        [Fact]
        public void Should_Parse_Description_List_With_Empty_Description()
        {
            // Arrange
            var input = @"TBD::
TODO:: Task to be done later.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            var descriptionList = Assert.IsAssignableFrom<IDescriptionList>(document.Elements.First());
            var firstItem = Assert.IsAssignableFrom<IDescriptionListItem>(descriptionList.Children.First());
            Assert.Equal("TBD", firstItem.Term);
            Assert.Equal("", firstItem.Description);
        }

        [Fact]
        public void Should_Parse_Description_List_With_Complex_Terms()
        {
            // Arrange
            var input = @"Application Programming Interface (API):: A set of protocols and tools for building software.
Integrated Development Environment:: Software for writing and debugging code.";

            // Act
            var document = _parser.Parse(input);

            // Assert
            var descriptionList = Assert.IsAssignableFrom<IDescriptionList>(document.Elements.First());
            var firstItem = Assert.IsAssignableFrom<IDescriptionListItem>(descriptionList.Children.First());
            Assert.Equal("Application Programming Interface (API)", firstItem.Term);
            Assert.Equal("A set of protocols and tools for building software.", firstItem.Description);
        }
    }
}