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

using AsciiDoc.NET.Core;
using AsciiDoc.NET.Core.Elements;
using AsciiDoc.NET.Converters.Core;
using System;
using System.Linq;
using System.Text;

namespace AsciiDoc.NET.Converters.Html
{
    /// <summary>
    /// Converts AsciiDoc documents to HTML format with semantic markup and CSS class support.
    /// This converter produces clean, standards-compliant HTML5 output that can be styled
    /// with CSS frameworks or custom stylesheets.
    /// </summary>
    /// <remarks>
    /// The HTML converter implements the visitor pattern to traverse the document tree and
    /// generate corresponding HTML elements. It supports all core AsciiDoc features including
    /// sections, lists, tables, code blocks, and inline formatting. The output includes
    /// semantic CSS classes for styling flexibility.
    /// </remarks>
    public class HtmlDocumentConverter : DocumentConverterBase<string>
    {
        /// <summary>
        /// Converts the complete document structure to HTML format.
        /// This is the main entry point that orchestrates the conversion of the document header
        /// and body elements into a complete HTML representation.
        /// </summary>
        /// <param name="document">The parsed AsciiDoc document to convert.</param>
        /// <param name="context">The conversion context containing options and state.</param>
        /// <returns>Complete HTML document as a string.</returns>
        protected override string ConvertDocument(IDocument document, IConverterContext context)
        {
            // Pre-size StringBuilder based on estimated output size for better performance
            var estimatedCapacity = EstimateOutputCapacity(document);
            var html = new StringBuilder(estimatedCapacity);
            
            // Convert document header if present (title, author, revision info)
            if (document.Header != null)
            {
                ConvertHeader(document.Header, html, context);
            }

            // Convert all document body elements in order
            foreach (var element in document.Elements)
            {
                ConvertElement(element, html, context);
            }

            // Wrap the content in proper HTML document structure if needed
            return WrapInHtmlDocument(html.ToString(), document, context);
        }

        /// <summary>
        /// Converts the document header (title, author, revision) to HTML markup.
        /// The header is rendered as semantic HTML with appropriate CSS classes for styling.
        /// </summary>
        /// <param name="header">The document header containing metadata.</param>
        /// <param name="html">The StringBuilder to append HTML output to.</param>
        /// <param name="context">The conversion context.</param>
        private void ConvertHeader(IDocumentHeader header, StringBuilder html, IConverterContext context)
        {
            // Batch header operations to reduce StringBuilder calls
            var headerContent = new StringBuilder(256);
            
            // Convert document title as main heading (h1)
            if (!string.IsNullOrEmpty(header.Title))
            {
                headerContent.AppendLine($"<h1>{EscapeHtml(header.Title)}</h1>");
            }

            // Convert author information with semantic class
            if (!string.IsNullOrEmpty(header.Author))
            {
                headerContent.AppendLine($"<div class=\"author\">{EscapeHtml(header.Author)}</div>");
            }

            // Convert revision information (version, date)
            if (!string.IsNullOrEmpty(header.Revision))
            {
                headerContent.AppendLine($"<div class=\"revision\">{EscapeHtml(header.Revision)}</div>");
            }
            
            // Append the batched header content in one operation
            html.Append(headerContent);
        }

        /// <summary>
        /// Central dispatch method that converts any document element to its HTML representation.
        /// This method implements type-based dispatch to call the appropriate conversion method
        /// for each element type while maintaining proper context stack management.
        /// </summary>
        /// <param name="element">The document element to convert.</param>
        /// <param name="html">The StringBuilder to append HTML output to.</param>
        /// <param name="context">The conversion context for state management.</param>
        private void ConvertElement(IDocumentElement element, StringBuilder html, IConverterContext context)
        {
            // Push element onto context stack for nested element processing
            context.PushElement(element);

            // Dispatch to specific conversion method based on element type
            switch (element)
            {
                case ISection section:
                    ConvertSection(section, html, context);
                    break;
                case IParagraph paragraph:
                    ConvertParagraph(paragraph, html, context);
                    break;
                case IDescriptionList descriptionList:
                    ConvertDescriptionList(descriptionList, html, context);
                    break;
                case IList list:
                    ConvertList(list, html, context);
                    break;
                case ITable table:
                    ConvertTable(table, html, context);
                    break;
                case IBlockQuote blockQuote:
                    ConvertBlockQuote(blockQuote, html, context);
                    break;
                case ISidebar sidebar:
                    ConvertSidebar(sidebar, html, context);
                    break;
                case IExample example:
                    ConvertExample(example, html, context);
                    break;
                case IAdmonition admonition:
                    ConvertAdmonition(admonition, html, context);
                    break;
                case ICodeBlock codeBlock:
                    ConvertCodeBlock(codeBlock, html, context);
                    break;
                case IAnchor anchor:
                    ConvertAnchor(anchor, html, context);
                    break;
                case ICrossReference crossReference:
                    ConvertCrossReference(crossReference, html, context);
                    break;
                case IImageMacro imageMacro:
                    ConvertImageMacro(imageMacro, html, context);
                    break;
                case IVideoMacro videoMacro:
                    ConvertVideoMacro(videoMacro, html, context);
                    break;
                case IIncludeMacro includeMacro:
                    ConvertIncludeMacro(includeMacro, html, context);
                    break;
                case ITableOfContents tableOfContents:
                    ConvertTableOfContents(tableOfContents, html, context);
                    break;
                case IFootnote footnote:
                    ConvertFootnote(footnote, html, context);
                    break;
                case IMacro macro:
                    ConvertMacro(macro, html, context);
                    break;
                default:
                    ConvertGenericElement(element, html, context);
                    break;
            }

            context.PopElement();
        }

        private void ConvertSection(ISection section, StringBuilder html, IConverterContext context)
        {
            // Only render header if the section has a title (not used as include container)
            if (!string.IsNullOrEmpty(section.Title))
            {
                var level = section.Level + 1; // H1 is reserved for document title
                html.AppendLine($"<h{level}>{EscapeHtml(section.Title)}</h{level}>");
            }

            foreach (var child in section.Children)
            {
                ConvertElement(child, html, context);
            }
        }

        private void ConvertParagraph(IParagraph paragraph, StringBuilder html, IConverterContext context)
        {
            html.Append("<p>");
            
            if (paragraph.Children.Any())
            {
                // Convert child elements (inline elements)
                foreach (var child in paragraph.Children)
                {
                    ConvertElement(child, html, context);
                }
            }
            else
            {
                // Fallback to plain text for simple paragraphs
                html.Append(EscapeHtml(paragraph.Text));
            }
            
            html.AppendLine("</p>");
        }

        private void ConvertList(IList list, StringBuilder html, IConverterContext context)
        {
            var tagName = list.Type == ListType.Ordered ? "ol" : "ul";
            if (list.Type == ListType.Ordered && list.StartNumber > 1)
            {
                html.AppendLine($"<{tagName} start=\"{list.StartNumber}\">");
            }
            else
            {
                html.AppendLine($"<{tagName}>");
            }

            foreach (var item in list.Children.OfType<IListItem>())
            {
                html.Append("<li>");
                
                // Handle checkbox items
                if (item.IsCheckbox)
                {
                    var checkedAttribute = item.IsChecked ? " checked" : "";
                    html.Append($"<input type=\"checkbox\" disabled{checkedAttribute}> ");
                }
                
                html.Append(EscapeHtml(item.Text));
                
                foreach (var child in item.Children)
                {
                    ConvertElement(child, html, context);
                }
                
                html.AppendLine("</li>");
            }

            html.AppendLine($"</{tagName}>");
        }

        private void ConvertDescriptionList(IDescriptionList descriptionList, StringBuilder html, IConverterContext context)
        {
            html.AppendLine("<dl>");

            foreach (var item in descriptionList.Children.OfType<IDescriptionListItem>())
            {
                html.AppendLine($"<dt>{EscapeHtml(item.Term)}</dt>");
                html.AppendLine($"<dd>{EscapeHtml(item.Description)}</dd>");
            }

            html.AppendLine("</dl>");
        }

        private void ConvertCodeBlock(ICodeBlock codeBlock, StringBuilder html, IConverterContext context)
        {
            // Optimize by batching string operations instead of multiple small appends
            var languageClass = !string.IsNullOrEmpty(codeBlock.Language) 
                ? $" class=\"language-{EscapeHtml(codeBlock.Language)}\""
                : string.Empty;
            
            html.Append($"<pre><code{languageClass}>{EscapeHtml(codeBlock.Content)}</code></pre>");
            html.AppendLine();
        }

        private void ConvertBlockQuote(IBlockQuote blockQuote, StringBuilder html, IConverterContext context)
        {
            html.AppendLine("<blockquote>");
            
            // Convert content (split by lines and wrap in paragraphs if needed)
            var lines = blockQuote.Content.Split('\n');
            if (lines.Length == 1)
            {
                html.AppendLine($"<p>{EscapeHtml(blockQuote.Content)}</p>");
            }
            else
            {
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        html.AppendLine($"<p>{EscapeHtml(line.Trim())}</p>");
                    }
                }
            }
            
            // Add attribution if present
            if (!string.IsNullOrEmpty(blockQuote.Attribution))
            {
                html.Append("<cite>");
                html.Append(EscapeHtml(blockQuote.Attribution));
                if (!string.IsNullOrEmpty(blockQuote.Cite))
                {
                    html.Append($", <em>{EscapeHtml(blockQuote.Cite)}</em>");
                }
                html.AppendLine("</cite>");
            }
            
            html.AppendLine("</blockquote>");
        }

        private void ConvertSidebar(ISidebar sidebar, StringBuilder html, IConverterContext context)
        {
            html.AppendLine("<div class=\"sidebarblock\">");
            
            // Add title if present
            if (!string.IsNullOrEmpty(sidebar.Title))
            {
                html.AppendLine("<div class=\"title\">");
                html.AppendLine(EscapeHtml(sidebar.Title));
                html.AppendLine("</div>");
            }
            
            // Add content wrapper
            html.AppendLine("<div class=\"content\">");
            
            // Convert child elements
            foreach (var child in sidebar.Children)
            {
                ConvertElement(child, html, context);
            }
            
            html.AppendLine("</div>");
            html.AppendLine("</div>");
        }

        private void ConvertExample(IExample example, StringBuilder html, IConverterContext context)
        {
            html.AppendLine("<div class=\"exampleblock\">");
            
            // Add title if present
            if (!string.IsNullOrEmpty(example.Title))
            {
                html.AppendLine("<div class=\"title\">");
                html.AppendLine(EscapeHtml(example.Title));
                html.AppendLine("</div>");
            }
            
            // Add content wrapper
            html.AppendLine("<div class=\"content\">");
            
            // Convert child elements
            foreach (var child in example.Children)
            {
                ConvertElement(child, html, context);
            }
            
            html.AppendLine("</div>");
            html.AppendLine("</div>");
        }

        private void ConvertAdmonition(IAdmonition admonition, StringBuilder html, IConverterContext context)
        {
            var typeClass = admonition.Type.ToString().ToLower();
            var typeLabel = admonition.Type.ToString().ToUpper();
            
            html.AppendLine($"<div class=\"admonitionblock {typeClass}\">");
            html.AppendLine("<table>");
            html.AppendLine("<tr>");
            html.AppendLine($"<td class=\"icon\"><div class=\"title\">{typeLabel}</div></td>");
            html.AppendLine("<td class=\"content\">");
            
            if (!string.IsNullOrEmpty(admonition.Title))
            {
                html.AppendLine($"<div class=\"title\">{EscapeHtml(admonition.Title)}</div>");
            }
            
            html.AppendLine($"<div class=\"paragraph\"><p>{EscapeHtml(admonition.Content)}</p></div>");
            html.AppendLine("</td>");
            html.AppendLine("</tr>");
            html.AppendLine("</table>");
            html.AppendLine("</div>");
        }

        private void ConvertTable(ITable table, StringBuilder html, IConverterContext context)
        {
            html.AppendLine("<table class=\"tableblock frame-all grid-all\">");
            
            if (table.Header != null)
            {
                html.AppendLine("<thead>");
                ConvertTableHeader(table.Header, html, context);
                html.AppendLine("</thead>");
            }
            
            if (table.Rows.Any())
            {
                html.AppendLine("<tbody>");
                foreach (var row in table.Rows)
                {
                    ConvertTableRow(row, html, context);
                }
                html.AppendLine("</tbody>");
            }
            
            html.AppendLine("</table>");
        }

        private void ConvertTableOfContents(ITableOfContents tableOfContents, StringBuilder html, IConverterContext context)
        {
            html.AppendLine("<div class=\"toc\">");
            
            // Add title if present
            if (!string.IsNullOrEmpty(tableOfContents.Title))
            {
                html.AppendLine($"<div class=\"toc-title\">{EscapeHtml(tableOfContents.Title)}</div>");
            }
            
            // Generate nested navigation list
            if (tableOfContents.Entries.Any())
            {
                html.AppendLine("<ul>");
                
                foreach (var entry in tableOfContents.Entries)
                {
                    ConvertTableOfContentsEntry(entry, html, context);
                }
                
                html.AppendLine("</ul>");
            }
            
            html.AppendLine("</div>");
        }

        private void ConvertTableOfContentsEntry(ITableOfContentsEntry entry, StringBuilder html, IConverterContext context)
        {
            html.Append("<li>");
            
            // Create link to section
            html.Append($"<a href=\"#{EscapeHtml(entry.AnchorId)}\">{EscapeHtml(entry.Title)}</a>");
            
            // Add child entries if present
            if (entry.Children.Any())
            {
                html.AppendLine("<ul>");
                
                foreach (var child in entry.Children)
                {
                    ConvertTableOfContentsEntry(child, html, context);
                }
                
                html.AppendLine("</ul>");
            }
            
            html.AppendLine("</li>");
        }

        private void ConvertFootnote(IFootnote footnote, StringBuilder html, IConverterContext context)
        {
            if (footnote.IsReference)
            {
                // Footnote reference link
                html.Append($"<a href=\"#_footnote_{EscapeHtml(footnote.Id)}\" class=\"footnote\" title=\"{EscapeHtml(footnote.Text)}\">");
                html.Append($"<sup>{EscapeHtml(footnote.ReferenceLabel)}</sup>");
                html.Append("</a>");
            }
            else
            {
                // Inline footnote with popup or link
                html.Append($"<a href=\"#_footnotedef_{EscapeHtml(footnote.Id)}\" class=\"footnote\">");
                html.Append($"<sup>{EscapeHtml(footnote.ReferenceLabel)}</sup>");
                html.Append("</a>");
                
                // Add footnote definition to a collection for later rendering
                // In a full implementation, you'd collect footnotes and render them at the end
            }
        }

        private void ConvertTableHeader(ITableHeader header, StringBuilder html, IConverterContext context)
        {
            html.Append("<tr>");
            foreach (var cell in header.Cells)
            {
                ConvertTableCell(cell, html, context, true);
            }
            html.AppendLine("</tr>");
        }

        private void ConvertTableRow(ITableRow row, StringBuilder html, IConverterContext context)
        {
            html.Append("<tr>");
            foreach (var cell in row.Cells)
            {
                ConvertTableCell(cell, html, context, row.IsHeader);
            }
            html.AppendLine("</tr>");
        }

        private void ConvertTableCell(ITableCell cell, StringBuilder html, IConverterContext context, bool isHeader = false)
        {
            var tagName = isHeader || cell.IsHeader ? "th" : "td";
            html.Append($"<{tagName}");
            
            if (cell.ColSpan > 1)
            {
                html.Append($" colspan=\"{cell.ColSpan}\"");
            }
            
            if (cell.RowSpan > 1)
            {
                html.Append($" rowspan=\"{cell.RowSpan}\"");
            }
            
            if (!string.IsNullOrEmpty(cell.Alignment))
            {
                html.Append($" class=\"halign-{cell.Alignment}\"");
            }
            
            html.Append(">");
            html.Append(EscapeHtml(cell.Content));
            html.Append($"</{tagName}>");
        }

        private void ConvertGenericElement(IDocumentElement element, StringBuilder html, IConverterContext context)
        {
            switch (element)
            {
                case IText text:
                    html.Append(EscapeHtml(text.Content));
                    break;
                case IEmphasis emphasis:
                    html.Append("<em>");
                    html.Append(EscapeHtml(emphasis.Text));
                    html.Append("</em>");
                    break;
                case IStrong strong:
                    html.Append("<strong>");
                    html.Append(EscapeHtml(strong.Text));
                    html.Append("</strong>");
                    break;
                case IHighlight highlight:
                    html.Append("<mark>");
                    html.Append(EscapeHtml(highlight.Text));
                    html.Append("</mark>");
                    break;
                case ISuperscript superscript:
                    html.Append("<sup>");
                    html.Append(EscapeHtml(superscript.Text));
                    html.Append("</sup>");
                    break;
                case ISubscript subscript:
                    html.Append("<sub>");
                    html.Append(EscapeHtml(subscript.Text));
                    html.Append("</sub>");
                    break;
                case ILink link:
                    html.Append($"<a href=\"{EscapeHtml(link.Url)}\"");
                    if (!string.IsNullOrEmpty(link.Title))
                        html.Append($" title=\"{EscapeHtml(link.Title)}\"");
                    html.Append(">");
                    html.Append(EscapeHtml(link.Text));
                    html.Append("</a>");
                    break;
                case IImage image:
                    html.Append($"<img src=\"{EscapeHtml(image.Src)}\" alt=\"{EscapeHtml(image.Alt)}\"");
                    if (!string.IsNullOrEmpty(image.Title))
                        html.Append($" title=\"{EscapeHtml(image.Title)}\"");
                    html.Append("/>");
                    break;
                case IInlineCode inlineCode:
                    html.Append($"<code>{EscapeHtml(inlineCode.Content)}</code>");
                    break;
                case IAnchor anchor:
                    ConvertAnchor(anchor, html, context);
                    break;
                case ICrossReference crossReference:
                    ConvertCrossReference(crossReference, html, context);
                    break;
            }
        }

        private void ConvertAnchor(IAnchor anchor, StringBuilder html, IConverterContext context)
        {
            // Create an HTML anchor element with the specified ID
            html.Append($"<a id=\"{EscapeHtml(anchor.Id)}\"");
            
            // If there's a label, show it as the anchor text
            if (!string.IsNullOrEmpty(anchor.Label))
            {
                html.Append(">");
                html.Append(EscapeHtml(anchor.Label));
                html.Append("</a>");
            }
            else
            {
                // Self-closing anchor for invisible anchors
                html.Append("></a>");
            }
        }

        private void ConvertCrossReference(ICrossReference crossReference, StringBuilder html, IConverterContext context)
        {
            // Create an HTML link to the internal anchor
            html.Append($"<a href=\"#{EscapeHtml(crossReference.TargetId)}\"");
            
            // Add CSS class for cross-references
            html.Append(" class=\"xref\"");
            
            html.Append(">");
            
            // Use custom link text if provided, otherwise use the target ID
            var linkText = !string.IsNullOrEmpty(crossReference.LinkText) 
                ? crossReference.LinkText 
                : crossReference.TargetId;
            
            html.Append(EscapeHtml(linkText));
            html.Append("</a>");
        }

        private void ConvertImageMacro(IImageMacro imageMacro, StringBuilder html, IConverterContext context)
        {
            html.Append("<img");
            html.Append($" src=\"{EscapeHtml(imageMacro.Source)}\"");
            html.Append($" alt=\"{EscapeHtml(imageMacro.Alt)}\"");
            
            if (!string.IsNullOrEmpty(imageMacro.Title))
                html.Append($" title=\"{EscapeHtml(imageMacro.Title)}\"");
            
            if (imageMacro.Width.HasValue)
                html.Append($" width=\"{imageMacro.Width}\"");
            
            if (imageMacro.Height.HasValue)
                html.Append($" height=\"{imageMacro.Height}\"");
            
            if (!string.IsNullOrEmpty(imageMacro.Align))
                html.Append($" class=\"align-{EscapeHtml(imageMacro.Align)}\"");
            
            html.Append("/>");
            
            if (!string.IsNullOrEmpty(imageMacro.Link))
            {
                html.Insert(html.Length - imageMacro.Source.Length - 50, $"<a href=\"{EscapeHtml(imageMacro.Link)}\">");
                html.Append("</a>");
            }
        }

        private void ConvertVideoMacro(IVideoMacro videoMacro, StringBuilder html, IConverterContext context)
        {
            html.Append("<video");
            
            if (videoMacro.Width.HasValue)
                html.Append($" width=\"{videoMacro.Width}\"");
            
            if (videoMacro.Height.HasValue)
                html.Append($" height=\"{videoMacro.Height}\"");
            
            if (videoMacro.Controls)
                html.Append(" controls");
            
            if (videoMacro.AutoPlay)
                html.Append(" autoplay");
            
            if (videoMacro.Loop)
                html.Append(" loop");
            
            if (videoMacro.Muted)
                html.Append(" muted");
            
            if (!string.IsNullOrEmpty(videoMacro.Poster))
                html.Append($" poster=\"{EscapeHtml(videoMacro.Poster)}\"");
            
            html.Append(">");
            html.Append($"<source src=\"{EscapeHtml(videoMacro.Source)}\" type=\"video/{EscapeHtml(videoMacro.VideoFormat)}\">");
            html.Append("Your browser does not support the video tag.");
            html.Append("</video>");
            
            if (!string.IsNullOrEmpty(videoMacro.Title))
            {
                html.AppendLine();
                html.Append($"<div class=\"video-title\">{EscapeHtml(videoMacro.Title)}</div>");
            }
        }

        private void ConvertIncludeMacro(IIncludeMacro includeMacro, StringBuilder html, IConverterContext context)
        {
            // For now, just render a placeholder comment indicating the include
            html.Append($"<!-- Include: {EscapeHtml(includeMacro.FilePath)}");
            
            if (!string.IsNullOrEmpty(includeMacro.Lines))
                html.Append($" (lines: {EscapeHtml(includeMacro.Lines)})");
            
            if (!string.IsNullOrEmpty(includeMacro.Tags))
                html.Append($" (tags: {EscapeHtml(includeMacro.Tags)})");
            
            html.Append(" -->");
            
            // Note: Full include processing would require file system access and is a complex feature
            // This is a placeholder implementation for the macro system foundation
        }

        private void ConvertMacro(IMacro macro, StringBuilder html, IConverterContext context)
        {
            // Generic macro rendering - creates a span with macro information
            html.Append($"<span class=\"macro {EscapeHtml(macro.Name)}\"");
            html.Append($" data-macro=\"{EscapeHtml(macro.Name)}\"");
            html.Append($" data-target=\"{EscapeHtml(macro.Target)}\"");
            
            foreach (var param in macro.Parameters)
            {
                html.Append($" data-{EscapeHtml(param.Key)}=\"{EscapeHtml(param.Value)}\"");
            }
            
            html.Append(">");
            
            // Display macro in a readable format
            html.Append($"{EscapeHtml(macro.Name)}::{EscapeHtml(macro.Target)}[");
            
            var paramStrings = macro.Parameters.Select(p => $"{p.Key}={p.Value}").ToArray();
            if (paramStrings.Length > 0)
            {
                html.Append(EscapeHtml(string.Join(",", paramStrings)));
            }
            
            html.Append("]");
            html.Append("</span>");
        }

        private string WrapInHtmlDocument(string body, IDocument document, IConverterContext context)
        {
            if (context.Options.PrettyPrint)
            {
                return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""{context.Options.OutputEncoding}"">
    <title>{EscapeHtml(document.Header?.Title ?? "AsciiDoc Document")}</title>
    <style>
        body {{ font-family: serif; line-height: 1.6; margin: 2rem; }}
        .author, .revision {{ color: #666; font-style: italic; }}
        pre {{ background: #f4f4f4; padding: 1rem; overflow-x: auto; }}
        code {{ background: #f4f4f4; padding: 0.2rem; }}
        table.tableblock {{ border-collapse: collapse; width: 100%; margin: 1rem 0; }}
        table.tableblock th, table.tableblock td {{ border: 1px solid #ddd; padding: 0.5rem; text-align: left; }}
        table.tableblock th {{ background-color: #f5f5f5; font-weight: bold; }}
        .halign-center {{ text-align: center; }}
        .halign-right {{ text-align: right; }}
        blockquote {{ margin: 1rem 2rem; padding: 0.5rem 1rem; border-left: 4px solid #ddd; background-color: #f9f9f9; }}
        blockquote cite {{ display: block; margin-top: 0.5rem; font-style: italic; text-align: right; }}
        .admonitionblock {{ margin: 1rem 0; }}
        .admonitionblock table {{ border: 0; background: none; width: 100%; }}
        .admonitionblock td.icon {{ width: 80px; vertical-align: top; text-align: center; }}
        .admonitionblock td.content {{ padding-left: 1rem; }}
        .admonitionblock.note {{ background-color: #e7f4fd; border-left: 4px solid #2196f3; }}
        .admonitionblock.tip {{ background-color: #e8f5e8; border-left: 4px solid #4caf50; }}
        .admonitionblock.important {{ background-color: #fff8e1; border-left: 4px solid #ff9800; }}
        .admonitionblock.warning {{ background-color: #fff3e0; border-left: 4px solid #ff5722; }}
        .admonitionblock.caution {{ background-color: #ffebee; border-left: 4px solid #f44336; }}
        .admonitionblock .title {{ font-weight: bold; }}
        .xref {{ color: #2196f3; text-decoration: none; }}
        .xref:hover {{ text-decoration: underline; }}
        .align-left {{ text-align: left; }}
        .align-center {{ text-align: center; }}
        .align-right {{ text-align: right; }}
        .video-title {{ font-style: italic; margin-top: 0.5rem; text-align: center; }}
        .macro {{ display: inline-block; background-color: #f0f0f0; padding: 0.2rem 0.4rem; border-radius: 3px; font-family: monospace; font-size: 0.9em; }}
        video {{ max-width: 100%; height: auto; }}
        img {{ max-width: 100%; height: auto; }}
    </style>
</head>
<body>
{body}
</body>
</html>";
            }
            else
            {
                return $"<!DOCTYPE html><html><head><meta charset=\"{context.Options.OutputEncoding}\"><title>{EscapeHtml(document.Header?.Title ?? "AsciiDoc Document")}</title></head><body>{body}</body></html>";
            }
        }

        /// <summary>
        /// Estimates the output capacity needed for the HTML StringBuilder based on document structure.
        /// This helps prevent multiple StringBuilder resizing operations during conversion.
        /// </summary>
        /// <param name="document">The document to estimate capacity for.</param>
        /// <returns>Estimated capacity in characters.</returns>
        private static int EstimateOutputCapacity(IDocument document)
        {
            // Base capacity for document structure (DOCTYPE, html, head, body tags)
            var capacity = 1024;
            
            // Add capacity for header
            if (document.Header != null)
            {
                capacity += 200; // Basic header structure
                if (!string.IsNullOrEmpty(document.Header.Title))
                    capacity += document.Header.Title.Length * 2; // HTML tags add overhead
                if (!string.IsNullOrEmpty(document.Header.Author))
                    capacity += document.Header.Author.Length * 2;
                if (!string.IsNullOrEmpty(document.Header.Revision))
                    capacity += document.Header.Revision.Length * 2;
            }
            
            // Estimate capacity for elements (rough heuristic: 3x content size for HTML markup)
            capacity += document.Elements.Count * 150; // Base per-element overhead
            
            // For small documents, ensure minimum reasonable capacity
            return Math.Max(capacity, 4096);
        }

        private static string EscapeHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
    }
}