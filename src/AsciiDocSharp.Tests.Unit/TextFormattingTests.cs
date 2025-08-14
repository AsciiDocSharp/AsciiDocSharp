// AsciiDocSharp
// Copyright (c) 2025 Guillermo Espert
// Distributed under the MIT License. See LICENSE.adoc in the project root for full license information.

using AsciiDocSharp.Core.Implementation;
using AsciiDocSharp.Parser.Implementation;
using Xunit;

namespace AsciiDocSharp.Tests.Unit
{
    public class TextFormattingTests
    {
        [Fact]
        public void Parser_Should_Parse_Constrained_Bold()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This is *bold* text.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.Equal(3, paragraph.Children.Count);
            
            Assert.IsType<Text>(paragraph.Children[0]);
            var strong = Assert.IsType<Strong>(paragraph.Children[1]);
            Assert.Equal("bold", strong.Text);
            Assert.IsType<Text>(paragraph.Children[2]);
        }

        [Fact]
        public void Parser_Should_Parse_Constrained_Italic()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This is _italic_ text.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.Equal(3, paragraph.Children.Count);
            
            Assert.IsType<Text>(paragraph.Children[0]);
            var emphasis = Assert.IsType<Emphasis>(paragraph.Children[1]);
            Assert.Equal("italic", emphasis.Text);
            Assert.IsType<Text>(paragraph.Children[2]);
        }

        [Fact]
        public void Parser_Should_Parse_Unconstrained_Bold()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "The **man**ual page.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.Equal(3, paragraph.Children.Count);
            
            Assert.IsType<Text>(paragraph.Children[0]);
            var strong = Assert.IsType<Strong>(paragraph.Children[1]);
            Assert.Equal("man", strong.Text);
            Assert.IsType<Text>(paragraph.Children[2]);
        }

        [Fact]
        public void Parser_Should_Parse_Unconstrained_Italic()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "That is fan__tastic__!";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.Equal(3, paragraph.Children.Count);
            
            Assert.IsType<Text>(paragraph.Children[0]);
            var emphasis = Assert.IsType<Emphasis>(paragraph.Children[1]);
            Assert.Equal("tastic", emphasis.Text);
            Assert.IsType<Text>(paragraph.Children[2]);
        }

        [Fact]
        public void Parser_Should_Parse_Mixed_Bold_Italic()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "That *_really_* works.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.Equal(3, paragraph.Children.Count);
            
            Assert.IsType<Text>(paragraph.Children[0]);
            // This should be a Strong element containing an Emphasis element
            var strong = Assert.IsType<Strong>(paragraph.Children[1]);
            Assert.Single(strong.Children);
            var emphasis = Assert.IsType<Emphasis>(strong.Children[0]);
            Assert.Equal("really", emphasis.Text);
            Assert.IsType<Text>(paragraph.Children[2]);
        }

        [Fact]
        public void Parser_Should_Parse_Bounded_Italic()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "i__tali__cs";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            // Should have 3 parts: "i", italic "tali", "cs"
            Assert.Equal(3, paragraph.Children.Count);
            
            var firstText = Assert.IsType<Text>(paragraph.Children[0]);
            Assert.Equal("i", firstText.Content);
            
            var emphasis = Assert.IsType<Emphasis>(paragraph.Children[1]);
            Assert.Equal("tali", emphasis.Text);
            
            var lastText = Assert.IsType<Text>(paragraph.Children[2]);
            Assert.Equal("cs", lastText.Content);
        }

        [Fact]
        public void Parser_Should_Parse_Bounded_Bold()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "**b**ol**d**";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            // Should have 3 parts: bold "b", "ol", bold "d"
            Assert.Equal(3, paragraph.Children.Count);
            
            var firstBold = Assert.IsType<Strong>(paragraph.Children[0]);
            Assert.Equal("b", firstBold.Text);
            
            var middleText = Assert.IsType<Text>(paragraph.Children[1]);
            Assert.Equal("ol", middleText.Content);
            
            var secondBold = Assert.IsType<Strong>(paragraph.Children[2]);
            Assert.Equal("d", secondBold.Text);
        }

        [Fact]
        public void Parser_Should_Parse_Complex_Bounded_Example()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "**__bo__**un__ded__";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            // Expected: bold "bo", "un", italic "ded"
            Assert.Equal(3, paragraph.Children.Count);
            
            var bold = Assert.IsType<Strong>(paragraph.Children[0]);
            Assert.Equal("bo", bold.Text);
            
            var middleText = Assert.IsType<Text>(paragraph.Children[1]);
            Assert.Equal("un", middleText.Content);
            
            var italic = Assert.IsType<Emphasis>(paragraph.Children[2]);
            Assert.Equal("ded", italic.Text);
        }
    }
}