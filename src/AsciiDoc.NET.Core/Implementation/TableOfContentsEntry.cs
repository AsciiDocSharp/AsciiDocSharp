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
    /// Implementation of a table of contents entry representing a document section.
    /// </summary>
    public class TableOfContentsEntry : ITableOfContentsEntry
    {
        /// <summary>
        /// Initializes a new instance of the TableOfContentsEntry class.
        /// </summary>
        /// <param name="title">The title text of the section.</param>
        /// <param name="level">The hierarchical level of the section.</param>
        /// <param name="anchorId">The anchor identifier for linking to the section.</param>
        /// <param name="children">The child entries representing subsections.</param>
        public TableOfContentsEntry(string title, int level, string anchorId, IReadOnlyList<ITableOfContentsEntry> children = null)
        {
            Title = title ?? string.Empty;
            Level = level;
            AnchorId = anchorId ?? string.Empty;
            Children = children ?? new List<ITableOfContentsEntry>();
        }

        /// <summary>
        /// Gets the title text of the section this entry represents.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the hierarchical level of this section within the document structure.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Gets the anchor identifier used to create links to this section.
        /// </summary>
        public string AnchorId { get; }

        /// <summary>
        /// Gets the child entries representing subsections under this section.
        /// </summary>
        public IReadOnlyList<ITableOfContentsEntry> Children { get; }
    }
}