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

using AsciiDocSharp.Converters.Html;
using AsciiDocSharp.Converters.Core;
using AsciiDocSharp.Core.Implementation;
using AsciiDocSharp.Parser.Implementation;
using Xunit;

namespace AsciiDocSharp.Tests.Unit
{
    public class CrossReferenceHtmlConversionTests
    {
        [Fact]
        public void ConvertToHtml_AnchorElement_RendersAnchorTag()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var parser = new AsciiDocParser();
            var input = "This has an [[my-anchor]] anchor.";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<a id=\"my-anchor\"></a>", html);
            Assert.Contains("This has an <a id=\"my-anchor\"></a> anchor.", html);
        }

        [Fact]
        public void ConvertToHtml_AnchorWithLabel_RendersAnchorWithText()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var parser = new AsciiDocParser();
            var input = "This has [[section1,Section Label]] with text.";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<a id=\"section1\">Section Label</a>", html);
        }

        [Fact]
        public void ConvertToHtml_CrossReference_RendersLinkWithHash()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var parser = new AsciiDocParser();
            var input = "See <<target-section>> for details.";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<a href=\"#target-section\" class=\"xref\">target-section</a>", html);
        }

        [Fact]
        public void ConvertToHtml_CrossReferenceWithLinkText_RendersCustomText()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var parser = new AsciiDocParser();
            var input = "See <<target,Custom Link Text>> for details.";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<a href=\"#target\" class=\"xref\">Custom Link Text</a>", html);
        }

        [Fact]
        public void ConvertToHtml_CompleteAnchorAndCrossReference_RendersCorrectly()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var parser = new AsciiDocParser();
            var input = @"
[[introduction]] Introduction

This is the introduction section.

Reference to <<introduction,the intro>> section.";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            // Check anchor rendering (anchor ID comes first, then label as separate text)
            Assert.Contains("<a id=\"introduction\"></a> Introduction", html);
            
            // Check cross-reference rendering
            Assert.Contains("<a href=\"#introduction\" class=\"xref\">the intro</a>", html);
        }

        [Fact]
        public void ConvertToHtml_HtmlEscaping_EscapesSpecialCharacters()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var parser = new AsciiDocParser();
            var input = "See <<section&data,Link <with> \"quotes\">> for info.";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("href=\"#section&amp;data\"", html);
            Assert.Contains("Link &lt;with&gt; &quot;quotes&quot;", html);
        }

        [Fact]
        public void ConvertToHtml_DocumentWithCssStyles_IncludesCrossReferenceStyles()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var parser = new AsciiDocParser();
            var input = "Basic text with <<ref>> reference.";
            var options = new ConverterOptions { OutputFullDocument = true };

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);

            // Assert
            Assert.Contains(".xref { color: #2196f3; text-decoration: none; }", html);
            Assert.Contains(".xref:hover { text-decoration: underline; }", html);
        }

        [Fact]
        public void ConvertToHtml_MultipleReferencesInSameParagraph_RendersAll()
        {
            // Arrange
            var converter = new HtmlDocumentConverter();
            var parser = new AsciiDocParser();
            var input = "Start [[anchor1]] middle <<ref1>> and <<ref2,custom>> end.";

            // Act
            var document = parser.Parse(input);
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<a id=\"anchor1\"></a>", html);
            Assert.Contains("<a href=\"#ref1\" class=\"xref\">ref1</a>", html);
            Assert.Contains("<a href=\"#ref2\" class=\"xref\">custom</a>", html);
        }
    }
}