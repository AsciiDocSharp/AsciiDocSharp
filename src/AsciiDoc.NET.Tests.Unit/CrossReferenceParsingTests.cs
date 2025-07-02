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

using AsciiDoc.NET.Core.Elements;
using AsciiDoc.NET.Core.Implementation;
using AsciiDoc.NET.Parser.Implementation;
using Xunit;

namespace AsciiDoc.NET.Tests.Unit
{
    public class CrossReferenceParsingTests
    {
        [Fact]
        public void Parse_AnchorInline_CreatesAnchorElement()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This is a paragraph with an [[anchor-id]] anchor.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.NotNull(document);
            Assert.Single(document.Elements);

            var paragraph = Assert.IsType<Paragraph>(document.Elements[0]);
            Assert.Equal(3, paragraph.Children.Count);

            // Text before anchor
            var textBefore = Assert.IsType<Text>(paragraph.Children[0]);
            Assert.Equal("This is a paragraph with an ", textBefore.Content);

            // Anchor element
            var anchor = Assert.IsType<Anchor>(paragraph.Children[1]);
            Assert.Equal("anchor-id", anchor.Id);
            Assert.Equal(string.Empty, anchor.Label);

            // Text after anchor
            var textAfter = Assert.IsType<Text>(paragraph.Children[2]);
            Assert.Equal(" anchor.", textAfter.Content);
        }

        [Fact]
        public void Parse_AnchorWithLabel_CreatesAnchorWithLabel()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This has an [[section1,Section One]] anchor with label.";

            // Act
            var document = parser.Parse(input);

            // Assert
            var paragraph = Assert.IsType<Paragraph>(document.Elements[0]);
            var anchor = Assert.IsType<Anchor>(paragraph.Children[1]);
            
            Assert.Equal("section1", anchor.Id);
            Assert.Equal("Section One", anchor.Label);
        }

        [Fact]
        public void Parse_CrossReference_CreatesCrossReferenceElement()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "See <<anchor-id>> for more details.";

            // Act
            var document = parser.Parse(input);

            // Assert
            var paragraph = Assert.IsType<Paragraph>(document.Elements[0]);
            Assert.Equal(3, paragraph.Children.Count);

            // Text before cross-reference
            var textBefore = Assert.IsType<Text>(paragraph.Children[0]);
            Assert.Equal("See ", textBefore.Content);

            // Cross-reference element
            var crossRef = Assert.IsType<CrossReference>(paragraph.Children[1]);
            Assert.Equal("anchor-id", crossRef.TargetId);
            Assert.Equal(string.Empty, crossRef.LinkText);

            // Text after cross-reference
            var textAfter = Assert.IsType<Text>(paragraph.Children[2]);
            Assert.Equal(" for more details.", textAfter.Content);
        }

        [Fact]
        public void Parse_CrossReferenceWithLinkText_CreatesCrossReferenceWithText()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "See <<section1,the first section>> for details.";

            // Act
            var document = parser.Parse(input);

            // Assert
            var paragraph = Assert.IsType<Paragraph>(document.Elements[0]);
            var crossRef = Assert.IsType<CrossReference>(paragraph.Children[1]);
            
            Assert.Equal("section1", crossRef.TargetId);
            Assert.Equal("the first section", crossRef.LinkText);
        }

        [Fact]
        public void Parse_MultipleAnchorsAndCrossReferences_ParsesAll()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = @"
[[intro]] Introduction

This is the introduction. See <<conclusion>> for the end.

[[conclusion]] Conclusion

This references <<intro,the introduction>>.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Equal(4, document.Elements.Count);

            // First paragraph with anchor
            var firstParagraph = Assert.IsType<Paragraph>(document.Elements[0]);
            var firstAnchor = Assert.IsType<Anchor>(firstParagraph.Children[0]);
            Assert.Equal("intro", firstAnchor.Id);

            // Second paragraph with cross-reference
            var secondParagraph = Assert.IsType<Paragraph>(document.Elements[1]);
            var firstCrossRef = Assert.IsType<CrossReference>(secondParagraph.Children[1]);
            Assert.Equal("conclusion", firstCrossRef.TargetId);

            // Third paragraph with anchor
            var thirdParagraph = Assert.IsType<Paragraph>(document.Elements[2]);
            var secondAnchor = Assert.IsType<Anchor>(thirdParagraph.Children[0]);
            Assert.Equal("conclusion", secondAnchor.Id);

            // Fourth paragraph with cross-reference with link text
            var fourthParagraph = Assert.IsType<Paragraph>(document.Elements[3]);
            var secondCrossRef = Assert.IsType<CrossReference>(fourthParagraph.Children[1]);
            Assert.Equal("intro", secondCrossRef.TargetId);
            Assert.Equal("the introduction", secondCrossRef.LinkText);
        }

        [Fact]
        public void Parse_AnchorAtStartOfLine_ParsesCorrectly()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "[[start]]Beginning of paragraph text.";

            // Act
            var document = parser.Parse(input);

            // Assert
            var paragraph = Assert.IsType<Paragraph>(document.Elements[0]);
            Assert.Equal(2, paragraph.Children.Count);

            var anchor = Assert.IsType<Anchor>(paragraph.Children[0]);
            Assert.Equal("start", anchor.Id);

            var text = Assert.IsType<Text>(paragraph.Children[1]);
            Assert.Equal("Beginning of paragraph text.", text.Content);
        }

        [Fact]
        public void Parse_CrossReferenceAtEndOfLine_ParsesCorrectly()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This paragraph ends with a reference <<end>>";

            // Act
            var document = parser.Parse(input);

            // Assert
            var paragraph = Assert.IsType<Paragraph>(document.Elements[0]);
            Assert.Equal(2, paragraph.Children.Count);

            var text = Assert.IsType<Text>(paragraph.Children[0]);
            Assert.Equal("This paragraph ends with a reference ", text.Content);

            var crossRef = Assert.IsType<CrossReference>(paragraph.Children[1]);
            Assert.Equal("end", crossRef.TargetId);
        }
    }
}