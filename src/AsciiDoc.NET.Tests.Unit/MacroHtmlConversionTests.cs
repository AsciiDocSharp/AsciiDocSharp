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
using AsciiDoc.NET.Converters.Html;
using AsciiDoc.NET.Converters.Core;
using AsciiDoc.NET.Core.Elements;
using AsciiDoc.NET.Core.Implementation;

namespace AsciiDoc.NET.Tests.Unit
{
    public class MacroHtmlConversionTests
    {
        [Fact]
        public void ConvertImageMacro_BasicImage_RendersCorrectly()
        {
            var input = "image::photo.jpg[A beautiful sunset]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("<img src=\"photo.jpg\" alt=\"A beautiful sunset\" title=\"A beautiful sunset\"/>", html);
        }

        [Fact]
        public void ConvertImageMacro_WithDimensions_RendersCorrectly()
        {
            var input = "image::photo.jpg[Photo,width=300,height=200]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("src=\"photo.jpg\"", html);
            Assert.Contains("alt=\"Photo\"", html);
            Assert.Contains("width=\"300\"", html);
            Assert.Contains("height=\"200\"", html);
        }

        [Fact]
        public void ConvertImageMacro_WithTitle_RendersCorrectly()
        {
            var input = "image::photo.jpg[Photo,title=\"My beautiful photo\"]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("title=\"My beautiful photo\"", html);
        }

        [Fact]
        public void ConvertVideoMacro_BasicVideo_RendersCorrectly()
        {
            var input = "video::demo.mp4[Demo Video]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("<video", html);
            Assert.Contains("controls", html);
            Assert.Contains("<source src=\"demo.mp4\" type=\"video/mp4\">", html);
            Assert.Contains("Your browser does not support the video tag.", html);
            Assert.Contains("</video>", html);
            Assert.Contains("<div class=\"video-title\">Demo Video</div>", html);
        }

        [Fact]
        public void ConvertVideoMacro_WithDimensions_RendersCorrectly()
        {
            var input = "video::demo.mp4[Demo,width=640,height=480,autoplay=true,loop=true,muted=true]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("width=\"640\"", html);
            Assert.Contains("height=\"480\"", html);
            Assert.Contains("autoplay", html);
            Assert.Contains("loop", html);
            Assert.Contains("muted", html);
        }

        [Fact]
        public void ConvertIncludeMacro_BasicInclude_RendersPlaceholder()
        {
            var input = "include::chapter1.adoc[]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("<!-- Include: chapter1.adoc -->", html);
        }

        [Fact]
        public void ConvertIncludeMacro_WithParameters_RendersPlaceholder()
        {
            var input = "include::chapter1.adoc[lines=1..10,tags=intro]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("<!-- Include: chapter1.adoc", html);
            Assert.Contains("(lines: 1..10)", html);
            Assert.Contains("(tags: intro)", html);
        }

        [Fact]
        public void ConvertCustomMacro_GenericMacro_RendersCorrectly()
        {
            var input = "custom::target[param1=value1,param2=value2]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("<span class=\"macro custom\"", html);
            Assert.Contains("data-macro=\"custom\"", html);
            Assert.Contains("data-target=\"target\"", html);
            Assert.Contains("data-param1=\"value1\"", html);
            Assert.Contains("data-param2=\"value2\"", html);
            Assert.Contains("custom::target[param1=value1,param2=value2]", html);
            Assert.Contains("</span>", html);
        }

        [Fact]
        public void ConvertDocument_WithMultipleMacros_RendersAllCorrectly()
        {
            var input = @"= Document with Macros

Here is an image:

image::photo.jpg[Photo]

And a video:

video::demo.mp4[Demo]

And an include:

include::chapter.adoc[]";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            // Check that all macros are rendered
            Assert.Contains("<img src=\"photo.jpg\"", html);
            Assert.Contains("<video", html);
            Assert.Contains("<!-- Include: chapter.adoc -->", html);
            
            // Check that the document structure is maintained
            Assert.Contains("<h1>Document with Macros</h1>", html);
            Assert.Contains("Here is an image:", html);
            Assert.Contains("And a video:", html);
            Assert.Contains("And an include:", html);
        }

        [Fact]
        public void ConvertImageMacro_HtmlEscaping_EscapesCorrectly()
        {
            var input = "image::photo.jpg[Photo <with> & \"quotes\"]";
            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var options = new ConverterOptions { PrettyPrint = false };
            
            var document = parser.Parse(input);
            var html = converter.Convert(document, options);
            
            Assert.Contains("alt=\"Photo &lt;with&gt; &amp; &quot;quotes&quot;\"", html);
        }
    }
}