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

namespace AsciiDoc.NET.Core
{
    /// <summary>
    /// Defines the Visitor pattern interface for traversing and processing document elements.
    /// Implementations can perform operations like converting to HTML, extracting text,
    /// validating structure, or gathering statistics without modifying the document tree.
    /// </summary>
    /// <remarks>
    /// The visitor pattern provides type-safe method dispatch and allows adding new operations
    /// to the document model without modifying existing element classes. Each element type
    /// has a corresponding Visit method that receives the strongly-typed element.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example visitor implementation for counting elements
    /// public class ElementCountVisitor : IDocumentVisitor
    /// {
    ///     private readonly Dictionary&lt;string, int&gt; _counts = new();
    ///     
    ///     public void Visit(IParagraph paragraph)
    ///     {
    ///         _counts["Paragraph"] = _counts.GetValueOrDefault("Paragraph") + 1;
    ///         // Visit children
    ///         foreach (var child in paragraph.Children)
    ///             child.Accept(this);
    ///     }
    ///     
    ///     // ... other Visit methods
    /// }
    /// </code>
    /// </example>
    public interface IDocumentVisitor
    {
        /// <summary>
        /// Visits a document element (the root of the document tree).
        /// </summary>
        /// <param name="document">The document to visit.</param>
        void Visit(IDocument document);

        /// <summary>
        /// Visits a section element (headings and their content).
        /// </summary>
        /// <param name="section">The section to visit.</param>
        void Visit(ISection section);

        /// <summary>
        /// Visits a paragraph element (block of regular text).
        /// </summary>
        /// <param name="paragraph">The paragraph to visit.</param>
        void Visit(IParagraph paragraph);

        /// <summary>
        /// Visits a list element (ordered or unordered list container).
        /// </summary>
        /// <param name="list">The list to visit.</param>
        void Visit(IList list);

        /// <summary>
        /// Visits a list item element (individual item within a list).
        /// </summary>
        /// <param name="listItem">The list item to visit.</param>
        void Visit(IListItem listItem);

        /// <summary>
        /// Visits a description list element (definition list with term-description pairs).
        /// </summary>
        /// <param name="descriptionList">The description list to visit.</param>
        void Visit(IDescriptionList descriptionList);

        /// <summary>
        /// Visits a description list item element (term and definition pair).
        /// </summary>
        /// <param name="descriptionListItem">The description list item to visit.</param>
        void Visit(IDescriptionListItem descriptionListItem);

        /// <summary>
        /// Visits a text element (plain text content).
        /// </summary>
        /// <param name="text">The text element to visit.</param>
        void Visit(IText text);

        /// <summary>
        /// Visits an emphasis element (italic text formatting).
        /// </summary>
        /// <param name="emphasis">The emphasis element to visit.</param>
        void Visit(IEmphasis emphasis);

        /// <summary>
        /// Visits a strong element (bold text formatting).
        /// </summary>
        /// <param name="strong">The strong element to visit.</param>
        void Visit(IStrong strong);

        /// <summary>
        /// Visits a highlight element (highlighted text formatting).
        /// </summary>
        /// <param name="highlight">The highlight element to visit.</param>
        void Visit(IHighlight highlight);

        /// <summary>
        /// Visits a superscript element (superscript text formatting).
        /// </summary>
        /// <param name="superscript">The superscript element to visit.</param>
        void Visit(ISuperscript superscript);

        /// <summary>
        /// Visits a subscript element (subscript text formatting).
        /// </summary>
        /// <param name="subscript">The subscript element to visit.</param>
        void Visit(ISubscript subscript);

        /// <summary>
        /// Visits a link element (hyperlink to external or internal resources).
        /// </summary>
        /// <param name="link">The link element to visit.</param>
        void Visit(ILink link);

        /// <summary>
        /// Visits an image element (embedded image reference).
        /// </summary>
        /// <param name="image">The image element to visit.</param>
        void Visit(IImage image);

        /// <summary>
        /// Visits a code block element (multi-line preformatted code).
        /// </summary>
        /// <param name="codeBlock">The code block to visit.</param>
        void Visit(ICodeBlock codeBlock);

        /// <summary>
        /// Visits an inline code element (single-line code within text).
        /// </summary>
        /// <param name="inlineCode">The inline code element to visit.</param>
        void Visit(IInlineCode inlineCode);

        /// <summary>
        /// Visits a table element (tabular data container).
        /// </summary>
        /// <param name="table">The table to visit.</param>
        void Visit(ITable table);

        /// <summary>
        /// Visits a table row element (horizontal row of table cells).
        /// </summary>
        /// <param name="tableRow">The table row to visit.</param>
        void Visit(ITableRow tableRow);

        /// <summary>
        /// Visits a table cell element (individual data cell within a table row).
        /// </summary>
        /// <param name="tableCell">The table cell to visit.</param>
        void Visit(ITableCell tableCell);

        /// <summary>
        /// Visits a table header element (header cell within a table).
        /// </summary>
        /// <param name="tableHeader">The table header to visit.</param>
        void Visit(ITableHeader tableHeader);

        /// <summary>
        /// Visits a block quote element (quoted text block).
        /// </summary>
        /// <param name="blockQuote">The block quote to visit.</param>
        void Visit(IBlockQuote blockQuote);

        /// <summary>
        /// Visits a sidebar element (supplementary content block).
        /// </summary>
        /// <param name="sidebar">The sidebar to visit.</param>
        void Visit(ISidebar sidebar);

        /// <summary>
        /// Visits an example block element (demonstration or sample content).
        /// </summary>
        /// <param name="example">The example block to visit.</param>
        void Visit(IExample example);

        /// <summary>
        /// Visits an admonition element (callout box like NOTE, WARNING, etc.).
        /// </summary>
        /// <param name="admonition">The admonition to visit.</param>
        void Visit(IAdmonition admonition);

        /// <summary>
        /// Visits an anchor element (named location for cross-references).
        /// </summary>
        /// <param name="anchor">The anchor to visit.</param>
        void Visit(IAnchor anchor);

        /// <summary>
        /// Visits a cross-reference element (link to an anchor within the document).
        /// </summary>
        /// <param name="crossReference">The cross-reference to visit.</param>
        void Visit(ICrossReference crossReference);

        /// <summary>
        /// Visits a macro element (general-purpose macro invocation).
        /// </summary>
        /// <param name="macro">The macro to visit.</param>
        void Visit(IMacro macro);

        /// <summary>
        /// Visits an image macro element (image inclusion with advanced options).
        /// </summary>
        /// <param name="imageMacro">The image macro to visit.</param>
        void Visit(IImageMacro imageMacro);

        /// <summary>
        /// Visits a video macro element (video embedding).
        /// </summary>
        /// <param name="videoMacro">The video macro to visit.</param>
        void Visit(IVideoMacro videoMacro);

        /// <summary>
        /// Visits an include macro element (file inclusion directive).
        /// </summary>
        /// <param name="includeMacro">The include macro to visit.</param>
        void Visit(IIncludeMacro includeMacro);

        /// <summary>
        /// Visits a table of contents element (navigation structure for document sections).
        /// </summary>
        /// <param name="tableOfContents">The table of contents to visit.</param>
        void Visit(ITableOfContents tableOfContents);

        /// <summary>
        /// Visits a footnote element (supplementary information linked from main text).
        /// </summary>
        /// <param name="footnote">The footnote to visit.</param>
        void Visit(IFootnote footnote);
    }
}