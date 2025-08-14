// AsciiDocSharp
// Copyright (c) 2025 Guillermo Espert
// Distributed under the MIT License. See LICENSE.adoc in the project root for full license information.

using AsciiDocSharp.Core.Implementation;
using AsciiDocSharp.Parser.Implementation;
using Xunit;

namespace AsciiDocSharp.Tests.Unit
{
    public class SimpleFormattingTest
    {
        [Fact]
        public void Parser_Should_Parse_Basic_Bold()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This is *bold* text.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.True(paragraph.Children.Count >= 1);
        }

        [Fact]
        public void Parser_Should_Parse_Basic_Italic()
        {
            // Arrange
            var parser = new AsciiDocParser();
            var input = "This is _italic_ text.";

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Children);
            var paragraph = Assert.IsType<Paragraph>(document.Children[0]);
            Assert.True(paragraph.Children.Count >= 1);
        }

        [Fact]
        public void Strong_Should_Support_Children()
        {
            // Test if Strong class supports children
            var strong = new Strong();
            var emphasis = new Emphasis("test");
            
            strong.AddChild(emphasis);
            
            Assert.Single(strong.Children);
            Assert.IsType<Emphasis>(strong.Children[0]);
        }

        [Fact]
        public void Emphasis_Should_Support_Children()
        {
            // Test if Emphasis class supports children
            var emphasis = new Emphasis();
            var text = new Text("test");
            
            emphasis.AddChild(text);
            
            Assert.Single(emphasis.Children);
            Assert.IsType<Text>(emphasis.Children[0]);
        }
    }
}