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
using System.Collections.Generic;

namespace AsciiDoc.Net.Core.Implementation
{
    /// <summary>
    /// Implementation of a table of contents element that generates navigation links from document sections.
    /// </summary>
    public class TableOfContents : DocumentElementBase, ITableOfContents
    {
        /// <summary>
        /// Initializes a new instance of the TableOfContents class.
        /// </summary>
        /// <param name="title">The title of the table of contents section.</param>
        /// <param name="maxDepth">The maximum depth level to include in the TOC.</param>
        /// <param name="entries">The collection of TOC entries.</param>
        public TableOfContents(string title, int maxDepth, IReadOnlyList<ITableOfContentsEntry> entries)
            : base("toc")
        {
            Title = title;
            MaxDepth = maxDepth;
            Entries = entries ?? new List<ITableOfContentsEntry>();
        }

        /// <summary>
        /// Gets the title of the table of contents section.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the maximum depth level to include in the table of contents.
        /// </summary>
        public int MaxDepth { get; }

        /// <summary>
        /// Gets the collection of table of contents entries.
        /// </summary>
        public IReadOnlyList<ITableOfContentsEntry> Entries { get; }

        /// <summary>
        /// Accepts a visitor for processing this table of contents element.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}