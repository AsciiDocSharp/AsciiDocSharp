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
using System.Linq;
using AsciiDocSharp.Core.Elements;

namespace AsciiDocSharp.Core.Implementation
{
    /// <summary>
    /// Represents a sidebar block implementation.
    /// Sidebars provide supplementary content that is visually separated from the main content.
    /// </summary>
    public class Sidebar : DocumentElementBase, ISidebar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sidebar"/> class.
        /// </summary>
        /// <param name="children">The child elements contained within the sidebar.</param>
        /// <param name="title">The optional title of the sidebar.</param>
        /// <param name="attributes">The document attributes for this sidebar.</param>
        public Sidebar(IEnumerable<IDocumentElement> children = null, string title = null, IDocumentAttributes attributes = null)
            : base("sidebar", attributes ?? new DocumentAttributes())
        {
            Title = title;
            
            // Add children if provided
            if (children != null)
            {
                foreach (var child in children)
                {
                    AddChild(child);
                }
            }
        }

        /// <summary>
        /// Gets the optional title of the sidebar.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Accepts a visitor for processing this sidebar element.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}