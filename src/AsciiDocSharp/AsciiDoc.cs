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

using AsciiDocSharp.Converters.Core;
using AsciiDocSharp.Converters.Html;

namespace AsciiDocSharp
{
    /// <summary>
    /// Provides convenient static methods for common AsciiDoc processing operations.
    /// This is the primary entry point for simple AsciiDoc-to-HTML conversions without
    /// requiring manual setup of parsers and converters.
    /// </summary>
    /// <remarks>
    /// This class uses shared static instances for optimal performance in scenarios where
    /// custom configuration is not required. For advanced scenarios with custom parsers,
    /// converters, or configuration, use <see cref="AsciiDocProcessor"/> directly.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Simple text-to-HTML conversion
    /// string html = AsciiDoc.ToHtml("= Hello World\n\nThis is a paragraph.");
    /// 
    /// // File-to-HTML conversion
    /// string html = AsciiDoc.FileToHtml("document.adoc");
    /// 
    /// // With custom converter options
    /// var options = new HtmlConverterOptions { IncludeDoctype = false };
    /// string html = AsciiDoc.ToHtml(content, options);
    /// </code>
    /// </example>
    public static class AsciiDoc
    {
        private static readonly AsciiDocProcessor _processor = new AsciiDocProcessor();
        private static readonly HtmlDocumentConverter _htmlConverter = new HtmlDocumentConverter();

        /// <summary>
        /// Converts AsciiDoc content to HTML using default settings.
        /// This is the most common method for simple conversions and returns a complete HTML document.
        /// </summary>
        /// <param name="asciiDocContent">The AsciiDoc content to convert. Cannot be null or empty.</param>
        /// <returns>The converted HTML content as a complete HTML document.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="asciiDocContent"/> is null or empty.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the AsciiDoc content cannot be parsed.</exception>
        /// <example>
        /// <code>
        /// string asciiDoc = @"
        /// = My Document
        /// 
        /// This is a *bold* paragraph with a https://example.com[link].
        /// ";
        /// string html = AsciiDoc.ToHtml(asciiDoc);
        /// </code>
        /// </example>
        public static string ToHtml(string asciiDocContent)
        {
            var options = new ConverterOptions { OutputFullDocument = true };
            return _processor.ProcessText(asciiDocContent, _htmlConverter, options);
        }

        /// <summary>
        /// Converts AsciiDoc content to HTML using the specified converter options.
        /// </summary>
        /// <param name="asciiDocContent">The AsciiDoc content to convert. Cannot be null or empty.</param>
        /// <param name="options">The converter options to customize output. Cannot be null.</param>
        /// <returns>The converted HTML content as a string.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="asciiDocContent"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the AsciiDoc content cannot be parsed.</exception>
        public static string ToHtml(string asciiDocContent, IConverterOptions options)
        {
            return _processor.ProcessText(asciiDocContent, _htmlConverter, options);
        }

        /// <summary>
        /// Converts an AsciiDoc file to HTML using default settings.
        /// The file is read and processed in its entirety and returns a complete HTML document.
        /// </summary>
        /// <param name="filePath">The path to the AsciiDoc file to convert. Cannot be null or empty.</param>
        /// <returns>The converted HTML content as a complete HTML document.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the AsciiDoc content cannot be parsed.</exception>
        /// <example>
        /// <code>
        /// string html = AsciiDoc.FileToHtml("documentation.adoc");
        /// File.WriteAllText("documentation.html", html);
        /// </code>
        /// </example>
        public static string FileToHtml(string filePath)
        {
            var options = new ConverterOptions { OutputFullDocument = true };
            return _processor.ProcessFile(filePath, _htmlConverter, options);
        }

        /// <summary>
        /// Converts an AsciiDoc file to HTML using the specified converter options.
        /// </summary>
        /// <param name="filePath">The path to the AsciiDoc file to convert. Cannot be null or empty.</param>
        /// <param name="options">The converter options to customize output. Cannot be null.</param>
        /// <returns>The converted HTML content as a string.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the AsciiDoc content cannot be parsed.</exception>
        public static string FileToHtml(string filePath, IConverterOptions options)
        {
            return _processor.ProcessFile(filePath, _htmlConverter, options);
        }

        /// <summary>
        /// Creates a new AsciiDoc processor instance for advanced scenarios.
        /// Use this when you need custom parser configuration or multiple processors.
        /// </summary>
        /// <returns>A new <see cref="AsciiDocProcessor"/> instance.</returns>
        /// <example>
        /// <code>
        /// var processor = AsciiDoc.CreateProcessor();
        /// var document = processor.ParseFromText(content);
        /// var html = processor.ConvertDocument(document, htmlConverter);
        /// </code>
        /// </example>
        public static AsciiDocProcessor CreateProcessor()
        {
            return new AsciiDocProcessor();
        }

        /// <summary>
        /// Creates a new HTML converter instance for advanced scenarios.
        /// Use this when you need custom converter configuration or multiple converters.
        /// </summary>
        /// <returns>A new <see cref="HtmlDocumentConverter"/> instance.</returns>
        /// <example>
        /// <code>
        /// var converter = AsciiDoc.CreateHtmlConverter();
        /// var options = new HtmlConverterOptions { IncludeDoctype = false };
        /// string html = converter.Convert(document, options);
        /// </code>
        /// </example>
        public static HtmlDocumentConverter CreateHtmlConverter()
        {
            return new HtmlDocumentConverter();
        }
    }
}