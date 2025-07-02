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
using System.Text.RegularExpressions;

namespace AsciiDoc.NET.Parser.Implementation
{
    public class AsciiDocTokenizer : ITokenizer
    {
        private string _input;
        private int _position;
        private int _line;
        private int _column;

        private static readonly Regex HeaderPattern = new Regex(@"^(=+)\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex ListItemPattern = new Regex(@"^(\*+|\d+\.)\s+(\[[ xX]\]\s+)?(.+)$", RegexOptions.Compiled);
        private static readonly Regex DescriptionListItemPattern = new Regex(@"^([^:\[\]]+)::\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex EmphasisPattern = new Regex(@"_([^_]+)_", RegexOptions.Compiled);
        private static readonly Regex StrongPattern = new Regex(@"\*([^*]+)\*", RegexOptions.Compiled);
        private static readonly Regex InlineCodePattern = new Regex(@"`([^`]+)`", RegexOptions.Compiled);
        private static readonly Regex LinkPattern = new Regex(@"(https?://[^\s\[\]]+)(\[([^\]]*)\])?", RegexOptions.Compiled);
        private static readonly Regex ImagePattern = new Regex(@"image::([^\[]+)\[([^\]]*)\]", RegexOptions.Compiled);
        private static readonly Regex TableDelimiterPattern = new Regex(@"^\|===+$", RegexOptions.Compiled);
        private static readonly Regex TableRowPattern = new Regex(@"^\|(.*)$", RegexOptions.Compiled);
        private static readonly Regex BlockQuoteDelimiterPattern = new Regex(@"^_{4,}$", RegexOptions.Compiled);
        private static readonly Regex SidebarDelimiterPattern = new Regex(@"^\*{4,}$", RegexOptions.Compiled);
        private static readonly Regex ExampleDelimiterPattern = new Regex(@"^={4,}$", RegexOptions.Compiled);
        private static readonly Regex AttributeLinePattern = new Regex(@"^:([^:!]+)(!?):\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex AttributeBlockPattern = new Regex(@"^\[([^\]]+)\]$", RegexOptions.Compiled);
        private static readonly Regex CodeBlockWithLanguagePattern = new Regex(@"^----(\w+)?$", RegexOptions.Compiled);
        private static readonly Regex AdmonitionPattern = new Regex(@"^(NOTE|TIP|IMPORTANT|WARNING|CAUTION):\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex AnchorPattern = new Regex(@"\[\[([^\]]+)\]\]", RegexOptions.Compiled);
        private static readonly Regex CrossReferencePattern = new Regex(@"<<([^>]+)>>", RegexOptions.Compiled);
        private static readonly Regex BlockMacroPattern = new Regex(@"^(\w+)::([^\[]*)\[([^\]]*)\]$", RegexOptions.Compiled);
        private static readonly Regex InlineMacroPattern = new Regex(@"(\w+):([^\[]*)\[([^\]]*)\]", RegexOptions.Compiled);
        private static readonly Regex TableOfContentsPattern = new Regex(@"^toc::\s*\[([^\]]*)\]$", RegexOptions.Compiled);
        private static readonly Regex FootnotePattern = new Regex(@"footnote:([^:\[\]]*?)\[([^\]]*)\]", RegexOptions.Compiled);

        public AsciiDocTokenizer()
        {
        }

        public AsciiDocTokenizer(string input)
        {
            Reset(input);
        }

        public bool HasMoreTokens => _position < _input?.Length;

        public void Reset(string input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _position = 0;
            _line = 1;
            _column = 1;
        }

        public IEnumerable<IToken> Tokenize(string input)
        {
            Reset(input);
            
            while (HasMoreTokens)
            {
                yield return NextToken();
            }
            
            yield return CreateToken(TokenType.EndOfFile, string.Empty);
        }

        public IToken NextToken()
        {
            if (!HasMoreTokens)
            {
                return CreateToken(TokenType.EndOfFile, string.Empty);
            }

            SkipWhitespace();

            if (!HasMoreTokens)
            {
                return CreateToken(TokenType.EndOfFile, string.Empty);
            }

            var currentChar = _input[_position];

            if (currentChar == '\n')
            {
                return ReadNewLine();
            }

            if (IsAtStartOfLine())
            {
                var lineContent = ReadLine().Trim();
                
                if (string.IsNullOrWhiteSpace(lineContent))
                {
                    return CreateToken(TokenType.EmptyLine, lineContent);
                }

                var headerMatch = HeaderPattern.Match(lineContent);
                if (headerMatch.Success)
                {
                    return CreateToken(TokenType.Header, lineContent);
                }

                var listMatch = ListItemPattern.Match(lineContent);
                if (listMatch.Success)
                {
                    return CreateToken(TokenType.ListItem, lineContent);
                }


                var tableDelimiterMatch = TableDelimiterPattern.Match(lineContent);
                if (tableDelimiterMatch.Success)
                {
                    return CreateToken(TokenType.TableDelimiter, lineContent);
                }

                var tableRowMatch = TableRowPattern.Match(lineContent);
                if (tableRowMatch.Success)
                {
                    return CreateToken(TokenType.TableRow, lineContent);
                }

                var blockQuoteDelimiterMatch = BlockQuoteDelimiterPattern.Match(lineContent);
                if (blockQuoteDelimiterMatch.Success)
                {
                    return CreateToken(TokenType.BlockQuoteDelimiter, lineContent);
                }

                var sidebarDelimiterMatch = SidebarDelimiterPattern.Match(lineContent);
                if (sidebarDelimiterMatch.Success)
                {
                    return CreateToken(TokenType.SidebarDelimiter, lineContent);
                }

                var exampleDelimiterMatch = ExampleDelimiterPattern.Match(lineContent);
                if (exampleDelimiterMatch.Success)
                {
                    return CreateToken(TokenType.ExampleDelimiter, lineContent);
                }

                var attributeLineMatch = AttributeLinePattern.Match(lineContent);
                if (attributeLineMatch.Success)
                {
                    return CreateToken(TokenType.AttributeLine, lineContent);
                }

                var attributeBlockMatch = AttributeBlockPattern.Match(lineContent);
                if (attributeBlockMatch.Success)
                {
                    return CreateToken(TokenType.AttributeBlockLine, lineContent);
                }

                var codeBlockWithLanguageMatch = CodeBlockWithLanguagePattern.Match(lineContent);
                if (codeBlockWithLanguageMatch.Success)
                {
                    return CreateToken(TokenType.CodeBlockDelimiter, lineContent);
                }

                var admonitionMatch = AdmonitionPattern.Match(lineContent);
                if (admonitionMatch.Success)
                {
                    return CreateToken(TokenType.AdmonitionBlock, lineContent);
                }

                var tocMatch = TableOfContentsPattern.Match(lineContent);
                if (tocMatch.Success)
                {
                    return CreateToken(TokenType.TableOfContents, lineContent);
                }

                var blockMacroMatch = BlockMacroPattern.Match(lineContent);
                if (blockMacroMatch.Success)
                {
                    return CreateToken(TokenType.BlockMacro, lineContent);
                }

                var descriptionListMatch = DescriptionListItemPattern.Match(lineContent);
                if (descriptionListMatch.Success)
                {
                    return CreateToken(TokenType.DescriptionListItem, lineContent);
                }

                return CreateToken(TokenType.Text, lineContent);
            }

            return ReadText();
        }

        private bool IsAtStartOfLine()
        {
            return _column == 1 || (_position > 0 && _input[_position - 1] == '\n');
        }

        private IToken ReadNewLine()
        {
            var token = CreateToken(TokenType.NewLine, "\n");
            _position++;
            _line++;
            _column = 1;
            return token;
        }

        private IToken ReadText()
        {
            var start = _position;
            var startColumn = _column;

            while (HasMoreTokens && _input[_position] != '\n')
            {
                _position++;
                _column++;
            }

            var value = _input.Substring(start, _position - start);
            return new Token(TokenType.Text, value, _line, startColumn, start);
        }

        private string ReadLine()
        {
            var start = _position;

            while (HasMoreTokens && _input[_position] != '\n')
            {
                _position++;
                _column++;
            }

            return _input.Substring(start, _position - start);
        }

        private void SkipWhitespace()
        {
            while (HasMoreTokens && char.IsWhiteSpace(_input[_position]) && _input[_position] != '\n')
            {
                _position++;
                _column++;
            }
        }

        private IToken CreateToken(TokenType type, string value)
        {
            return new Token(type, value, _line, _column, _position);
        }
    }
}