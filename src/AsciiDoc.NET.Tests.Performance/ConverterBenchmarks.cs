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
public class ConverterBenchmarks
{
    private IDocument _smallDocument = default!;
    private IDocument _mediumDocument = default!;
    private IDocument _largeDocument = default!;
    
    private HtmlDocumentConverter _htmlConverter = default!;
    private AsciiDocProcessor _processor = default!;

    [GlobalSetup]
    public void Setup()
    {
        _processor = new AsciiDocProcessor();
        _htmlConverter = new HtmlDocumentConverter();
        
        // Parse documents once for conversion benchmarks
        _smallDocument = _processor.ParseFromText(GenerateDocument(50, 10));
        _mediumDocument = _processor.ParseFromText(GenerateDocument(2000, 100));
        _largeDocument = _processor.ParseFromText(GenerateDocument(20000, 1000));
    }

    [Benchmark]
    public string ConvertSmallDocumentToHtml()
    {
        return _htmlConverter.Convert(_smallDocument);
    }

    [Benchmark]
    public string ConvertMediumDocumentToHtml()
    {
        return _htmlConverter.Convert(_mediumDocument);
    }

    [Benchmark]
    public string ConvertLargeDocumentToHtml()
    {
        return _htmlConverter.Convert(_largeDocument);
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
}