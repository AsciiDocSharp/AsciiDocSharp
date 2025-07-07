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

using AsciiDoc.Net.Core;
using System;
using System.Collections.Generic;

namespace AsciiDoc.Net.Converters.Core
{
    public class ConverterContext : IConverterContext
    {
        public IDocument Document { get; }
        public IConverterOptions Options { get; }
        public IDocumentElement CurrentElement => ElementStack.Count > 0 ? ElementStack.Peek() : null;
        public Stack<IDocumentElement> ElementStack { get; } = new Stack<IDocumentElement>();

        public ConverterContext(IDocument document, IConverterOptions options)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void PushElement(IDocumentElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            
            ElementStack.Push(element);
        }

        public IDocumentElement PopElement()
        {
            if (ElementStack.Count == 0)
                throw new InvalidOperationException("Cannot pop from empty element stack");
            
            return ElementStack.Pop();
        }
    }
}