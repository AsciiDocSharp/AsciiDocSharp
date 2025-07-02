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
    /// Represents a sidebar block in an AsciiDoc document.
    /// Sidebars provide supplementary content that is visually separated from the main content,
    /// typically rendered in a box or highlighted area.
    /// </summary>
    /// <remarks>
    /// Sidebar blocks in AsciiDoc can be created using delimited blocks with four asterisks (****)
    /// or by applying a [sidebar] attribute to a paragraph. They can contain any block content
    /// including paragraphs, lists, tables, and other elements.
    /// </remarks>
    /// <example>
    /// <para>AsciiDoc syntax:</para>
    /// <code>
    /// .Optional Title
    /// ****
    /// This is sidebar content.
    /// 
    /// * It can contain lists
    /// * And multiple paragraphs
    /// ****
    /// </code>
    /// </example>
    public interface ISidebar : IDocumentElement
    {
        /// <summary>
        /// Gets the optional title of the sidebar.
        /// Titles are specified using the dot notation before the sidebar delimiter.
        /// </summary>
        /// <value>The sidebar title, or null if no title is specified.</value>
        string Title { get; }

    }
}