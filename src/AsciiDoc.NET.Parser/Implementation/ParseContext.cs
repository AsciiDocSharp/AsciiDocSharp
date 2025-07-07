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
using System.Linq;
using AsciiDoc.Net.Core;
using AsciiDoc.Net.Core.Implementation;

namespace AsciiDoc.Net.Parser.Implementation
{
    public class ParseContext : IParseContext
    {
        public ParseContext(ITokenizer tokenizer, string filePath = null, IReadOnlyList<string> includeStack = null)
        {
            Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            GlobalAttributes = new DocumentAttributes();
            ElementStack = new Stack<IDocumentElement>();
            CurrentFilePath = filePath ?? string.Empty;
            IncludeStack = includeStack ?? new List<string>();
            Advance();
        }

        public ITokenizer Tokenizer { get; }
        public IToken CurrentToken { get; private set; }
        public IDocumentAttributes GlobalAttributes { get; }
        public Stack<IDocumentElement> ElementStack { get; }
        public string CurrentFilePath { get; }
        public IReadOnlyList<string> IncludeStack { get; }

        public void Advance()
        {
            CurrentToken = Tokenizer.NextToken();
        }

        public bool Accept(TokenType tokenType)
        {
            if (CurrentToken.Type == tokenType)
            {
                Advance();
                return true;
            }
            return false;
        }

        public bool Expect(TokenType tokenType)
        {
            if (CurrentToken.Type == tokenType)
            {
                Advance();
                return true;
            }
            throw new ParseException($"Expected {tokenType} but found {CurrentToken.Type} at line {CurrentToken.Line}, column {CurrentToken.Column}");
        }

        public void PushElement(IDocumentElement element)
        {
            ElementStack.Push(element);
        }

        public IDocumentElement PopElement()
        {
            return ElementStack.Count > 0 ? ElementStack.Pop() : null;
        }

        public IDocumentElement PeekElement()
        {
            return ElementStack.Count > 0 ? ElementStack.Peek() : null;
        }
    }
}