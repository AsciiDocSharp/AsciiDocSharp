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

using AsciiDocSharp.Core.Elements;
using AsciiDocSharp.Parser.Implementation;

namespace AsciiDocSharp.Tests.Unit;

public class TableParsingTests
{
    [Fact]
    public void Parse_SimpleTable_CreatesTableElement()
    {
        var input = @"|===
|Name |Age |City
|John |25 |New York
|Jane |30 |Chicago
|===";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        Assert.Single(document.Elements);
        var table = document.Elements[0] as ITable;
        Assert.NotNull(table);
        Assert.Equal(3, table.Rows.Count);
        
        // Check first row
        var firstRow = table.Rows[0];
        Assert.Equal(3, firstRow.Cells.Count);
        Assert.Equal("Name", firstRow.Cells[0].Content);
        Assert.Equal("Age", firstRow.Cells[1].Content);
        Assert.Equal("City", firstRow.Cells[2].Content);
    }

    [Fact]
    public void Parse_TableWithSpaces_HandlesSpacesCorrectly()
    {
        var input = @"|===
|First Name |Last Name |Email Address
|John Doe |Smith |john@example.com
|===";

        var parser = new AsciiDocParser();
        var document = parser.Parse(input);

        var table = document.Elements[0] as ITable;
        Assert.NotNull(table);
        Assert.Equal(2, table.Rows.Count);
        
        var firstRow = table.Rows[0];
        Assert.Equal("First Name", firstRow.Cells[0].Content);
        Assert.Equal("Last Name", firstRow.Cells[1].Content);
        Assert.Equal("Email Address", firstRow.Cells[2].Content);
        
        var secondRow = table.Rows[1];
        Assert.Equal("John Doe", secondRow.Cells[0].Content);
        Assert.Equal("Smith", secondRow.Cells[1].Content);
        Assert.Equal("john@example.com", secondRow.Cells[2].Content);
    }
}