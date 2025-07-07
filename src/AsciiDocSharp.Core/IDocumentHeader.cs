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

namespace AsciiDocSharp.Core
{
    /// <summary>
    /// Represents the document header containing metadata and document-level attributes.
    /// The header includes standard metadata like title and author information,
    /// as well as custom document attributes that control processing and output formatting.
    /// </summary>
    /// <example>
    /// <code>
    /// // Example AsciiDoc header:
    /// // = Document Title
    /// // John Doe &lt;john@example.com&gt;
    /// // v1.0, 2024-01-15
    /// // :toc: left
    /// // :source-highlighter: rouge
    /// 
    /// Console.WriteLine(header.Title);    // "Document Title"
    /// Console.WriteLine(header.Author);   // "John Doe"
    /// Console.WriteLine(header.Email);    // "john@example.com"
    /// Console.WriteLine(header.Revision); // "v1.0"
    /// Console.WriteLine(header.Date);     // "2024-01-15"
    /// </code>
    /// </example>
    public interface IDocumentHeader
    {
        /// <summary>
        /// Gets the document title from the first level-0 heading.
        /// Returns null if no title is specified in the document.
        /// </summary>
        /// <value>The document title, or null if not specified.</value>
        string Title { get; }

        /// <summary>
        /// Gets the document author name from the author line.
        /// Returns null if no author is specified in the document.
        /// </summary>
        /// <value>The author name, or null if not specified.</value>
        string Author { get; }

        /// <summary>
        /// Gets the author's email address from the author line.
        /// Returns null if no email is specified in the document.
        /// </summary>
        /// <value>The author's email address, or null if not specified.</value>
        string Email { get; }

        /// <summary>
        /// Gets the document revision information from the revision line.
        /// This typically includes version number and may include revision remarks.
        /// Returns null if no revision is specified in the document.
        /// </summary>
        /// <value>The revision information, or null if not specified.</value>
        string Revision { get; }

        /// <summary>
        /// Gets the document date from the revision line.
        /// Returns null if no date is specified in the document.
        /// </summary>
        /// <value>The document date, or null if not specified.</value>
        string Date { get; }

        /// <summary>
        /// Gets the document-level attributes that control processing and output.
        /// These attributes include settings like table of contents configuration,
        /// source code highlighting, custom CSS, and other document-wide settings.
        /// </summary>
        /// <value>The document attributes collection.</value>
        IDocumentAttributes Attributes { get; }
    }
}