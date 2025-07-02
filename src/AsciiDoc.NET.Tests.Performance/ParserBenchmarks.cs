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

namespace AsciiDoc.NET.Tests.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ParserBenchmarks
{
    private string _smallDocument = string.Empty;
    private string _mediumDocument = string.Empty;
    private string _largeDocument = string.Empty;
    private string _inlineHeavyDocument = string.Empty;
    
    private AsciiDocProcessor _processor = default!;

    [GlobalSetup]
    public void Setup()
    {
        _processor = new AsciiDocProcessor();
        
        // Small document (1KB)
        _smallDocument = GenerateDocument(50, 10);
        
        // Medium document (100KB)
        _mediumDocument = GenerateDocument(2000, 100);
        
        // Large document (1MB)
        _largeDocument = GenerateDocument(20000, 1000);
        
        // Inline-heavy document for testing inline parsing performance
        _inlineHeavyDocument = GenerateInlineHeavyDocument(1000);
    }

    [Benchmark]
    public void ParseSmallDocument()
    {
        _processor.ParseFromText(_smallDocument);
    }

    [Benchmark]
    public void ParseMediumDocument()
    {
        _processor.ParseFromText(_mediumDocument);
    }

    [Benchmark]
    public void ParseLargeDocument()
    {
        _processor.ParseFromText(_largeDocument);
    }

    [Benchmark]
    public void ParseInlineHeavyDocument()
    {
        _processor.ParseFromText(_inlineHeavyDocument);
    }

    private string GenerateDocument(int paragraphs, int wordsPerParagraph)
    {
        var sb = new StringBuilder();
        sb.AppendLine("= Performance Test Document");
        sb.AppendLine("Author Name");
        sb.AppendLine();
        
        for (int i = 0; i < paragraphs; i++)
        {
            sb.AppendLine($"== Section {i + 1}");
            sb.AppendLine();
            
            var paragraph = new StringBuilder();
            for (int j = 0; j < wordsPerParagraph; j++)
            {
                paragraph.Append($"word{j} ");
            }
            sb.AppendLine(paragraph.ToString().Trim());
            sb.AppendLine();
        }
        
        return sb.ToString();
    }

    private string GenerateInlineHeavyDocument(int lines)
    {
        var sb = new StringBuilder();
        sb.AppendLine("= Inline Heavy Document");
        sb.AppendLine("Author Name");
        sb.AppendLine();
        
        for (int i = 0; i < lines; i++)
        {
            sb.AppendLine($"This is line {i} with *bold text* and _italic text_ and `code text` and http://example.com[link text] and some more text.");
        }
        
        return sb.ToString();
    }
}