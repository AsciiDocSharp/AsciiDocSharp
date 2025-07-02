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
using AsciiDoc.NET.Parser.Implementation;

namespace AsciiDoc.NET.Tests.Unit;

public class BlockQuoteParsingTests
{
    [Fact]
    public void Parse_SimpleBlockQuote_CreatesBlockQuoteElement()
    {
        var input = @"____
This is a block quote.
It spans multiple lines.
____";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        Assert.Single(document.Elements);
        var blockQuote = document.Elements[0] as IBlockQuote;
        Assert.NotNull(blockQuote);
        Assert.Contains("This is a block quote.", blockQuote.Content);
        Assert.Contains("It spans multiple lines.", blockQuote.Content);
    }

    [Fact]
    public void Parse_BlockQuoteWithAttribution_ParsesAttribution()
    {
        var input = @"____
The quick brown fox jumps over the lazy dog.
-- Author Name
____";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        var blockQuote = document.Elements[0] as IBlockQuote;
        Assert.NotNull(blockQuote);
        Assert.Equal("The quick brown fox jumps over the lazy dog.", blockQuote.Content);
        Assert.Equal("Author Name", blockQuote.Attribution);
    }

    [Fact]
    public void Parse_SingleLineBlockQuote_HandlesCorrectly()
    {
        var input = @"____
This is a single line quote.
____";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        var blockQuote = document.Elements[0] as IBlockQuote;
        Assert.NotNull(blockQuote);
        Assert.Equal("This is a single line quote.", blockQuote.Content);
    }
}