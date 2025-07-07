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

namespace AsciiDoc.Net.Core.Elements
{
    /// <summary>
    /// Represents a footnote reference that appears inline in the document text.
    /// Footnotes provide supplementary information or citations that appear at the bottom
    /// of the page or section, linked from the main text via a reference marker.
    /// </summary>
    /// <remarks>
    /// In AsciiDoc, footnotes are created using the `footnote:[text]` or `footnoteref:[id,text]` syntax.
    /// The footnote reference appears as a superscript number or symbol in the text,
    /// with the full footnote content displayed elsewhere in the document.
    /// </remarks>
    /// <example>
    /// <code>
    /// // AsciiDoc syntax:
    /// // This is some text footnote:[This is a footnote.] with a reference.
    /// //
    /// // Creates inline reference linked to footnote at bottom of document
    /// </code>
    /// </example>
    public interface IFootnote : IDocumentElement
    {
        /// <summary>
        /// Gets the unique identifier for this footnote.
        /// Used to link the inline reference to the footnote content.
        /// </summary>
        /// <value>The footnote identifier, auto-generated if not specified.</value>
        string Id { get; }

        /// <summary>
        /// Gets the text content of the footnote.
        /// This is the explanatory text that appears in the footnote section.
        /// </summary>
        /// <value>The footnote text content.</value>
        string Text { get; }

        /// <summary>
        /// Gets the reference number or label for this footnote.
        /// This is the visible marker that appears in the main text.
        /// </summary>
        /// <value>The footnote reference label (typically a number).</value>
        string ReferenceLabel { get; }

        /// <summary>
        /// Gets a value indicating whether this is a footnote reference only.
        /// True for footnoteref citations that refer to an existing footnote definition.
        /// </summary>
        /// <value>True if this is a reference-only footnote; false if it contains content.</value>
        bool IsReference { get; }
    }
}