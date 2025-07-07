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

namespace AsciiDocSharp.Core
{
    /// <summary>
    /// Represents the root document node in an AsciiDoc document tree.
    /// This is the top-level container that holds the document header and all content elements.
    /// </summary>
    /// <example>
    /// <code>
    /// var parser = new AsciiDocParser();
    /// IDocument document = parser.Parse("= My Document\n\nHello world!");
    /// Console.WriteLine(document.Header.Title); // "My Document"
    /// Console.WriteLine(document.Elements.Count); // 1 (paragraph)
    /// </code>
    /// </example>
    public interface IDocument : IDocumentElement
    {
        /// <summary>
        /// Gets the document header containing metadata such as title, author, and document attributes.
        /// The header may be null if the document has no explicit header section.
        /// </summary>
        /// <value>The document header, or null if no header is present.</value>
        IDocumentHeader Header { get; }

        /// <summary>
        /// Gets a read-only collection of all top-level elements in the document body.
        /// This includes sections, paragraphs, code blocks, tables, and other block-level elements.
        /// </summary>
        /// <value>A read-only list of document elements in document order.</value>
        IReadOnlyList<IDocumentElement> Elements { get; }
    }
}