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

using AsciiDoc.NET.Converters.Html;
using AsciiDoc.NET.Parser.Implementation;

namespace AsciiDoc.NET.Tests.Unit;

public class BlockQuoteHtmlConversionTests
{
    [Fact]
    public void ConvertBlockQuote_SimpleBlockQuote_GeneratesCorrectHtml()
    {
        var input = @"____
This is a block quote.
____";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<blockquote>", html);
        Assert.Contains("<p>This is a block quote.</p>", html);
        Assert.Contains("</blockquote>", html);
    }

    [Fact]
    public void ConvertBlockQuote_WithAttribution_IncludesCite()
    {
        var input = @"____
The quick brown fox jumps over the lazy dog.
-- Author Name
____";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<blockquote>", html);
        Assert.Contains("<p>The quick brown fox jumps over the lazy dog.</p>", html);
        Assert.Contains("<cite>Author Name</cite>", html);
        Assert.Contains("</blockquote>", html);
    }

    [Fact]
    public void ConvertBlockQuote_WithEscapableContent_EscapesHtml()
    {
        var input = @"____
This contains <html> & special characters.
____";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("&lt;html&gt; &amp; special", html);
    }
}