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

public class TableHtmlConversionTests
{
    [Fact]
    public void ConvertTable_SimpleTable_GeneratesCorrectHtml()
    {
        var input = @"|===
|Name |Age |City
|John |25 |New York
|Jane |30 |Chicago
|===";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<table class=\"tableblock frame-all grid-all\">", html);
        Assert.Contains("<tbody>", html);
        Assert.Contains("<tr><td>Name</td><td>Age</td><td>City</td></tr>", html);
        Assert.Contains("<tr><td>John</td><td>25</td><td>New York</td></tr>", html);
        Assert.Contains("<tr><td>Jane</td><td>30</td><td>Chicago</td></tr>", html);
        Assert.Contains("</tbody>", html);
        Assert.Contains("</table>", html);
    }

    [Fact]
    public void ConvertTable_WithEscapableContent_EscapesHtml()
    {
        var input = @"|===
|Name |Description
|<John> |Uses & symbols
|===";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);
        
        var converter = new HtmlDocumentConverter();
        var html = converter.Convert(document);

        Assert.Contains("<td>&lt;John&gt;</td>", html);
        Assert.Contains("<td>Uses &amp; symbols</td>", html);
    }
}