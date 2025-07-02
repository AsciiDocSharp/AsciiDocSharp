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
using AsciiDoc.NET.Core.Elements;

namespace AsciiDoc.NET.Core.Implementation
{
    /// <summary>
    /// Represents superscript text formatting implementation.
    /// Superscript text is rendered smaller and raised above the baseline.
    /// </summary>
    public class Superscript : DocumentElementBase, ISuperscript
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Superscript"/> class.
        /// </summary>
        /// <param name="text">The text content to be rendered as superscript.</param>
        /// <param name="attributes">Optional document attributes for this superscript element.</param>
        /// <exception cref="ArgumentNullException">Thrown when text is null.</exception>
        public Superscript(string text, IDocumentAttributes attributes = null) 
            : base("superscript", attributes ?? new DocumentAttributes())
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Gets the text content that should be rendered as superscript.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Accepts a visitor for processing this superscript element.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}