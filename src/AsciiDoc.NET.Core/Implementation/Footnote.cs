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

using AsciiDoc.Net.Core.Elements;

namespace AsciiDoc.Net.Core.Implementation
{
    /// <summary>
    /// Implementation of a footnote element that provides supplementary information linked from the main text.
    /// </summary>
    public class Footnote : DocumentElementBase, IFootnote
    {
        /// <summary>
        /// Initializes a new instance of the Footnote class.
        /// </summary>
        /// <param name="id">The unique identifier for this footnote.</param>
        /// <param name="text">The text content of the footnote.</param>
        /// <param name="referenceLabel">The reference label for this footnote.</param>
        /// <param name="isReference">Whether this is a reference-only footnote.</param>
        public Footnote(string id, string text, string referenceLabel, bool isReference = false)
            : base("footnote")
        {
            Id = id ?? string.Empty;
            Text = text ?? string.Empty;
            ReferenceLabel = referenceLabel ?? string.Empty;
            IsReference = isReference;
        }

        /// <summary>
        /// Gets the unique identifier for this footnote.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the text content of the footnote.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the reference number or label for this footnote.
        /// </summary>
        public string ReferenceLabel { get; }

        /// <summary>
        /// Gets a value indicating whether this is a footnote reference only.
        /// </summary>
        public bool IsReference { get; }

        /// <summary>
        /// Accepts a visitor for processing this footnote element.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}