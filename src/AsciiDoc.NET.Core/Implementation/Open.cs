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

using System.Collections.Generic;
using System.Linq;
using AsciiDoc.NET.Core.Elements;

namespace AsciiDoc.NET.Core.Implementation
{
    /// <summary>
    /// Represents an open block implementation.
    /// Open blocks are versatile containers that can enclose content without adding specific semantic meaning,
    /// or can masquerade as other block types when combined with attributes.
    /// </summary>
    public class Open : DocumentElementBase, IOpen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Open"/> class.
        /// </summary>
        /// <param name="children">The child elements contained within the open block.</param>
        /// <param name="title">The optional title of the open block.</param>
        /// <param name="masqueradeType">The type this open block is masquerading as (e.g., "sidebar", "source").</param>
        /// <param name="attributes">The document attributes for this open block.</param>
        public Open(IEnumerable<IDocumentElement> children = null, string title = null, string masqueradeType = null, IDocumentAttributes attributes = null)
            : base("open", attributes ?? new DocumentAttributes())
        {
            Title = title;
            MasqueradeType = masqueradeType;
            
            // Add children if provided
            if (children != null)
            {
                foreach (var child in children)
                {
                    AddChild(child);
                }
            }
        }

        /// <summary>
        /// Gets the optional title of the open block.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the masquerade type of the open block.
        /// </summary>
        public string MasqueradeType { get; }

        /// <summary>
        /// Accepts a visitor for processing this open block element.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}