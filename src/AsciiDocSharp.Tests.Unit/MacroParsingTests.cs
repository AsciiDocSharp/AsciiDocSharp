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
using AsciiDocSharp.Core.Elements;
using AsciiDocSharp.Core.Implementation;

namespace AsciiDocSharp.Tests.Unit
{
    public class MacroParsingTests
    {
        [Fact]
        public void ParseBlockMacro_ImageMacro_ParsesCorrectly()
        {
            var input = "image::photo.jpg[A beautiful sunset]";
            var parser = new AsciiDocParser();
            
            var result = parser.ParseElement(input);
            
            Assert.NotNull(result);
            Assert.IsType<ImageMacro>(result);
            
            var imageMacro = (ImageMacro)result;
            Assert.Equal("image", imageMacro.Name);
            Assert.Equal("photo.jpg", imageMacro.Source);
            Assert.Equal("A beautiful sunset", imageMacro.Alt);
            Assert.Equal(MacroType.Block, imageMacro.MacroType);
        }

        [Fact]
        public void ParseBlockMacro_VideoMacro_ParsesCorrectly()
        {
            var input = "video::demo.mp4[Demo Video,width=640,height=480,controls=true]";
            var parser = new AsciiDocParser();
            
            var result = parser.ParseElement(input);
            
            Assert.NotNull(result);
            Assert.IsType<VideoMacro>(result);
            
            var videoMacro = (VideoMacro)result;
            Assert.Equal("video", videoMacro.Name);
            Assert.Equal("demo.mp4", videoMacro.Source);
            Assert.Equal("Demo Video", videoMacro.Title);
            Assert.Equal(640, videoMacro.Width);
            Assert.Equal(480, videoMacro.Height);
            Assert.True(videoMacro.Controls);
            Assert.Equal(MacroType.Block, videoMacro.MacroType);
        }

        [Fact]
        public void ParseBlockMacro_IncludeMacro_ParsesCorrectly()
        {
            var input = "include::chapter1.adoc[lines=1..10,tags=intro]";
            var parser = new AsciiDocParser();
            
            var result = parser.ParseElement(input);
            
            Assert.NotNull(result);
            Assert.IsType<IncludeMacro>(result);
            
            var includeMacro = (IncludeMacro)result;
            Assert.Equal("include", includeMacro.Name);
            Assert.Equal("chapter1.adoc", includeMacro.FilePath);
            Assert.Equal("1..10", includeMacro.Lines);
            Assert.Equal("intro", includeMacro.Tags);
            Assert.Equal(MacroType.Block, includeMacro.MacroType);
        }

        [Fact]
        public void ParseBlockMacro_CustomMacro_ParsesCorrectly()
        {
            var input = "custom::target[param1=value1,param2=value2]";
            var parser = new AsciiDocParser();
            
            var result = parser.ParseElement(input);
            
            Assert.NotNull(result);
            Assert.IsType<Macro>(result);
            
            var macro = (Macro)result;
            Assert.Equal("custom", macro.Name);
            Assert.Equal("target", macro.Target);
            Assert.Equal("value1", macro.Parameters["param1"]);
            Assert.Equal("value2", macro.Parameters["param2"]);
            Assert.Equal(MacroType.Block, macro.MacroType);
        }

        [Fact]
        public void ParseDocument_WithBlockMacros_ParsesAllCorrectly()
        {
            var input = @"= Document Title

This is a paragraph.

image::photo.jpg[A photo]

Another paragraph.

video::demo.mp4[Demo,width=640]";

            var parser = new AsciiDocParser();
            var document = parser.Parse(input);
            
            Assert.Equal("Document Title", document.Header.Title);
            Assert.Equal(4, document.Elements.Count);
            
            // First paragraph
            Assert.IsType<Paragraph>(document.Elements[0]);
            
            // Image macro
            Assert.IsType<ImageMacro>(document.Elements[1]);
            var imageMacro = (ImageMacro)document.Elements[1];
            Assert.Equal("photo.jpg", imageMacro.Source);
            
            // Second paragraph
            Assert.IsType<Paragraph>(document.Elements[2]);
            
            // Video macro
            Assert.IsType<VideoMacro>(document.Elements[3]);
            var videoMacro = (VideoMacro)document.Elements[3];
            Assert.Equal("demo.mp4", videoMacro.Source);
            Assert.Equal(640, videoMacro.Width);
        }

        [Fact]
        public void ParseMacroParameters_ComplexParameters_ParsesCorrectly()
        {
            var input = "image::photo.jpg[Alt text,width=300,height=200,title=\"My Photo\",link=http://example.com]";
            var parser = new AsciiDocParser();
            
            var result = parser.ParseElement(input);
            
            Assert.NotNull(result);
            Assert.IsType<ImageMacro>(result);
            
            var imageMacro = (ImageMacro)result;
            Assert.Equal("Alt text", imageMacro.Alt);
            Assert.Equal(300, imageMacro.Width);
            Assert.Equal(200, imageMacro.Height);
            Assert.Equal("My Photo", imageMacro.Title);
            Assert.Equal("http://example.com", imageMacro.Link);
        }

        [Fact]
        public void ParseMacroParameters_EmptyParameters_HandlesCorrectly()
        {
            var input = "image::photo.jpg[]";
            var parser = new AsciiDocParser();
            
            var result = parser.ParseElement(input);
            
            Assert.NotNull(result);
            Assert.IsType<ImageMacro>(result);
            
            var imageMacro = (ImageMacro)result;
            Assert.Equal("photo", imageMacro.Alt); // Default from filename
            Assert.Null(imageMacro.Width);
            Assert.Null(imageMacro.Height);
        }
    }
}