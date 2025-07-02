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

using AsciiDoc.NET.Parser.Implementation;

namespace AsciiDoc.NET.Tests.Unit;

public class AttributeParsingTests
{
    [Fact]
    public void Parse_DocumentAttributes_StoresInDocumentAttributes()
    {
        var input = @":author: John Doe
:email: john@example.com
:doctype: book

= Document Title

This is a paragraph.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        Assert.Equal("John Doe", document.Attributes.GetAttribute("author"));
        Assert.Equal("john@example.com", document.Attributes.GetAttribute("email"));
        Assert.Equal("book", document.Attributes.GetAttribute("doctype"));
    }

    [Fact]
    public void Parse_BooleanAttributes_HandlesCorrectly()
    {
        var input = @":numbered:
:toc-placement!:

= Document Title

Content here.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        Assert.Equal("true", document.Attributes.GetAttribute("numbered"));
        Assert.Equal("false", document.Attributes.GetAttribute("toc-placement"));
    }

    [Fact]
    public void Parse_AttributesWithSpaces_HandlesCorrectly()
    {
        var input = @":title: My Document Title
:author: John Smith Jr.

= Document Title

Content here.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        Assert.Equal("My Document Title", document.Attributes.GetAttribute("title"));
        Assert.Equal("John Smith Jr.", document.Attributes.GetAttribute("author"));
    }
}