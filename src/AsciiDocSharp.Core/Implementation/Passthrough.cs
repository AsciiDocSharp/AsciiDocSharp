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

using AsciiDocSharp.Core.Elements;

namespace AsciiDocSharp.Core.Implementation
{
    /// <summary>
    /// Represents a passthrough block implementation.
    /// Passthrough blocks allow raw content to bypass normal AsciiDoc substitutions
    /// and pass directly to the output format.
    /// </summary>
    public class Passthrough : DocumentElementBase, IPassthrough
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Passthrough"/> class.
        /// </summary>
        /// <param name="content">The raw content of the passthrough block.</param>
        /// <param name="title">The optional title of the passthrough block.</param>
        /// <param name="substitutions">The substitution options for the passthrough block.</param>
        /// <param name="attributes">The document attributes for this passthrough block.</param>
        public Passthrough(string content, string title = null, string substitutions = null, IDocumentAttributes attributes = null)
            : base("passthrough", attributes ?? new DocumentAttributes())
        {
            Content = content ?? string.Empty;
            Title = title;
            Substitutions = substitutions;
        }

        /// <summary>
        /// Gets the raw content of the passthrough block.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the optional title of the passthrough block.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the substitution options for the passthrough block.
        /// </summary>
        public string Substitutions { get; }

        /// <summary>
        /// Accepts a visitor for processing this passthrough block element.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}