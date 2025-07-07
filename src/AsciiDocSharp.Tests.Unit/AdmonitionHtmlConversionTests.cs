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
using AsciiDocSharp.Parser.Implementation;

namespace AsciiDocSharp.Tests.Unit;

public class AdmonitionHtmlConversionTests
{
    [Fact]
    public void ConvertAdmonition_NoteAdmonition_GeneratesCorrectHtml()
    {
        var input = "NOTE: This is a note admonition.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<div class=\"admonitionblock note\">", html);
        Assert.Contains("<td class=\"icon\"><div class=\"title\">NOTE</div></td>", html);
        Assert.Contains("<p>This is a note admonition.</p>", html);
    }

    [Fact]
    public void ConvertAdmonition_TipAdmonition_GeneratesCorrectHtml()
    {
        var input = "TIP: This is a helpful tip.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<div class=\"admonitionblock tip\">", html);
        Assert.Contains("<td class=\"icon\"><div class=\"title\">TIP</div></td>", html);
        Assert.Contains("<p>This is a helpful tip.</p>", html);
    }

    [Fact]
    public void ConvertAdmonition_ImportantAdmonition_GeneratesCorrectHtml()
    {
        var input = "IMPORTANT: This is very important information.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<div class=\"admonitionblock important\">", html);
        Assert.Contains("<td class=\"icon\"><div class=\"title\">IMPORTANT</div></td>", html);
        Assert.Contains("<p>This is very important information.</p>", html);
    }

    [Fact]
    public void ConvertAdmonition_WarningAdmonition_GeneratesCorrectHtml()
    {
        var input = "WARNING: This is a warning message.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<div class=\"admonitionblock warning\">", html);
        Assert.Contains("<td class=\"icon\"><div class=\"title\">WARNING</div></td>", html);
        Assert.Contains("<p>This is a warning message.</p>", html);
    }

    [Fact]
    public void ConvertAdmonition_CautionAdmonition_GeneratesCorrectHtml()
    {
        var input = "CAUTION: Proceed with caution.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<div class=\"admonitionblock caution\">", html);
        Assert.Contains("<td class=\"icon\"><div class=\"title\">CAUTION</div></td>", html);
        Assert.Contains("<p>Proceed with caution.</p>", html);
    }

    [Fact]
    public void ConvertAdmonition_WithHtmlEscaping_EscapesCorrectly()
    {
        var input = "NOTE: This contains <html> & special characters.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("&lt;html&gt; &amp; special", html);
    }

    [Fact]
    public void ConvertAdmonition_MultipleAdmonitions_GeneratesAllCorrectly()
    {
        var input = @"NOTE: First note.
TIP: Second tip.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<div class=\"admonitionblock note\">", html);
        Assert.Contains("First note.", html);
        Assert.Contains("<div class=\"admonitionblock tip\">", html);
        Assert.Contains("Second tip.", html);
    }
}