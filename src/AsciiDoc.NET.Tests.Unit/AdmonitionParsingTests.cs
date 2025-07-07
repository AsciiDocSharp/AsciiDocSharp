// AsciiDoc.Net
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

using AsciiDoc.Net.Core.Elements;
using AsciiDoc.Net.Parser.Implementation;

namespace AsciiDoc.Net.Tests.Unit;

public class AdmonitionParsingTests
{
    [Fact]
    public void Parse_NoteAdmonition_CreatesNoteElement()
    {
        var input = "NOTE: This is a note admonition.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        Assert.Single(document.Elements);
        var admonition = document.Elements[0] as IAdmonition;
        Assert.NotNull(admonition);
        Assert.Equal(AdmonitionType.Note, admonition.Type);
        Assert.Equal("This is a note admonition.", admonition.Content);
    }

    [Fact]
    public void Parse_TipAdmonition_CreatesTipElement()
    {
        var input = "TIP: This is a helpful tip.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        var admonition = document.Elements[0] as IAdmonition;
        Assert.NotNull(admonition);
        Assert.Equal(AdmonitionType.Tip, admonition.Type);
        Assert.Equal("This is a helpful tip.", admonition.Content);
    }

    [Fact]
    public void Parse_ImportantAdmonition_CreatesImportantElement()
    {
        var input = "IMPORTANT: This is very important information.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        var admonition = document.Elements[0] as IAdmonition;
        Assert.NotNull(admonition);
        Assert.Equal(AdmonitionType.Important, admonition.Type);
        Assert.Equal("This is very important information.", admonition.Content);
    }

    [Fact]
    public void Parse_WarningAdmonition_CreatesWarningElement()
    {
        var input = "WARNING: This is a warning message.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        var admonition = document.Elements[0] as IAdmonition;
        Assert.NotNull(admonition);
        Assert.Equal(AdmonitionType.Warning, admonition.Type);
        Assert.Equal("This is a warning message.", admonition.Content);
    }

    [Fact]
    public void Parse_CautionAdmonition_CreatesCautionElement()
    {
        var input = "CAUTION: Proceed with caution.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        var admonition = document.Elements[0] as IAdmonition;
        Assert.NotNull(admonition);
        Assert.Equal(AdmonitionType.Caution, admonition.Type);
        Assert.Equal("Proceed with caution.", admonition.Content);
    }

    [Fact]
    public void Parse_AdmonitionWithExtraSpaces_HandlesCorrectly()
    {
        var input = "NOTE:   This has extra spaces.   ";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        var admonition = document.Elements[0] as IAdmonition;
        Assert.NotNull(admonition);
        Assert.Equal(AdmonitionType.Note, admonition.Type);
        Assert.Equal("This has extra spaces.", admonition.Content);
    }

    [Fact]
    public void Parse_MultipleAdmonitions_CreatesMultipleElements()
    {
        var input = @"NOTE: First note.
TIP: Second tip.
WARNING: Third warning.";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        Assert.Equal(3, document.Elements.Count);
        
        var note = document.Elements[0] as IAdmonition;
        Assert.Equal(AdmonitionType.Note, note.Type);
        Assert.Equal("First note.", note.Content);

        var tip = document.Elements[1] as IAdmonition;
        Assert.Equal(AdmonitionType.Tip, tip.Type);
        Assert.Equal("Second tip.", tip.Content);

        var warning = document.Elements[2] as IAdmonition;
        Assert.Equal(AdmonitionType.Warning, warning.Type);
        Assert.Equal("Third warning.", warning.Content);
    }
}