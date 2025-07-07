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

namespace AsciiDoc.NET.Core.Elements
{
    /// <summary>
    /// Represents a verse block in an AsciiDoc document.
    /// Verse blocks preserve line breaks and white space, making them ideal for poetry,
    /// song lyrics, addresses, and other content where formatting is significant.
    /// </summary>
    /// <remarks>
    /// Verse blocks in AsciiDoc use the [verse] block style and are typically delimited
    /// with four underscores (____). They preserve the original line breaks and spacing
    /// from the source document, unlike regular paragraphs which flow text.
    /// </remarks>
    /// <example>
    /// <para>AsciiDoc syntax:</para>
    /// <code>
    /// [verse, Carl Sandburg, Chicago]
    /// ____
    /// Hog Butcher for the World,
    /// Tool Maker, Stacker of Wheat,
    /// Player with Railroads and the Nation's Freight Handler;
    /// Stormy, husky, brawling,
    /// City of Big Shoulders
    /// ____
    /// </code>
    /// </example>
    public interface IVerse : IDocumentElement
    {
        /// <summary>
        /// Gets the text content of the verse block with preserved line breaks.
        /// </summary>
        /// <value>The verse content as a string with line breaks preserved.</value>
        string Content { get; }

        /// <summary>
        /// Gets the optional title of the verse block.
        /// </summary>
        /// <value>The verse title, or null if no title is specified.</value>
        string Title { get; }

        /// <summary>
        /// Gets the optional author attribution for the verse.
        /// </summary>
        /// <value>The verse author, or null if no author is specified.</value>
        string Author { get; }

        /// <summary>
        /// Gets the optional citation or source information for the verse.
        /// </summary>
        /// <value>The verse citation, or null if no citation is specified.</value>
        string Citation { get; }
    }
}