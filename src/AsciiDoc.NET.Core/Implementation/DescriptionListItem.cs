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

namespace AsciiDoc.NET.Core.Implementation
{
    /// <summary>
    /// Implementation of a description list item containing a term and its definition.
    /// Represents items parsed from "term:: description" syntax in AsciiDoc.
    /// </summary>
    public class DescriptionListItem : DocumentElementBase, IDescriptionListItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionListItem"/> class.
        /// </summary>
        /// <param name="term">The term being defined.</param>
        /// <param name="description">The description or definition of the term.</param>
        public DescriptionListItem(string term, string description) : base("description-list-item")
        {
            Term = term ?? string.Empty;
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// Gets the term being defined in this description list item.
        /// </summary>
        public string Term { get; }

        /// <summary>
        /// Gets the description or definition of the term.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Accepts a visitor for processing this description list item element.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}