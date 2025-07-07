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
    /// Represents a single entry in a table of contents, corresponding to a document section.
    /// Each entry contains the section title, hierarchy level, and anchor information for linking.
    /// </summary>
    /// <remarks>
    /// TOC entries form a hierarchical structure matching the document's section hierarchy.
    /// Each entry can have child entries representing subsections, creating a nested navigation structure.
    /// </remarks>
    /// <example>
    /// <code>
    /// // For this section structure:
    /// // == Introduction (level 1)
    /// // === Overview (level 2) 
    /// // === Goals (level 2)
    /// // == Implementation (level 1)
    /// //
    /// // The TOC entries would be:
    /// // - Introduction (level 1, children: Overview, Goals)
    /// //   - Overview (level 2)
    /// //   - Goals (level 2)
    /// // - Implementation (level 1)
    /// </code>
    /// </example>
    public interface ITableOfContentsEntry
    {
        /// <summary>
        /// Gets the title text of the section this entry represents.
        /// This is the same text that appears in the section heading.
        /// </summary>
        /// <value>The section title text.</value>
        string Title { get; }

        /// <summary>
        /// Gets the hierarchical level of this section within the document structure.
        /// Level 1 corresponds to top-level sections (== in AsciiDoc), level 2 to subsections (===), etc.
        /// </summary>
        /// <value>The section level, starting from 1 for top-level sections.</value>
        int Level { get; }

        /// <summary>
        /// Gets the anchor identifier used to create links to this section.
        /// This corresponds to the section's ID attribute or an auto-generated identifier.
        /// </summary>
        /// <value>The anchor ID that can be used in href attributes for navigation links.</value>
        string AnchorId { get; }

        /// <summary>
        /// Gets the child entries representing subsections under this section.
        /// The collection is empty for sections that have no subsections.
        /// </summary>
        /// <value>A read-only collection of child TOC entries in document order.</value>
        IReadOnlyList<ITableOfContentsEntry> Children { get; }
    }
}