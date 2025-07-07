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

using AsciiDoc.NET.Core;
using AsciiDoc.NET.Core.Elements;
using AsciiDoc.NET.Core.Implementation;
using AsciiDoc.NET.Parser.Implementation;
using AsciiDoc.NET.Converters.Html;
using System.Linq;
using Xunit;

namespace AsciiDoc.NET.Tests.Unit
{
    public class VerseBlockTests
    {
        [Fact]
        public void ParseVerse_BasicVerseWithAttributes_ParsesCorrectly()
        {
            // Arrange
            var input = @"[verse, Carl Sandburg, Chicago]
____
Hog Butcher for the World,
Tool Maker, Stacker of Wheat,
Player with Railroads and the Nation's Freight Handler;
Stormy, husky, brawling,
City of Big Shoulders
____";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IVerse>(document.Elements.First());
            
            var verse = (IVerse)document.Elements.First();
            Assert.Contains("Hog Butcher for the World", verse.Content);
            Assert.Contains("Tool Maker, Stacker of Wheat", verse.Content);
            Assert.Contains("Player with Railroads and the Nation's Freight Handler", verse.Content);
            Assert.Equal("Carl Sandburg", verse.Author);
            Assert.Equal("Chicago", verse.Citation);
        }

        [Fact]
        public void ParseVerse_BasicVerseWithoutAttributes_ParsesCorrectly()
        {
            // Arrange
            var input = @"[verse]
____
Roses are red,
Violets are blue,
AsciiDoc is great,
And so are you!
____";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IVerse>(document.Elements.First());
            
            var verse = (IVerse)document.Elements.First();
            Assert.Contains("Roses are red", verse.Content);
            Assert.Contains("Violets are blue", verse.Content);
            Assert.Contains("AsciiDoc is great", verse.Content);
            Assert.Contains("And so are you!", verse.Content);
            Assert.Null(verse.Author);
            Assert.Null(verse.Citation);
        }

        [Fact]
        public void ParseVerse_VerseWithLineBreaks_PreservesFormatting()
        {
            // Arrange
            var input = @"[verse]
____
Line one
Line two

Line four after blank line
____";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            var verse = (IVerse)document.Elements.First();
            
            // Verify line breaks are preserved
            var lines = verse.Content.Split('\n');
            Assert.Equal(4, lines.Length);
            Assert.Equal("Line one", lines[0].Trim());
            Assert.Equal("Line two", lines[1].Trim());
            Assert.Equal("", lines[2].Trim()); // Empty line preserved
            Assert.Equal("Line four after blank line", lines[3].Trim());
        }

        [Fact]
        public void ConvertVerse_VerseWithAuthorAndCitation_GeneratesCorrectHtml()
        {
            // Arrange
            var input = @"[verse, Carl Sandburg, Chicago]
____
Hog Butcher for the World,
Tool Maker, Stacker of Wheat,
Player with Railroads and the Nation's Freight Handler
____";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"verseblock\">", html);
            Assert.Contains("<pre class=\"content\">", html);
            Assert.Contains("Hog Butcher for the World", html);
            Assert.Contains("Tool Maker, Stacker of Wheat", html);
            Assert.Contains("Player with Railroads and the Nation&#39;s Freight Handler", html);
            Assert.Contains("</pre>", html);
            Assert.Contains("<div class=\"attribution\">", html);
            Assert.Contains("Carl Sandburg", html);
            Assert.Contains("<cite>Chicago</cite>", html);
            Assert.Contains("</div>", html);
        }

        [Fact]
        public void ConvertVerse_VerseWithTitle_IncludesTitle()
        {
            // Arrange - Create verse directly since we don't have title parsing yet
            var verse = new Verse("Poetry content\nLine two", "My Poem", "Author", "Source");
            var converter = new HtmlDocumentConverter();
            var document = new Document();
            document.AddChild(verse);

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<div class=\"verseblock\">", html);
            Assert.Contains("<div class=\"title\">", html);
            Assert.Contains("My Poem", html);
            Assert.Contains("Poetry content", html);
            Assert.Contains("Line two", html);
        }
    }
}