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

using AsciiDoc.NET.Core.Elements;

namespace AsciiDoc.NET.Core.Implementation
{
    /// <summary>
    /// Represents a verse block implementation.
    /// Verse blocks preserve line breaks and white space, making them ideal for poetry,
    /// song lyrics, addresses, and other content where formatting is significant.
    /// </summary>
    public class Verse : DocumentElementBase, IVerse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Verse"/> class.
        /// </summary>
        /// <param name="content">The verse content with preserved line breaks.</param>
        /// <param name="title">The optional title of the verse block.</param>
        /// <param name="author">The optional author attribution for the verse.</param>
        /// <param name="citation">The optional citation or source information for the verse.</param>
        /// <param name="attributes">The document attributes for this verse block.</param>
        public Verse(string content, string title = null, string author = null, string citation = null, IDocumentAttributes attributes = null)
            : base("verse", attributes ?? new DocumentAttributes())
        {
            Content = content ?? string.Empty;
            Title = title;
            Author = author;
            Citation = citation;
        }

        /// <summary>
        /// Gets the text content of the verse block with preserved line breaks.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the optional title of the verse block.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the optional author attribution for the verse.
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Gets the optional citation or source information for the verse.
        /// </summary>
        public string Citation { get; }

        /// <summary>
        /// Accepts a visitor for processing this verse element.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}