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

using System.Collections.Generic;

namespace AsciiDoc.Net.Core
{
    /// <summary>
    /// Represents the base interface for all elements in an AsciiDoc document tree.
    /// All document elements (paragraphs, sections, lists, etc.) implement this interface,
    /// providing a consistent tree structure with parent-child relationships and visitor pattern support.
    /// </summary>
    /// <example>
    /// <code>
    /// // Walking the document tree
    /// void PrintElement(IDocumentElement element, int depth = 0)
    /// {
    ///     var indent = new string(' ', depth * 2);
    ///     Console.WriteLine($"{indent}{element.ElementType}");
    ///     foreach (var child in element.Children)
    ///         PrintElement(child, depth + 1);
    /// }
    /// </code>
    /// </example>
    public interface IDocumentElement
    {
        /// <summary>
        /// Gets the type identifier for this element (e.g., "Paragraph", "Section", "List").
        /// This is primarily used for debugging and logging purposes.
        /// </summary>
        /// <value>A string identifying the element type.</value>
        string ElementType { get; }

        /// <summary>
        /// Gets the attributes associated with this element.
        /// Attributes include element-specific properties like CSS classes, IDs, and custom metadata.
        /// </summary>
        /// <value>The element's attribute collection.</value>
        IDocumentAttributes Attributes { get; }

        /// <summary>
        /// Gets the parent element of this element in the document tree.
        /// The root document element has a null parent.
        /// </summary>
        /// <value>The parent element, or null if this is the root element.</value>
        IDocumentElement Parent { get; }

        /// <summary>
        /// Gets a read-only collection of direct child elements.
        /// For example, a section's children might include paragraphs and subsections.
        /// </summary>
        /// <value>A read-only list of child elements in document order.</value>
        IReadOnlyList<IDocumentElement> Children { get; }

        /// <summary>
        /// Adds a child element to this element and sets the child's parent reference.
        /// The child is added at the end of the children collection.
        /// </summary>
        /// <param name="child">The element to add as a child. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="child"/> is null.</exception>
        void AddChild(IDocumentElement child);

        /// <summary>
        /// Removes a child element from this element and clears the child's parent reference.
        /// If the child is not found, no action is taken.
        /// </summary>
        /// <param name="child">The child element to remove. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="child"/> is null.</exception>
        void RemoveChild(IDocumentElement child);

        /// <summary>
        /// Accepts a visitor for traversing and processing the document tree.
        /// This implements the Visitor pattern, allowing external operations on the document structure.
        /// </summary>
        /// <param name="visitor">The visitor to accept. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="visitor"/> is null.</exception>
        /// <example>
        /// <code>
        /// var htmlConverter = new HtmlDocumentConverter();
        /// document.Accept(htmlConverter);
        /// string html = htmlConverter.GetResult();
        /// </code>
        /// </example>
        void Accept(IDocumentVisitor visitor);
    }
}