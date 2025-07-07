// AsciiDocSharp
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

namespace AsciiDocSharp.Core.Elements
{
    /// <summary>
    /// Represents a table of contents element that provides navigation links to document sections.
    /// The table of contents is typically generated automatically from the section headings
    /// in the document, creating a hierarchical list of links.
    /// </summary>
    /// <remarks>
    /// In AsciiDoc, a table of contents can be inserted using the `toc::[]` macro or by
    /// setting the `toc` document attribute. The TOC includes entries for all sections
    /// with their titles and links to the corresponding section anchors.
    /// </remarks>
    /// <example>
    /// <code>
    /// // AsciiDoc markup for TOC:
    /// // toc::[]
    /// // 
    /// // = Document Title
    /// // == First Section  
    /// // === Subsection
    /// // == Second Section
    /// </code>
    /// </example>
    public interface ITableOfContents : IDocumentElement
    {
        /// <summary>
        /// Gets the title of the table of contents section.
        /// This is typically "Table of Contents" or a localized equivalent.
        /// </summary>
        /// <value>The title text for the TOC section, or null if no title is specified.</value>
        string Title { get; }

        /// <summary>
        /// Gets the maximum depth level to include in the table of contents.
        /// For example, a depth of 2 includes sections up to level 2 (== in AsciiDoc).
        /// </summary>
        /// <value>The maximum section level to include, with 1 being the top level.</value>
        int MaxDepth { get; }

        /// <summary>
        /// Gets the collection of table of contents entries organized hierarchically.
        /// Each entry represents a document section with its title, level, and anchor reference.
        /// </summary>
        /// <value>A read-only collection of TOC entries in document order.</value>
        IReadOnlyList<ITableOfContentsEntry> Entries { get; }
    }
}