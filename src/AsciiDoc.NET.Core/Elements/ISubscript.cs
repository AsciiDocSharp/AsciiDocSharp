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
    /// Represents subscript text formatting in an AsciiDoc document.
    /// Subscript text is typically rendered smaller and lowered below the baseline,
    /// commonly used for chemical formulas, mathematical expressions, and footnote references.
    /// </summary>
    /// <remarks>
    /// In AsciiDoc, subscript text is denoted using tilde symbols: ~text~
    /// This is useful for chemical formulas (H~2~O), mathematical notation (x~i~),
    /// and various scientific expressions that require baseline-lowered text.
    /// </remarks>
    /// <example>
    /// <para>AsciiDoc syntax:</para>
    /// <code>
    /// The chemical formula for water is H~2~O.
    /// The array element x~i~ represents the i-th item.
    /// </code>
    /// </example>
    public interface ISubscript : IDocumentElement
    {
        /// <summary>
        /// Gets the text content that should be rendered as subscript.
        /// </summary>
        /// <value>The text to be displayed in subscript format.</value>
        string Text { get; }
    }
}