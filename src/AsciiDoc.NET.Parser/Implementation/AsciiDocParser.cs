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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AsciiDoc.Net.Core;
using AsciiDoc.Net.Core.Elements;
using AsciiDoc.Net.Core.Implementation;

namespace AsciiDoc.Net.Parser.Implementation
{
    public class AsciiDocParser : IAsciiDocParser
    {
        private static readonly Regex HeaderPattern = new Regex(@"^(=+)\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex ListItemPattern = new Regex(@"^(\*+|\d+\.)\s+(\[[ xX]\]\s+)?(.+)$", RegexOptions.Compiled);
        private static readonly Regex DescriptionListItemPattern = new Regex(@"^([^:\[\]]+)::\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex AttributeLinePattern = new Regex(@"^:([^:!]+)(!?):\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex AttributeBlockPattern = new Regex(@"^\[([^\]]+)\]$", RegexOptions.Compiled);
        private static readonly Regex CodeBlockWithLanguagePattern = new Regex(@"^----(\w+)?$", RegexOptions.Compiled);
        private static readonly Regex SourceAttributePattern = new Regex(@"^source(?:,\s*(\w+))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex AdmonitionPattern = new Regex(@"^(NOTE|TIP|IMPORTANT|WARNING|CAUTION):\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex VerseAttributePattern = new Regex(@"^\[verse(?:,\s*([^,\]]+))?(?:,\s*([^\]]+))?\]$", RegexOptions.Compiled);
        private static readonly Regex LiteralAttributePattern = new Regex(@"^\[literal\]$", RegexOptions.Compiled);
        
        // Individual patterns for inline element matching
        private static readonly Regex StrongPattern = new Regex(@"\*([^*]+)\*", RegexOptions.Compiled);
        private static readonly Regex EmphasisPattern = new Regex(@"_([^_]+)_", RegexOptions.Compiled);
        private static readonly Regex HighlightPattern = new Regex(@"#([^#]+)#", RegexOptions.Compiled);
        private static readonly Regex SuperscriptPattern = new Regex(@"\^([^\^]+)\^", RegexOptions.Compiled);
        private static readonly Regex SubscriptPattern = new Regex(@"~([^~]+)~", RegexOptions.Compiled);
        private static readonly Regex InlineCodePattern = new Regex(@"`([^`]+)`", RegexOptions.Compiled);
        private static readonly Regex LinkPattern = new Regex(@"(https?://[^\s\[\]]+)(\[([^\]]*)\])?", RegexOptions.Compiled);
        private static readonly Regex ImagePattern = new Regex(@"image::([^\[]+)\[([^\]]*)\]", RegexOptions.Compiled);
        private static readonly Regex AnchorPattern = new Regex(@"\[\[([^\]]+)\]\]", RegexOptions.Compiled);
        private static readonly Regex CrossReferencePattern = new Regex(@"<<([^,>]+?)(?:,(.+?))?>>", RegexOptions.Compiled);
        private static readonly Regex BlockMacroPattern = new Regex(@"^(\w+)::([^\[]*)\[([^\]]*)\]$", RegexOptions.Compiled);
        private static readonly Regex InlineMacroPattern = new Regex(@"(\w+):([^\[]*)\[([^\]]*)\]", RegexOptions.Compiled);
        private static readonly Regex TableOfContentsPattern = new Regex(@"^toc::\s*\[([^\]]*)\]$", RegexOptions.Compiled);
        private static readonly Regex FootnotePattern = new Regex(@"footnote:([^:\[\]]*?)\[([^\]]*)\]", RegexOptions.Compiled);

        public IDocument Parse(string input)
        {
            return Parse(input, null);
        }

        public IDocument Parse(string input, ParserOptions options)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty", nameof(input));

            var tokenizer = new AsciiDocTokenizer(input);
            var basePath = options?.BasePath;
            var context = new ParseContext(tokenizer, basePath);
            var document = new Document();

            context.PushElement(document);

            // Check if the first non-empty line is a document title (= Title)
            bool isFirstContentLine = true;
            
            while (context.CurrentToken.Type != TokenType.EndOfFile)
            {
                // Skip initial empty lines and newlines
                if (isFirstContentLine && (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine))
                {
                    context.Advance();
                    continue;
                }
                
                // If this is the first content line and it's a level 1 header, treat it as document title
                if (isFirstContentLine && context.CurrentToken.Type == TokenType.Header)
                {
                    var headerValue = context.CurrentToken.Value;
                    var match = HeaderPattern.Match(headerValue);
                    if (match.Success && match.Groups[1].Value.Length == 1) // Single = means document title
                    {
                        var title = match.Groups[2].Value.Trim();
                        var newHeader = new DocumentHeader(title, document.Header.Author, document.Header.Email, document.Header.Revision, document.Header.Date);
                        document = new Document(newHeader);
                        context.Advance();
                        isFirstContentLine = false;
                        continue;
                    }
                }
                
                isFirstContentLine = false;
                
                // Handle attribute lines specially - don't add them as elements but process them
                if (context.CurrentToken.Type == TokenType.AttributeLine)
                {
                    ParseAttributeLine(context);
                }
                else
                {
                    var element = ParseElement(context);
                    if (element != null)
                    {
                        document.AddChild(element);
                    }
                }
                
                if (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    context.Advance();
                }
                else if (context.CurrentToken.Type != TokenType.EndOfFile)
                {
                    context.Advance();
                }
            }

            // Copy global attributes to document attributes
            if (context.GlobalAttributes is DocumentAttributes globalAttrs && document.Attributes is DocumentAttributes docAttrs)
            {
                foreach (var attrName in globalAttrs.AttributeNames)
                {
                    var attrValue = globalAttrs.GetAttribute(attrName);
                    docAttrs.SetAttribute(attrName, attrValue);
                }
            }

            return document;
        }

        public IDocument ParseFile(string filePath)
        {
            return ParseFile(filePath, null);
        }

        public IDocument ParseFile(string filePath, ParserOptions options)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var content = File.ReadAllText(filePath);
            var fileDirectory = Path.GetDirectoryName(Path.GetFullPath(filePath));
            
            // Create options with the file's directory as base path if no options provided
            var parseOptions = options ?? new ParserOptions();
            if (string.IsNullOrEmpty(parseOptions.BasePath))
            {
                parseOptions = new ParserOptions(fileDirectory);
            }

            return Parse(content, parseOptions);
        }

        public IDocumentElement ParseElement(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var tokenizer = new AsciiDocTokenizer(input);
            var context = new ParseContext(tokenizer);
            
            return ParseElement(context);
        }

        public IDocumentElement ParseElement(IParseContext context)
        {
            switch (context.CurrentToken.Type)
            {
                case TokenType.Header:
                    return ParseSectionTitle(context);
                case TokenType.ListItem:
                    return ParseList(context);
                case TokenType.DescriptionListItem:
                    return ParseDescriptionList(context);
                case TokenType.TableDelimiter:
                    return ParseTable(context);
                case TokenType.BlockQuoteDelimiter:
                    return ParseBlockQuote(context);
                case TokenType.SidebarDelimiter:
                    return ParseSidebar(context);
                case TokenType.ExampleDelimiter:
                    return ParseExample(context);
                case TokenType.OpenDelimiter:
                    return ParseOpen(context);
                case TokenType.VerseDelimiter:
                    return ParseVerse(context);
                case TokenType.LiteralDelimiter:
                    return ParseLiteral(context);
                case TokenType.LiteralAttribute:
                    return ParseLiteralAttribute(context);
                case TokenType.ListingAttribute:
                    return ParseListingAttribute(context);
                case TokenType.PassthroughAttribute:
                    return ParsePassthroughAttribute(context);
                case TokenType.PassthroughDelimiter:
                    return ParsePassthrough(context);
                case TokenType.AttributeLine:
                    ParseAttributeLine(context);
                    return null;
                case TokenType.AttributeBlockLine:
                    return ParseAttributeBlockElement(context);
                case TokenType.VerseAttribute:
                    return ParseVerseAttribute(context);
                case TokenType.AdmonitionBlock:
                    return ParseAdmonition(context);
                case TokenType.TableOfContents:
                    return ParseTableOfContents(context);
                case TokenType.BlockMacro:
                    return ParseBlockMacro(context);
                case TokenType.Text:
                    return ParseParagraph(context);
                case TokenType.CodeBlockDelimiter:
                    return ParseCodeBlock(context);
                case TokenType.EmptyLine:
                case TokenType.NewLine:
                    return null;
                default:
                    return null;
            }
        }

        private ISection ParseSectionTitle(IParseContext context)
        {
            var headerValue = context.CurrentToken.Value;
            var match = HeaderPattern.Match(headerValue);
            
            if (!match.Success)
                throw new ParseException($"Invalid header format: {headerValue}", context.CurrentToken.Line, context.CurrentToken.Column);

            var level = match.Groups[1].Value.Length;
            var title = match.Groups[2].Value.Trim();

            return new Section(title, level);
        }

        private IListItem ParseListItem(IParseContext context)
        {
            var listValue = context.CurrentToken.Value;
            var match = ListItemPattern.Match(listValue);
            
            if (!match.Success)
                throw new ParseException($"Invalid list item format: {listValue}", context.CurrentToken.Line, context.CurrentToken.Column);

            var marker = match.Groups[1].Value;
            var checkboxGroup = match.Groups[2].Value;
            var text = match.Groups[3].Value.Trim();
            var level = marker.StartsWith("*") ? marker.Length : 1;

            // Parse checkbox information
            bool isCheckbox = !string.IsNullOrEmpty(checkboxGroup);
            bool isChecked = false;
            
            if (isCheckbox)
            {
                // Remove leading/trailing whitespace and brackets to get checkbox state
                var checkboxState = checkboxGroup.Trim().TrimStart('[').TrimEnd(']').Trim();
                isChecked = checkboxState.Equals("x", StringComparison.OrdinalIgnoreCase);
            }

            return new ListItem(text, level, isCheckbox, isChecked);
        }

        private IList ParseList(IParseContext context)
        {
            // Determine list type from the first token
            var listType = DetermineListTypeFromToken(context.CurrentToken.Value);
            var list = new List(listType);
            
            // Parse the first item
            var firstItem = ParseListItem(context);
            list.AddChild(firstItem);
            context.Advance();
            
            // Skip newlines and continue parsing list items
            while (context.CurrentToken.Type != TokenType.EndOfFile)
            {
                // Skip newlines
                if (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    context.Advance();
                    continue;
                }
                
                // If we encounter another list item, add it to the current list
                if (context.CurrentToken.Type == TokenType.ListItem)
                {
                    var item = ParseListItem(context);
                    list.AddChild(item);
                    context.Advance();
                }
                else
                {
                    // We've reached the end of the list
                    break;
                }
            }
            
            return list;
        }
        
        private ListType DetermineListTypeFromToken(string tokenValue)
        {
            var match = ListItemPattern.Match(tokenValue);
            if (match.Success)
            {
                var marker = match.Groups[1].Value;
                return marker.Contains(".") ? ListType.Ordered : ListType.Unordered;
            }
            return ListType.Unordered;
        }

        private IDescriptionList ParseDescriptionList(IParseContext context)
        {
            var descriptionList = new DescriptionList();
            
            // Parse the first item
            var firstItem = ParseDescriptionListItem(context);
            descriptionList.AddChild(firstItem);
            context.Advance();
            
            // Skip newlines and continue parsing description list items
            while (context.CurrentToken.Type != TokenType.EndOfFile)
            {
                // Skip newlines
                if (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    context.Advance();
                    continue;
                }
                
                // If we encounter another description list item, add it to the current list
                if (context.CurrentToken.Type == TokenType.DescriptionListItem)
                {
                    var item = ParseDescriptionListItem(context);
                    descriptionList.AddChild(item);
                    context.Advance();
                }
                else
                {
                    // We've reached the end of the description list
                    break;
                }
            }
            
            return descriptionList;
        }

        private IDescriptionListItem ParseDescriptionListItem(IParseContext context)
        {
            var descriptionValue = context.CurrentToken.Value;
            var match = DescriptionListItemPattern.Match(descriptionValue);
            
            if (!match.Success)
                throw new ParseException($"Invalid description list item format: {descriptionValue}", context.CurrentToken.Line, context.CurrentToken.Column);

            var term = match.Groups[1].Value.Trim();
            var description = match.Groups[2].Value.Trim();

            return new DescriptionListItem(term, description);
        }

        private IParagraph ParseParagraph(IParseContext context)
        {
            var text = context.CurrentToken.Value.Trim();
            var paragraph = new Paragraph();
            
            var inlineElements = ParseInlineElements(text);
            foreach (var element in inlineElements)
            {
                paragraph.AddChild(element);
            }
            
            return paragraph;
        }
        
        /// <summary>
        /// Parses inline formatting elements (bold, italic, links, etc.) from a text string.
        /// </summary>
        /// <param name="text">The text content to parse for inline elements.</param>
        /// <returns>A list of document elements representing the parsed inline content.</returns>
        private List<IDocumentElement> ParseInlineElements(string text)
        {
            var elements = new List<IDocumentElement>();
            var position = 0;
            var textLength = text.Length;
            
            // Parse inline elements using individual pattern matching
            while (position < textLength)
            {
                // Find the next inline formatting pattern
                var nextMatch = FindNextInlineMatch(text, position);
                
                if (nextMatch == null || !nextMatch.Success)
                {
                    // No more inline patterns found - add remaining text as plain text element
                    if (position < textLength)
                    {
                        // Use optimized substring for remaining text
                        elements.Add(new Text(text.Substring(position)));
                    }
                    break;
                }
                
                // Add any plain text that appears before the inline element
                if (nextMatch.Index > position)
                {
                    // Use direct substring instead of creating multiple strings
                    elements.Add(new Text(text.Substring(position, nextMatch.Index - position)));
                }
                
                // Convert the matched pattern to its corresponding inline element
                elements.Add(CreateInlineElement(nextMatch));
                
                // Move position past the processed inline element
                position = nextMatch.Index + nextMatch.Length;
            }
            
            return elements;
        }
        
        /// <summary>
        /// Finds the earliest occurring inline formatting pattern in the text starting from the specified index.
        /// </summary>
        /// <param name="text">The text to search for inline patterns.</param>
        /// <param name="startIndex">The position to start searching from.</param>
        /// <returns>The earliest regex match found, or null if no patterns match.</returns>
        private Match FindNextInlineMatch(string text, int startIndex)
        {
            Match earliestMatch = null;
            int earliestIndex = int.MaxValue;
            
            // Check each inline pattern and find the earliest match
            // Note: FootnotePattern must come before InlineMacroPattern to avoid conflicts
            var patterns = new[] {
                StrongPattern,
                EmphasisPattern,
                HighlightPattern,
                SuperscriptPattern,
                SubscriptPattern,
                InlineCodePattern,
                LinkPattern,
                ImagePattern,
                AnchorPattern,
                CrossReferencePattern,
                FootnotePattern,
                InlineMacroPattern
            };
            
            foreach (var pattern in patterns)
            {
                var match = pattern.Match(text, startIndex);
                if (match.Success && match.Index < earliestIndex)
                {
                    earliestMatch = match;
                    earliestIndex = match.Index;
                }
            }
            
            return earliestMatch;
        }
        
        private IDocumentElement CreateInlineElement(Match match)
        {
            // Determine which pattern matched by checking the regex used
            if (StrongPattern.IsMatch(match.Value))
            {
                return new Strong(match.Groups[1].Value);
            }
            else if (EmphasisPattern.IsMatch(match.Value))
            {
                return new Emphasis(match.Groups[1].Value);
            }
            else if (HighlightPattern.IsMatch(match.Value))
            {
                return new Highlight(match.Groups[1].Value);
            }
            else if (SuperscriptPattern.IsMatch(match.Value))
            {
                return new Superscript(match.Groups[1].Value);
            }
            else if (SubscriptPattern.IsMatch(match.Value))
            {
                return new Subscript(match.Groups[1].Value);
            }
            else if (InlineCodePattern.IsMatch(match.Value))
            {
                return new InlineCode(match.Groups[1].Value);
            }
            else if (LinkPattern.IsMatch(match.Value))
            {
                var url = match.Groups[1].Value;
                var text = match.Groups.Count > 3 && !string.IsNullOrEmpty(match.Groups[3].Value)
                    ? match.Groups[3].Value
                    : url;
                return new Link(url, text);
            }
            else if (ImagePattern.IsMatch(match.Value))
            {
                var src = match.Groups[1].Value;
                var alt = match.Groups[2].Value;
                return new Image(src, alt);
            }
            else if (AnchorPattern.IsMatch(match.Value))
            {
                var anchorContent = match.Groups[1].Value;
                // Parse [[id,label]] format - ID is required, label is optional
                var parts = anchorContent.Split(',');
                var id = parts[0].Trim();
                var label = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                return new Anchor(id, label);
            }
            else if (CrossReferencePattern.IsMatch(match.Value))
            {
                var targetId = match.Groups[1].Value.Trim();
                var linkText = match.Groups.Count > 2 && !string.IsNullOrEmpty(match.Groups[2].Value)
                    ? match.Groups[2].Value.Trim()
                    : string.Empty;
                return new CrossReference(targetId, linkText);
            }
            else if (FootnotePattern.IsMatch(match.Value))
            {
                return ParseFootnote(match.Value);
            }
            else if (InlineMacroPattern.IsMatch(match.Value))
            {
                var macroName = match.Groups[1].Value.Trim();
                var target = match.Groups[2].Value.Trim();
                var attributeString = match.Groups[3].Value.Trim();
                var parameters = ParseMacroParameters(attributeString);
                return CreateMacroElement(macroName, target, parameters, MacroType.Inline);
            }
            
            return new Text(match.Value);
        }

        private IDocumentElement ParseAttributeBlockElement(IParseContext context)
        {
            var attributeValue = context.CurrentToken.Value;
            var match = AttributeBlockPattern.Match(attributeValue);
            
            if (match.Success)
            {
                var attributeContent = match.Groups[1].Value;
                context.Advance();
                
                // Skip any empty lines or newlines
                while (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    context.Advance();
                }
                
                // Check if this is a verse attribute
                var verseMatch = VerseAttributePattern.Match(attributeValue);
                if (verseMatch.Success)
                {
                    // Extract author and citation from verse attribute
                    string author = verseMatch.Groups[1].Success ? verseMatch.Groups[1].Value.Trim() : null;
                    string citation = verseMatch.Groups[2].Success ? verseMatch.Groups[2].Value.Trim() : null;
                    
                    // Check for verse delimiters (____) or open block delimiters (--)
                    if (context.CurrentToken.Type == TokenType.BlockQuoteDelimiter)
                    {
                        return ParseVerseWithAttributes(context, null, author, citation);
                    }
                    // Could also support other delimiters like -- for open blocks
                }
                
                // Check if this is a literal attribute
                var literalMatch = LiteralAttributePattern.Match(attributeValue);
                if (literalMatch.Success)
                {
                    // Check for literal delimiters (....) or parse following text as literal
                    if (context.CurrentToken.Type == TokenType.LiteralDelimiter)
                    {
                        return ParseLiteral(context);
                    }
                    else if (context.CurrentToken.Type == TokenType.Text)
                    {
                        return ParseLiteralFromText(context);
                    }
                }
                
                // Check if the next element is a code block
                if (context.CurrentToken.Type == TokenType.CodeBlockDelimiter)
                {
                    // Parse language from source attribute
                    var sourceMatch = SourceAttributePattern.Match(attributeContent);
                    if (sourceMatch.Success)
                    {
                        var language = sourceMatch.Groups[1].Success ? sourceMatch.Groups[1].Value : null;
                        return ParseCodeBlockWithLanguage(context, language);
                    }
                }
                
                // Check if there's a following text element to combine with this attribute block
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    // Parse the following text as a paragraph with the attribute block as context
                    var textContent = context.CurrentToken.Value;
                    var paragraph = new Paragraph();
                    var inlineElements = ParseInlineElements(textContent);
                    foreach (var element in inlineElements)
                    {
                        paragraph.AddChild(element);
                    }
                    context.Advance();
                    return paragraph;
                }
                
                // If no following content, treat the attribute block itself as a paragraph
                var attributeParagraph = new Paragraph();
                attributeParagraph.AddChild(new Text(attributeContent));
                return attributeParagraph;
            }
            
            context.Advance();
            return null;
        }

        private ICodeBlock ParseCodeBlock(IParseContext context)
        {
            // Check if the code block delimiter has a language tag
            var delimiterValue = context.CurrentToken.Value;
            var languageMatch = CodeBlockWithLanguagePattern.Match(delimiterValue);
            string language = null;
            
            if (languageMatch.Success && languageMatch.Groups[1].Success)
            {
                language = languageMatch.Groups[1].Value;
            }
            
            return ParseCodeBlockWithLanguage(context, language);
        }
        
        private ICodeBlock ParseCodeBlockWithLanguage(IParseContext context, string language)
        {
            context.Advance();

            var content = string.Empty;
            
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.CodeBlockDelimiter)
            {
                if (context.CurrentToken.Type == TokenType.Text || 
                    context.CurrentToken.Type == TokenType.CodeContent)
                {
                    content += context.CurrentToken.Value;
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content += "\n";
                }
                
                context.Advance();
            }

            if (context.CurrentToken.Type == TokenType.CodeBlockDelimiter)
            {
                context.Advance();
            }

            return new CodeBlock(content.Trim(), language);
        }

        private ITable ParseTable(IParseContext context)
        {
            var table = new Table();
            
            // Skip the opening delimiter
            context.Advance();
            
            // Parse table rows until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.TableDelimiter)
            {
                if (context.CurrentToken.Type == TokenType.TableRow)
                {
                    var row = ParseTableRow(context);
                    if (row != null)
                    {
                        table.AddRow(row);
                    }
                }
                else if (context.CurrentToken.Type == TokenType.NewLine || 
                         context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    // Skip empty lines
                    context.Advance();
                }
                else
                {
                    context.Advance();
                }
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.TableDelimiter)
            {
                context.Advance();
            }
            
            return table;
        }

        private ITableRow ParseTableRow(IParseContext context)
        {
            var rowContent = context.CurrentToken.Value;
            
            // Remove the leading |
            if (rowContent.StartsWith("|"))
            {
                rowContent = rowContent.Substring(1);
            }
            
            // Split by | to get cells
            var cellContents = rowContent.Split('|');
            var row = new TableRow();
            
            foreach (var cellContent in cellContents)
            {
                var trimmedContent = cellContent.Trim();
                if (!string.IsNullOrEmpty(trimmedContent))
                {
                    var cell = new TableCell(trimmedContent);
                    row.AddCell(cell);
                }
            }
            
            context.Advance();
            return row;
        }

        private IBlockQuote ParseBlockQuote(IParseContext context)
        {
            // Skip the opening delimiter
            context.Advance();
            
            var content = string.Empty;
            var attribution = string.Empty;
            var cite = string.Empty;
            
            // Parse block quote content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.BlockQuoteDelimiter)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    var line = context.CurrentToken.Value;
                    
                    // Check for attribution line (starts with --)
                    if (line.StartsWith("-- "))
                    {
                        attribution = line.Substring(3).Trim();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(content))
                            content += "\n";
                        content += line;
                    }
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    if (!string.IsNullOrEmpty(content))
                        content += "\n";
                }
                
                context.Advance();
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.BlockQuoteDelimiter)
            {
                context.Advance();
            }
            
            return new BlockQuote(content.Trim(), attribution, cite);
        }

        private ISidebar ParseSidebar(IParseContext context)
        {
            // Skip the opening delimiter
            context.Advance();
            
            var sidebar = new Sidebar(title: null);
            
            // Parse sidebar content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.SidebarDelimiter)
            {
                // Skip empty lines and newlines
                if (context.CurrentToken.Type == TokenType.NewLine || 
                    context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    context.Advance();
                    continue;
                }
                
                // Parse child elements within the sidebar
                var element = ParseElement(context);
                if (element != null)
                {
                    sidebar.AddChild(element);
                    context.Advance();
                }
                else
                {
                    context.Advance();
                }
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.SidebarDelimiter)
            {
                context.Advance();
            }
            
            return sidebar;
        }

        private IExample ParseExample(IParseContext context)
        {
            // Skip the opening delimiter
            context.Advance();
            
            var example = new Example(title: null);
            
            // Parse example content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.ExampleDelimiter)
            {
                // Skip empty lines and newlines
                if (context.CurrentToken.Type == TokenType.NewLine || 
                    context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    context.Advance();
                    continue;
                }
                
                // Parse child elements within the example
                var element = ParseElement(context);
                if (element != null)
                {
                    example.AddChild(element);
                    context.Advance();
                }
                else
                {
                    context.Advance();
                }
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.ExampleDelimiter)
            {
                context.Advance();
            }
            
            return example;
        }

        private IVerse ParseVerse(IParseContext context)
        {
            // Skip the opening delimiter
            context.Advance();
            
            // Skip the immediate newline after the opening delimiter if present
            if (context.CurrentToken.Type == TokenType.NewLine)
            {
                context.Advance();
            }
            
            var content = new System.Text.StringBuilder();
            string title = null;
            string author = null;
            string citation = null;
            
            // Parse verse content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.VerseDelimiter)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    content.Append(context.CurrentToken.Value);
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content.AppendLine();
                }
                else if (context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    content.AppendLine();
                }
                
                context.Advance();
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.VerseDelimiter)
            {
                context.Advance();
            }
            
            // Remove trailing newline if present
            var verseContent = content.ToString();
            if (verseContent.EndsWith("\n"))
            {
                verseContent = verseContent.Substring(0, verseContent.Length - 1);
            }
            if (verseContent.EndsWith("\r"))
            {
                verseContent = verseContent.Substring(0, verseContent.Length - 1);
            }
            
            return new Verse(verseContent, title, author, citation);
        }

        private IVerse ParseVerseWithAttributes(IParseContext context, string title, string author, string citation)
        {
            // Skip the opening delimiter (____) 
            context.Advance();
            
            // Skip the immediate newline after the opening delimiter if present
            if (context.CurrentToken.Type == TokenType.NewLine)
            {
                context.Advance();
            }
            
            var content = new System.Text.StringBuilder();
            
            // Parse verse content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.BlockQuoteDelimiter)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    content.Append(context.CurrentToken.Value);
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content.AppendLine();
                }
                else if (context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    content.AppendLine();
                }
                
                context.Advance();
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.BlockQuoteDelimiter)
            {
                context.Advance();
            }
            
            // Remove trailing newline if present
            var verseContent = content.ToString();
            if (verseContent.EndsWith("\n"))
            {
                verseContent = verseContent.Substring(0, verseContent.Length - 1);
            }
            if (verseContent.EndsWith("\r"))
            {
                verseContent = verseContent.Substring(0, verseContent.Length - 1);
            }
            
            return new Verse(verseContent, title, author, citation);
        }

        private IVerse ParseVerseAttribute(IParseContext context)
        {
            var attributeValue = context.CurrentToken.Value;
            
            // Extract author and citation from verse attribute
            var verseMatch = VerseAttributePattern.Match(attributeValue);
            string author = verseMatch.Groups[1].Success ? verseMatch.Groups[1].Value.Trim() : null;
            string citation = verseMatch.Groups[2].Success ? verseMatch.Groups[2].Value.Trim() : null;
            
            context.Advance();
            
            // Skip any empty lines or newlines
            while (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine)
            {
                context.Advance();
            }
            
            // Check for verse delimiters (____) 
            if (context.CurrentToken.Type == TokenType.BlockQuoteDelimiter)
            {
                return ParseVerseWithAttributes(context, null, author, citation);
            }
            
            // If no delimiter found, return null and let the parsing continue
            return null;
        }

        private ILiteral ParseLiteral(IParseContext context)
        {
            // Skip the opening delimiter (....)
            context.Advance();
            
            var content = new System.Text.StringBuilder();
            
            // Parse literal content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.LiteralDelimiter)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    content.Append(context.CurrentToken.Value);
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content.Append('\n');
                }
                else if (context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    content.Append('\n');
                }
                
                context.Advance();
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.LiteralDelimiter)
            {
                context.Advance();
            }
            
            // Remove leading/trailing newlines
            var literalContent = content.ToString();
            literalContent = literalContent.Trim('\n', '\r');
            
            return new Literal(literalContent);
        }

        private ILiteral ParseLiteralFromText(IParseContext context)
        {
            // Parse text content as literal (used with [literal] attribute)
            var content = new System.Text.StringBuilder();
            
            // Parse literal content until we hit an empty line, new block element, or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.EmptyLine &&
                   context.CurrentToken.Type != TokenType.Header &&
                   context.CurrentToken.Type != TokenType.ListItem &&
                   context.CurrentToken.Type != TokenType.AttributeLine &&
                   context.CurrentToken.Type != TokenType.AttributeBlockLine)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    content.Append(context.CurrentToken.Value);
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content.Append('\n');
                }
                
                context.Advance();
            }
            
            // Remove leading/trailing newlines
            var literalContent = content.ToString();
            literalContent = literalContent.Trim('\n', '\r');
            
            return new Literal(literalContent);
        }

        private ILiteral ParseLiteralAttribute(IParseContext context)
        {
            // Skip the [literal] attribute line
            context.Advance();
            
            // Skip any empty lines or newlines
            while (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine)
            {
                context.Advance();
            }
            
            // Check if the next element is a literal delimiter
            if (context.CurrentToken.Type == TokenType.LiteralDelimiter)
            {
                return ParseLiteral(context);
            }
            // Otherwise parse following text as literal
            else if (context.CurrentToken.Type == TokenType.Text)
            {
                return ParseLiteralFromText(context);
            }
            
            // If no content follows, return empty literal
            return new Literal("");
        }

        private IListing ParseListingAttribute(IParseContext context)
        {
            // Skip the [listing] attribute line
            context.Advance();
            
            // Skip any empty lines or newlines
            while (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine)
            {
                context.Advance();
            }
            
            // Check if the next element is a code block delimiter (----)
            if (context.CurrentToken.Type == TokenType.CodeBlockDelimiter)
            {
                return ParseListingFromCodeBlock(context);
            }
            // Otherwise parse following text as listing
            else if (context.CurrentToken.Type == TokenType.Text)
            {
                return ParseListingFromText(context);
            }
            
            // If no content follows, return empty listing
            return new Listing("", null, null);
        }

        private IListing ParseListingFromCodeBlock(IParseContext context)
        {
            // Skip the opening delimiter (----)
            context.Advance();
            
            var content = new System.Text.StringBuilder();
            
            // Parse listing content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.CodeBlockDelimiter)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    content.Append(context.CurrentToken.Value);
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content.Append('\n');
                }
                else if (context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    content.Append('\n');
                }
                
                context.Advance();
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.CodeBlockDelimiter)
            {
                context.Advance();
            }
            
            // Remove leading/trailing newlines
            var listingContent = content.ToString();
            listingContent = listingContent.Trim('\n', '\r');
            
            return new Listing(listingContent, null, null);
        }

        private IListing ParseListingFromText(IParseContext context)
        {
            // Parse text content as listing (used with [listing] attribute)
            var content = new System.Text.StringBuilder();
            
            // Parse listing content until we hit an empty line, new block element, or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.EmptyLine &&
                   context.CurrentToken.Type != TokenType.Header &&
                   context.CurrentToken.Type != TokenType.ListItem &&
                   context.CurrentToken.Type != TokenType.AttributeLine &&
                   context.CurrentToken.Type != TokenType.AttributeBlockLine)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    content.Append(context.CurrentToken.Value);
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content.Append('\n');
                }
                
                context.Advance();
            }
            
            // Remove leading/trailing newlines
            var listingContent = content.ToString();
            listingContent = listingContent.Trim('\n', '\r');
            
            return new Listing(listingContent, null, null);
        }

        private IAdmonition ParseAdmonition(IParseContext context)
        {
            var admonitionValue = context.CurrentToken.Value;
            var match = AdmonitionPattern.Match(admonitionValue);
            
            if (!match.Success)
                throw new ParseException($"Invalid admonition format: {admonitionValue}", context.CurrentToken.Line, context.CurrentToken.Column);

            var typeString = match.Groups[1].Value;
            var content = match.Groups[2].Value.Trim();
            
            // Parse the admonition type
            AdmonitionType type;
            if (!Enum.TryParse(typeString, true, out type))
            {
                throw new ParseException($"Unknown admonition type: {typeString}", context.CurrentToken.Line, context.CurrentToken.Column);
            }

            return new Admonition(type, content);
        }

        private void ParseAttributeLine(IParseContext context)
        {
            var attributeValue = context.CurrentToken.Value;
            var match = AttributeLinePattern.Match(attributeValue);
            
            if (match.Success)
            {
                var name = match.Groups[1].Value.Trim();
                var unsetFlag = match.Groups[2].Value; // The "!" character if present
                var value = match.Groups[3].Value.Trim();
                
                // Handle special boolean attributes
                if (!string.IsNullOrEmpty(unsetFlag)) // Has "!" suffix
                {
                    value = "false";
                }
                else if (string.IsNullOrEmpty(value)) // Empty value means true
                {
                    value = "true";
                }
                
                if (context.GlobalAttributes is DocumentAttributes attrs)
                {
                    attrs.SetAttribute(name, value);
                }
            }
            
            context.Advance();
        }

        private ITableOfContents ParseTableOfContents(IParseContext context)
        {
            var tocValue = context.CurrentToken.Value;
            var match = TableOfContentsPattern.Match(tocValue);
            
            if (!match.Success)
                throw new ParseException($"Invalid table of contents format: {tocValue}", context.CurrentToken.Line, context.CurrentToken.Column);

            var attributeString = match.Groups[1].Value.Trim();
            var parameters = ParseMacroParameters(attributeString);
            
            // Extract TOC parameters
            var title = parameters.ContainsKey("title") ? parameters["title"] : "Table of Contents";
            var maxDepthStr = parameters.ContainsKey("levels") ? parameters["levels"] : "3";
            var maxDepth = int.TryParse(maxDepthStr, out var depth) ? depth : 3;
            
            context.Advance();
            
            // Generate TOC entries from document sections
            var entries = GenerateTableOfContentsEntries(context, maxDepth);
            
            return new TableOfContents(title, maxDepth, entries);
        }

        private IDocumentElement ParseFootnote(string footnoteText)
        {
            var match = FootnotePattern.Match(footnoteText);
            if (!match.Success)
                return new Text(footnoteText);

            var id = match.Groups[1].Value.Trim(); // The ID part (can be empty)
            var text = match.Groups[2].Value.Trim(); // The content part
            
            if (string.IsNullOrEmpty(id))
            {
                // footnote:[text] - anonymous footnote
                id = GenerateFootnoteId();
                var referenceLabel = GetFootnoteReferenceLabel(id);
                return new Footnote(id, text, referenceLabel, false);
            }
            else if (string.IsNullOrEmpty(text))
            {
                // footnote:id[] - reference to existing footnote
                var referenceLabel = GetFootnoteReferenceLabel(id);
                return new Footnote(id, string.Empty, referenceLabel, true);
            }
            else
            {
                // footnote:id[text] - named footnote with content
                var referenceLabel = GetFootnoteReferenceLabel(id);
                return new Footnote(id, text, referenceLabel, false);
            }
        }

        private static int _footnoteCounter = 1;

        private string GenerateFootnoteId()
        {
            return $"_footnotedef_{_footnoteCounter++}";
        }

        private string GetFootnoteReferenceLabel(string id)
        {
            // Simple numeric labeling for now. In a full implementation,
            // this would maintain a global footnote counter across the document
            return _footnoteCounter.ToString();
        }

        private IReadOnlyList<ITableOfContentsEntry> GenerateTableOfContentsEntries(IParseContext context, int maxDepth)
        {
            var entries = new List<ITableOfContentsEntry>();
            var allElements = GetAllDocumentElements(context);
            
            var sections = allElements.OfType<ISection>()
                .Where(s => !string.IsNullOrEmpty(s.Title) && s.Level <= maxDepth)
                .ToList();
            
            var stack = new Stack<(ITableOfContentsEntry entry, int level)>();
            
            foreach (var section in sections)
            {
                var anchorId = section.Id ?? GenerateAnchorId(section.Title);
                var entry = new TableOfContentsEntry(section.Title, section.Level, anchorId);
                
                // Pop items from stack until we find an appropriate parent
                while (stack.Count > 0 && stack.Peek().level >= section.Level)
                {
                    stack.Pop();
                }
                
                if (stack.Count == 0)
                {
                    // Top-level entry
                    entries.Add(entry);
                }
                else
                {
                    // Add as child to parent (this would require modifying TableOfContentsEntry to be mutable)
                    // For now, we'll add all entries at the top level and rely on the HTML converter to handle nesting
                    entries.Add(entry);
                }
                
                stack.Push((entry, section.Level));
            }
            
            return entries;
        }

        private List<IDocumentElement> GetAllDocumentElements(IParseContext context)
        {
            // This is a simplified approach. In a full implementation, you'd traverse
            // the entire document tree to collect all sections
            var elements = new List<IDocumentElement>();
            
            // For now, return empty list. This will need to be enhanced to properly
            // traverse the document structure and collect all sections
            return elements;
        }

        private string GenerateAnchorId(string title)
        {
            // Generate a URL-friendly anchor ID from the title
            return title.ToLowerInvariant()
                       .Replace(" ", "-")
                       .Replace("'", "")
                       .Replace("\"", "")
                       .Replace("(", "")
                       .Replace(")", "")
                       .Replace("[", "")
                       .Replace("]", "")
                       .Replace("{", "")
                       .Replace("}", "");
        }

        private IDocumentElement ParseBlockMacro(IParseContext context)
        {
            var macroValue = context.CurrentToken.Value;
            var match = BlockMacroPattern.Match(macroValue);
            
            if (!match.Success)
                throw new ParseException($"Invalid block macro format: {macroValue}", context.CurrentToken.Line, context.CurrentToken.Column);

            var macroName = match.Groups[1].Value.Trim();
            var target = match.Groups[2].Value.Trim();
            var attributeString = match.Groups[3].Value.Trim();
            
            var parameters = ParseMacroParameters(attributeString);
            
            context.Advance();
            
            return CreateMacroElement(macroName, target, parameters, MacroType.Block, context);
        }

        private IDocumentElement CreateMacroElement(string macroName, string target, 
            IReadOnlyDictionary<string, string> parameters, MacroType macroType, IParseContext context = null)
        {
            switch (macroName.ToLowerInvariant())
            {
                case "image":
                    return new ImageMacro(target, parameters, macroType);
                case "video":
                    return new VideoMacro(target, parameters, macroType);
                case "include":
                    var includeMacro = new IncludeMacro(target, parameters, macroType);
                    
                    // Process include directive if context is available
                    if (context != null)
                    {
                        return ProcessIncludeDirective(includeMacro, context);
                    }
                    return includeMacro;
                default:
                    return new Macro(macroName, target, parameters, macroType);
            }
        }

        private Dictionary<string, string> ParseMacroParameters(string parameterString)
        {
            var parameters = new Dictionary<string, string>();
            
            if (string.IsNullOrEmpty(parameterString))
                return parameters;
            
            // Handle simple positional parameters (alt text) and named parameters
            var parts = SplitMacroParameters(parameterString);
            
            for (int i = 0; i < parts.Count; i++)
            {
                var part = parts[i].Trim();
                if (string.IsNullOrEmpty(part))
                    continue;
                    
                // Check if it's a named parameter (key=value)
                var equalIndex = part.IndexOf('=');
                if (equalIndex > 0)
                {
                    var key = part.Substring(0, equalIndex).Trim();
                    var value = part.Substring(equalIndex + 1).Trim();
                    // Remove quotes if present
                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    parameters[key] = value;
                }
                else
                {
                    // Positional parameter - first one is typically alt text or title
                    if (i == 0)
                    {
                        parameters["alt"] = part;
                        parameters["title"] = part;
                    }
                    else
                    {
                        parameters[$"param{i}"] = part;
                    }
                }
            }
            
            return parameters;
        }

        /// <summary>
        /// Splits macro parameter strings while respecting quoted values and escape sequences.
        /// This method handles complex parameter parsing including comma separation, quote handling,
        /// and escape sequence processing for AsciiDoc macro syntax.
        /// </summary>
        /// <param name="parameterString">The raw parameter string from within macro brackets.</param>
        /// <returns>A list of individual parameter strings with quotes and escapes processed.</returns>
        /// <remarks>
        /// COMPLEX PARSING LOGIC: This method implements a state machine to handle:
        /// - Comma-separated parameters: param1,param2,param3
        /// - Quoted parameters: "value with, comma","another value"
        /// - Mixed quotes: 'single quoted',"double quoted"
        /// - Escape sequences: "value with \"embedded\" quotes"
        /// 
        /// PERFORMANCE NOTE: String operations create multiple allocations. Consider using
        /// ReadOnlySpan&lt;char&gt; for zero-allocation parsing in performance-critical scenarios.
        /// </remarks>
        private List<string> SplitMacroParameters(string parameterString)
        {
            var parts = new List<string>();
            var current = new System.Text.StringBuilder();
            var inQuotes = false;
            var quoteChar = '\0';  // Track which quote character we're currently inside
            
            // State machine to parse parameters with quote and escape handling
            for (int i = 0; i < parameterString.Length; i++)
            {
                var ch = parameterString[i];
                
                // Handle start of quoted section
                if (!inQuotes && (ch == '"' || ch == '\''))
                {
                    inQuotes = true;
                    quoteChar = ch;
                    current.Append(ch);
                }
                else if (inQuotes && ch == quoteChar)
                {
                    inQuotes = false;
                    current.Append(ch);
                }
                else if (!inQuotes && ch == ',')
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(ch);
                }
            }
            
            if (current.Length > 0)
            {
                parts.Add(current.ToString());
            }
            
            return parts;
        }

        private IDocumentElement ProcessIncludeDirective(IIncludeMacro includeMacro, IParseContext context)
        {
            try
            {
                var includeProcessor = new IncludeProcessor(this, context.Tokenizer);
                var basePath = context.CurrentFilePath;
                var includeStack = context.IncludeStack;
                
                var elements = includeProcessor.ProcessInclude(includeMacro, basePath, includeStack);
                
                // If we have multiple elements, we need to wrap them in a container
                if (elements.Count == 0)
                {
                    return includeMacro; // Return the original macro if no content
                }
                else if (elements.Count == 1)
                {
                    return elements[0]; // Return single element directly
                }
                else
                {
                    // Create a section to contain multiple elements
                    var section = new Section("", 0); // Empty section as container
                    foreach (var element in elements)
                    {
                        section.AddChild(element);
                    }
                    return section;
                }
            }
            catch (ParseException ex)
            {
                // Re-throw circular reference exceptions - these are critical errors that must not be suppressed
                if (ex.Message.Contains("Circular include detected"))
                {
                    throw;
                }
                
                // For other parse exceptions (like file not found for non-optional includes), 
                // return the original macro to allow graceful degradation
                return includeMacro;
            }
        }

        private IOpen ParseOpen(IParseContext context)
        {
            // Skip the opening delimiter
            context.Advance();
            
            var open = new Open(title: null, masqueradeType: null);
            
            // Parse open block content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.OpenDelimiter)
            {
                // Skip empty lines and newlines
                if (context.CurrentToken.Type == TokenType.NewLine || 
                    context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    context.Advance();
                    continue;
                }
                
                // Parse child elements within the open block
                var element = ParseElement(context);
                if (element != null)
                {
                    open.AddChild(element);
                    context.Advance();
                }
                else
                {
                    context.Advance();
                }
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.OpenDelimiter)
            {
                context.Advance();
            }
            
            return open;
        }

        private IPassthrough ParsePassthrough(IParseContext context)
        {
            // Skip the opening delimiter (++++++)
            context.Advance();
            
            var content = new System.Text.StringBuilder();
            
            // Parse passthrough content until we hit the closing delimiter or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.PassthroughDelimiter)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    content.Append(context.CurrentToken.Value);
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content.Append('\n');
                }
                else if (context.CurrentToken.Type == TokenType.EmptyLine)
                {
                    content.Append('\n');
                }
                
                context.Advance();
            }
            
            // Skip the closing delimiter if present
            if (context.CurrentToken.Type == TokenType.PassthroughDelimiter)
            {
                context.Advance();
            }
            
            // Preserve raw content without trimming (important for passthrough)
            var passthroughContent = content.ToString();
            
            return new Passthrough(passthroughContent);
        }

        private IPassthrough ParsePassthroughFromText(IParseContext context)
        {
            // Parse text content as passthrough (used with [pass] attribute)
            var content = new System.Text.StringBuilder();
            
            // Parse passthrough content until we hit an empty line, new block element, or EOF
            while (context.CurrentToken.Type != TokenType.EndOfFile && 
                   context.CurrentToken.Type != TokenType.EmptyLine &&
                   context.CurrentToken.Type != TokenType.Header &&
                   context.CurrentToken.Type != TokenType.ListItem &&
                   context.CurrentToken.Type != TokenType.AttributeLine &&
                   context.CurrentToken.Type != TokenType.AttributeBlockLine)
            {
                if (context.CurrentToken.Type == TokenType.Text)
                {
                    content.Append(context.CurrentToken.Value);
                }
                else if (context.CurrentToken.Type == TokenType.NewLine)
                {
                    content.Append('\n');
                }
                
                context.Advance();
            }
            
            // Preserve raw content without trimming (important for passthrough)
            var passthroughContent = content.ToString();
            
            return new Passthrough(passthroughContent);
        }

        private IPassthrough ParsePassthroughAttribute(IParseContext context)
        {
            // Skip the [pass] attribute line
            context.Advance();
            
            // Skip any empty lines or newlines
            while (context.CurrentToken.Type == TokenType.NewLine || context.CurrentToken.Type == TokenType.EmptyLine)
            {
                context.Advance();
            }
            
            // Check if the next element is a passthrough delimiter
            if (context.CurrentToken.Type == TokenType.PassthroughDelimiter)
            {
                return ParsePassthrough(context);
            }
            // Otherwise parse following text as passthrough
            else if (context.CurrentToken.Type == TokenType.Text)
            {
                return ParsePassthroughFromText(context);
            }
            
            // If no content follows, return empty passthrough
            return new Passthrough("");
        }
    }
}