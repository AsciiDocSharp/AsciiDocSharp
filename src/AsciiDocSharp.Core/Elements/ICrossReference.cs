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
    /// Represents a cross-reference to another element within the same document.
    /// Cross-references create navigation links to anchors, sections, or other referenceable elements.
    /// </summary>
    public interface ICrossReference : IDocumentElement
    {
        /// <summary>
        /// Gets the target identifier that this cross-reference points to.
        /// </summary>
        string TargetId { get; }

        /// <summary>
        /// Gets the optional custom link text for this cross-reference.
        /// If null or empty, the target element's title or label should be used.
        /// </summary>
        string LinkText { get; }

        /// <summary>
        /// Gets the resolved target element, if available.
        /// This may be null during parsing but should be resolved before conversion.
        /// </summary>
        IDocumentElement ResolvedTarget { get; }
    }
}