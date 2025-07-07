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

namespace AsciiDocSharp.Converters.Core
{
    /// <summary>
    /// Defines the contract for converting AsciiDoc document trees to specific output formats.
    /// This interface implements the Visitor pattern to traverse and transform document elements
    /// into the target format without modifying the original document structure.
    /// </summary>
    /// <typeparam name="T">The output type produced by the converter (e.g., string for text formats, byte[] for binary formats).</typeparam>
    /// <remarks>
    /// Converters are the second phase of the AsciiDocSharp processing pipeline. They receive
    /// a parsed document tree and transform it into the desired output format. The converter
    /// system is designed to be extensible, allowing custom output formats to be implemented
    /// by creating new converter classes.
    /// </remarks>
    /// <example>
    /// <code>
    /// // HTML converter example
    /// public class HtmlDocumentConverter : IDocumentConverter&lt;string&gt;
    /// {
    ///     public string Convert(IDocument document)
    ///     {
    ///         var html = new StringBuilder();
    ///         document.Accept(this);
    ///         return html.ToString();
    ///     }
    /// }
    /// 
    /// // Usage
    /// var converter = new HtmlDocumentConverter();
    /// string htmlOutput = converter.Convert(document);
    /// </code>
    /// </example>
    public interface IDocumentConverter<T>
    {
        /// <summary>
        /// Converts a complete AsciiDoc document to the target format using default conversion options.
        /// This method traverses the entire document tree and produces the output representation.
        /// </summary>
        /// <param name="document">The parsed AsciiDoc document to convert. Cannot be null.</param>
        /// <returns>The converted document in the target format.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        /// <exception cref="ConverterException">Thrown when the conversion process encounters an unrecoverable error.</exception>
        /// <example>
        /// <code>
        /// // Convert document to HTML with default settings
        /// var htmlConverter = new HtmlDocumentConverter();
        /// string html = htmlConverter.Convert(document);
        /// 
        /// // Convert document to Markdown with default settings
        /// var markdownConverter = new MarkdownDocumentConverter();
        /// string markdown = markdownConverter.Convert(document);
        /// </code>
        /// </example>
        T Convert(IDocument document);

        /// <summary>
        /// Converts a complete AsciiDoc document to the target format using the specified conversion options.
        /// Options allow customization of the output format, styling, and converter behavior.
        /// </summary>
        /// <param name="document">The parsed AsciiDoc document to convert. Cannot be null.</param>
        /// <param name="options">The conversion options to customize the output. Cannot be null.</param>
        /// <returns>The converted document in the target format with applied customizations.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="document"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ConverterException">Thrown when the conversion process encounters an unrecoverable error.</exception>
        /// <example>
        /// <code>
        /// // Convert to HTML with custom options
        /// var options = new HtmlConverterOptions
        /// {
        ///     PrettyPrint = true,
        ///     IncludeDoctype = false,
        ///     CustomCssClasses = new Dictionary&lt;string, string&gt;
        ///     {
        ///         { "admonition", "callout-box" }
        ///     }
        /// };
        /// 
        /// var htmlConverter = new HtmlDocumentConverter();
        /// string html = htmlConverter.Convert(document, options);
        /// </code>
        /// </example>
        T Convert(IDocument document, IConverterOptions options);
    }
}