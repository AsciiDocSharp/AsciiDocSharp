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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AsciiDoc.NET.Core;
using AsciiDoc.NET.Core.Elements;

namespace AsciiDoc.NET.Parser.Implementation
{
    /// <summary>
    /// Processes include directives with file inclusion, circular reference detection, and path resolution.
    /// </summary>
    public class IncludeProcessor : IIncludeProcessor
    {
        private readonly IAsciiDocParser _parser;
        private readonly ITokenizer _tokenizer;

        public IncludeProcessor(IAsciiDocParser parser, ITokenizer tokenizer)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        }

        public IReadOnlyList<IDocumentElement> ProcessInclude(IIncludeMacro includeMacro, string basePath, IReadOnlyList<string> includeStack)
        {
            if (includeMacro == null)
                throw new ArgumentNullException(nameof(includeMacro));

            var resolvedPath = ResolveIncludePath(includeMacro.FilePath, basePath);
            
            // Check for circular references
            if (WouldCreateCircularReference(resolvedPath, includeStack))
            {
                var includeChain = string.Join(" -> ", includeStack.Select(Path.GetFileName));
                var circularFile = Path.GetFileName(resolvedPath);
                throw new ParseException($"Circular include detected: {includeChain} -> {circularFile}. " +
                                       $"File '{circularFile}' is already being processed in the include chain.", 0, 0);
            }

            // Check if file exists
            if (!File.Exists(resolvedPath))
            {
                if (includeMacro.Optional)
                {
                    // Return empty list for optional includes that don't exist
                    return new List<IDocumentElement>();
                }
                throw new ParseException($"Include file not found: {resolvedPath}", 0, 0);
            }

            try
            {
                // Read the file content
                var content = File.ReadAllText(resolvedPath);
                
                // Apply line filtering if specified
                content = ApplyLineFiltering(content, includeMacro.Lines);
                
                // Apply tag filtering if specified  
                content = ApplyTagFiltering(content, includeMacro.Tags);
                
                // Apply indentation if specified
                content = ApplyIndentation(content, includeMacro.IndentLevel);
                
                // Parse the content with updated include stack
                var newIncludeStack = new List<string>(includeStack) { resolvedPath };
                var elements = ParseIncludeContent(content, resolvedPath, newIncludeStack);
                
                // Apply level offset if specified
                elements = ApplyLevelOffset(elements, includeMacro.LevelOffset);
                
                return elements;
            }
            catch (IOException ex)
            {
                if (includeMacro.Optional)
                {
                    // Return empty list for optional includes that can't be read
                    return new List<IDocumentElement>();
                }
                throw new ParseException($"Error reading include file '{resolvedPath}': {ex.Message}", 0, 0);
            }
        }

        public string ResolveIncludePath(string filePath, string basePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            // If absolute path, return as-is
            if (Path.IsPathRooted(filePath))
                return Path.GetFullPath(filePath);

            // If no base path provided, use current directory
            if (string.IsNullOrEmpty(basePath))
                basePath = Directory.GetCurrentDirectory();

            // If base path is a file, get its directory
            if (File.Exists(basePath))
                basePath = Path.GetDirectoryName(basePath);

            // Combine and normalize the path
            var combinedPath = Path.Combine(basePath, filePath);
            return Path.GetFullPath(combinedPath);
        }

        public bool WouldCreateCircularReference(string filePath, IReadOnlyList<string> includeStack)
        {
            if (string.IsNullOrEmpty(filePath) || includeStack == null)
                return false;

            var normalizedPath = Path.GetFullPath(filePath);
            return includeStack.Any(path => string.Equals(Path.GetFullPath(path), normalizedPath, StringComparison.OrdinalIgnoreCase));
        }

        private string ApplyLineFiltering(string content, string linesSpec)
        {
            if (string.IsNullOrEmpty(linesSpec))
                return content;

            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var filteredLines = new List<string>();

            foreach (var range in linesSpec.Split(','))
            {
                var trimmedRange = range.Trim();
                if (trimmedRange.Contains(".."))
                {
                    // Range: "1..5" or "10..-1" (end)
                    var parts = trimmedRange.Split(new[] { ".." }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        var start = ParseLineNumber(parts[0], lines.Length);
                        var end = ParseLineNumber(parts[1], lines.Length);
                        
                        if (start > 0 && end > 0 && start <= end)
                        {
                            for (int i = start - 1; i < end && i < lines.Length; i++)
                            {
                                filteredLines.Add(lines[i]);
                            }
                        }
                    }
                }
                else
                {
                    // Single line: "5"
                    var lineNum = ParseLineNumber(trimmedRange, lines.Length);
                    if (lineNum > 0 && lineNum <= lines.Length)
                    {
                        filteredLines.Add(lines[lineNum - 1]);
                    }
                }
            }

            return string.Join(Environment.NewLine, filteredLines);
        }

        private int ParseLineNumber(string lineSpec, int totalLines)
        {
            if (string.IsNullOrEmpty(lineSpec))
                return 0;

            if (lineSpec == "-1")
                return totalLines;

            if (int.TryParse(lineSpec, out var lineNum))
                return lineNum;

            return 0;
        }

        private string ApplyTagFiltering(string content, string tagsSpec)
        {
            if (string.IsNullOrEmpty(tagsSpec))
                return content;

            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var filteredLines = new List<string>();
            var tags = tagsSpec.Split(',').Select(t => t.Trim()).ToList();
            
            var inIncludedTag = false;
            var currentTags = new HashSet<string>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Check for tag start: // tag::tagname[]
                if (trimmedLine.StartsWith("// tag::") && trimmedLine.EndsWith("[]"))
                {
                    var tagName = trimmedLine.Substring(8, trimmedLine.Length - 10);
                    currentTags.Add(tagName);
                    
                    if (tags.Contains(tagName))
                    {
                        inIncludedTag = true;
                    }
                    continue; // Don't include the tag marker itself
                }
                
                // Check for tag end: // end::tagname[]
                if (trimmedLine.StartsWith("// end::") && trimmedLine.EndsWith("[]"))
                {
                    var tagName = trimmedLine.Substring(8, trimmedLine.Length - 10);
                    currentTags.Remove(tagName);
                    
                    if (tags.Contains(tagName))
                    {
                        inIncludedTag = currentTags.Any(t => tags.Contains(t));
                    }
                    continue; // Don't include the tag marker itself
                }
                
                // Include line if we're in an included tag
                if (inIncludedTag)
                {
                    filteredLines.Add(line);
                }
            }

            return string.Join(Environment.NewLine, filteredLines);
        }

        private string ApplyIndentation(string content, string indentSpec)
        {
            if (string.IsNullOrEmpty(indentSpec))
                return content;

            if (!int.TryParse(indentSpec, out var indentLevel) || indentLevel <= 0)
                return content;

            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var indentedLines = lines.Select(line => new string(' ', indentLevel) + line);
            
            return string.Join(Environment.NewLine, indentedLines);
        }

        private IReadOnlyList<IDocumentElement> ParseIncludeContent(string content, string filePath, IReadOnlyList<string> includeStack)
        {
            // Create a new tokenizer for the content
            var tokenizer = new AsciiDocTokenizer(content);
            
            // Create a parse context for the include content
            var context = new ParseContext(tokenizer, filePath, includeStack);
            
            // Parse the content into elements using the current parser
            var elements = new List<IDocumentElement>();
            while (context.CurrentToken.Type != TokenType.EndOfFile)
            {
                var element = _parser.ParseElement(context);
                if (element != null)
                {
                    elements.Add(element);
                }

                context.Advance();
            }
            
            return elements;
        }

        private IReadOnlyList<IDocumentElement> ApplyLevelOffset(IReadOnlyList<IDocumentElement> elements, string levelOffsetSpec)
        {
            if (string.IsNullOrEmpty(levelOffsetSpec))
                return elements;

            if (!int.TryParse(levelOffsetSpec.TrimStart('+'), out var offset))
                return elements;

            // Apply level offset to all section elements
            var modifiedElements = new List<IDocumentElement>();
            foreach (var element in elements)
            {
                if (element is ISection section)
                {
                    // Create a new section with adjusted level
                    var newLevel = Math.Max(1, section.Level + offset);
                    var newSection = new AsciiDoc.NET.Core.Implementation.Section(section.Title, newLevel);
                    
                    // Copy children
                    foreach (var child in section.Children)
                    {
                        newSection.AddChild(child);
                    }
                    
                    modifiedElements.Add(newSection);
                }
                else
                {
                    modifiedElements.Add(element);
                }
            }
            
            return modifiedElements;
        }
    }
}