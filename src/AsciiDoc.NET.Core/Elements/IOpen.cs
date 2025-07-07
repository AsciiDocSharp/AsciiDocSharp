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

using System.Collections.Generic;

namespace AsciiDoc.NET.Core.Elements
{
    /// <summary>
    /// Represents an open block in an AsciiDoc document.
    /// Open blocks are versatile containers that can enclose content without adding specific semantic meaning,
    /// or can masquerade as other block types when combined with attributes.
    /// </summary>
    /// <remarks>
    /// Open blocks in AsciiDoc are created using delimited blocks with double hyphens (--).
    /// They serve as generic structural containers that can adapt to different presentation needs.
    /// Open blocks can contain any block content including paragraphs, lists, code blocks, and other elements.
    /// A key limitation is that open blocks cannot be nested inside another open block.
    /// </remarks>
    /// <example>
    /// <para>AsciiDoc syntax for anonymous container:</para>
    /// <code>
    /// --
    /// An open block can be an anonymous container,
    /// or it can masquerade as any other block.
    /// --
    /// </code>
    /// <para>AsciiDoc syntax masquerading as sidebar:</para>
    /// <code>
    /// [sidebar]
    /// .Related information
    /// --
    /// This is aside text.
    /// 
    /// It is used to present information related to the main content.
    /// --
    /// </code>
    /// </example>
    public interface IOpen : IDocumentElement
    {
        /// <summary>
        /// Gets the optional title of the open block.
        /// Titles are specified using the dot notation before the open block delimiter.
        /// </summary>
        /// <value>The open block title, or null if no title is specified.</value>
        string Title { get; }

        /// <summary>
        /// Gets the masquerade type of the open block.
        /// Open blocks can masquerade as other block types (e.g., sidebar, source) when
        /// combined with appropriate attributes. This property indicates what type the
        /// open block is masquerading as, or null if it's an anonymous container.
        /// </summary>
        /// <value>The masquerade type (e.g., "sidebar", "source"), or null for anonymous containers.</value>
        string MasqueradeType { get; }
    }
}