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

using AsciiDocSharp.Core;
using System.Collections.Generic;

namespace AsciiDocSharp.Converters.Core
{
    /// <summary>
    /// Provides contextual information and state management during document conversion.
    /// The converter context maintains the current state of the conversion process,
    /// including the element hierarchy, conversion options, and document references.
    /// </summary>
    /// <remarks>
    /// The converter context enables stateful conversion operations by tracking the current
    /// position in the document tree and providing access to parent elements. This is
    /// essential for converters that need context-aware output generation, such as
    /// nested list handling or hierarchical section numbering.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example converter using context for nested element handling
    /// public void ConvertList(IList list, IConverterContext context)
    /// {
    ///     // Check if we're inside another list for proper nesting
    ///     bool isNestedList = context.ElementStack.Any(e => e is IList);
    ///     
    ///     var cssClass = isNestedList ? "nested-list" : "top-level-list";
    ///     output.Append($"&lt;ul class=\"{cssClass}\"&gt;");
    ///     
    ///     context.PushElement(list);
    ///     // Convert child elements...
    ///     context.PopElement();
    ///     
    ///     output.Append("&lt;/ul&gt;");
    /// }
    /// </code>
    /// </example>
    public interface IConverterContext
    {
        /// <summary>
        /// Gets the root document being converted.
        /// This provides access to document-level information such as headers,
        /// global attributes, and the complete document structure.
        /// </summary>
        /// <value>The document being converted. Never null.</value>
        /// <example>
        /// <code>
        /// // Access document metadata during conversion
        /// if (context.Document.Header?.Title != null)
        /// {
        ///     var title = context.Document.Header.Title;
        ///     output.Append($"&lt;title&gt;{EscapeHtml(title)}&lt;/title&gt;");
        /// }
        /// 
        /// // Check document-level attributes
        /// var tocEnabled = context.Document.Header?.Attributes.HasAttribute("toc") ?? false;
        /// if (tocEnabled)
        /// {
        ///     GenerateTableOfContents(context.Document);
        /// }
        /// </code>
        /// </example>
        IDocument Document { get; }

        /// <summary>
        /// Gets the conversion options that control output generation behavior.
        /// These options include formatting preferences, encoding settings, and
        /// converter-specific configuration.
        /// </summary>
        /// <value>The converter options. Never null.</value>
        /// <example>
        /// <code>
        /// // Use options to control output formatting
        /// string indent = context.Options.PrettyPrint ? "  " : "";
        /// string newline = context.Options.PrettyPrint ? "\n" : "";
        /// 
        /// output.Append($"{indent}&lt;p&gt;{newline}");
        /// output.Append($"{indent}  {content}{newline}");
        /// output.Append($"{indent}&lt;/p&gt;{newline}");
        /// 
        /// // Access custom properties
        /// if (context.Options.CustomProperties.TryGetValue("syntax-highlighter", out var highlighter))
        /// {
        ///     ApplySyntaxHighlighting(codeBlock, highlighter.ToString());
        /// }
        /// </code>
        /// </example>
        IConverterOptions Options { get; }

        /// <summary>
        /// Gets the document element currently being processed.
        /// This is the element at the top of the element stack and represents
        /// the immediate context for conversion operations.
        /// </summary>
        /// <value>
        /// The current element being converted, or null if no element is currently being processed.
        /// </value>
        /// <example>
        /// <code>
        /// // Use current element for context-aware conversion
        /// if (context.CurrentElement is ISection section)
        /// {
        ///     int level = GetSectionLevel(section);
        ///     output.Append($"&lt;h{level}&gt;{EscapeHtml(section.Title)}&lt;/h{level}&gt;");
        /// }
        /// 
        /// // Access element attributes
        /// var cssClasses = context.CurrentElement?.Attributes.GetAttribute("class") ?? "";
        /// if (!string.IsNullOrEmpty(cssClasses))
        /// {
        ///     output.Append($" class=\"{EscapeHtml(cssClasses)}\"");
        /// }
        /// </code>
        /// </example>
        IDocumentElement CurrentElement { get; }

        /// <summary>
        /// Gets the stack of elements representing the current conversion hierarchy.
        /// This stack tracks the path from the root document to the currently processing element,
        /// enabling context-aware conversion decisions based on parent elements.
        /// </summary>
        /// <value>A stack of document elements with the current element at the top. Never null.</value>
        /// <example>
        /// <code>
        /// // Check for specific parent element types
        /// bool insideTable = context.ElementStack.Any(e => e is ITable);
        /// bool insideCodeBlock = context.ElementStack.Any(e => e is ICodeBlock);
        /// 
        /// // Find the immediate parent section
        /// var parentSection = context.ElementStack
        ///     .OfType&lt;ISection&gt;()
        ///     .FirstOrDefault();
        /// 
        /// if (parentSection != null)
        /// {
        ///     var sectionId = GenerateSectionId(parentSection.Title);
        ///     output.Append($"&lt;div data-section=\"{sectionId}\"&gt;");
        /// }
        /// </code>
        /// </example>
        Stack<IDocumentElement> ElementStack { get; }

        /// <summary>
        /// Pushes a document element onto the context stack, making it the current element.
        /// This method should be called when beginning to process a new element to establish
        /// the proper conversion context for nested elements.
        /// </summary>
        /// <param name="element">The element to push onto the stack. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
        /// <example>
        /// <code>
        /// // Proper element stack management during conversion
        /// public void ConvertSection(ISection section, IConverterContext context)
        /// {
        ///     context.PushElement(section);
        ///     
        ///     try
        ///     {
        ///         // Convert section header
        ///         ConvertSectionHeader(section, context);
        ///         
        ///         // Convert section content
        ///         foreach (var child in section.Children)
        ///         {
        ///             ConvertElement(child, context);
        ///         }
        ///     }
        ///     finally
        ///     {
        ///         context.PopElement(); // Always clean up the stack
        ///     }
        /// }
        /// </code>
        /// </example>
        void PushElement(IDocumentElement element);

        /// <summary>
        /// Removes and returns the current element from the context stack.
        /// This method should be called when finishing the processing of an element
        /// to restore the previous conversion context.
        /// </summary>
        /// <returns>
        /// The element that was removed from the top of the stack,
        /// or null if the stack is empty.
        /// </returns>
        /// <example>
        /// <code>
        /// // Balanced push/pop operations
        /// context.PushElement(listElement);
        /// 
        /// // Process list items...
        /// foreach (var item in listElement.Children.OfType&lt;IListItem&gt;())
        /// {
        ///     ConvertListItem(item, context);
        /// }
        /// 
        /// var poppedElement = context.PopElement();
        /// Debug.Assert(poppedElement == listElement); // Verify stack integrity
        /// </code>
        /// </example>
        IDocumentElement PopElement();
    }
}