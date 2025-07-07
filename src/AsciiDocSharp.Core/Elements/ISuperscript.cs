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

namespace AsciiDocSharp.Core.Elements
{
    /// <summary>
    /// Represents superscript text formatting in an AsciiDoc document.
    /// Superscript text is typically rendered smaller and raised above the baseline,
    /// commonly used for mathematical expressions, footnote references, and ordinal indicators.
    /// </summary>
    /// <remarks>
    /// In AsciiDoc, superscript text is denoted using caret symbols: ^text^
    /// This is useful for mathematical notation (x^2^), scientific notation (10^6^),
    /// ordinal numbers (1^st^, 2^nd^), and footnote references.
    /// </remarks>
    /// <example>
    /// <para>AsciiDoc syntax:</para>
    /// <code>
    /// The formula is E=mc^2^ where c is the speed of light.
    /// This is the 1^st^ example of superscript formatting.
    /// </code>
    /// </example>
    public interface ISuperscript : IDocumentElement
    {
        /// <summary>
        /// Gets the text content that should be rendered as superscript.
        /// </summary>
        /// <value>The text to be displayed in superscript format.</value>
        string Text { get; }
    }
}