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

namespace AsciiDoc.Net.Core.Implementation
{
    /// <summary>
    /// Implementation of highlighted text element.
    /// </summary>
    public class Highlight : DocumentElementBase, IHighlight
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Highlight"/> class.
        /// </summary>
        /// <param name="text">The text content to be highlighted. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when text is null.</exception>
        public Highlight(string text) : base("highlight")
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <inheritdoc />
        public string Text { get; }

        /// <inheritdoc />
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}