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
using AsciiDocSharp.Parser;
using AsciiDocSharp.Parser.Implementation;
using AsciiDocSharp.Converters.Core;
using System;
using System.IO;

namespace AsciiDocSharp
{
    /// <summary>
    /// Provides a high-level interface for processing AsciiDoc documents through parsing and conversion.
    /// This class orchestrates the two-phase processing: parsing AsciiDoc into a document tree,
    /// then converting that tree to the desired output format using the visitor pattern.
    /// </summary>
    /// <remarks>
    /// The processor supports dependency injection for custom parsers and maintains separation
    /// between parsing logic and conversion logic. This allows for flexible combinations of
    /// parsers and converters without tight coupling.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic usage with default parser
    /// var processor = new AsciiDocProcessor();
    /// var document = processor.ParseFromText("= Hello World\n\nContent here.");
    /// var html = processor.ConvertDocument(document, new HtmlDocumentConverter());
    /// 
    /// // Advanced usage with custom parser and options
    /// var customParser = new AsciiDocParser();
    /// var processor = new AsciiDocProcessor(customParser);
    /// var options = new ParserOptions { IncludeDirectivesEnabled = false };
    /// var document = processor.ParseFromText(content, options);
    /// </code>
    /// </example>
    public class AsciiDocProcessor
    {
        private readonly IAsciiDocParser _parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsciiDocProcessor"/> class with the default parser.
        /// The default parser provides standard AsciiDoc processing capabilities.
        /// </summary>
        public AsciiDocProcessor() : this(new AsciiDocParser())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsciiDocProcessor"/> class with a custom parser.
        /// This constructor supports dependency injection scenarios and custom parser implementations.
        /// </summary>
        /// <param name="parser">The parser to use for processing AsciiDoc content. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="parser"/> is null.</exception>
        public AsciiDocProcessor(IAsciiDocParser parser)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        /// <summary>
        /// Parses AsciiDoc content from a string into a document tree using default parser options.
        /// </summary>
        /// <param name="asciiDocContent">The AsciiDoc content to parse. Cannot be null or empty.</param>
        /// <returns>A document tree representing the parsed AsciiDoc content.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="asciiDocContent"/> is null or empty.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the content cannot be parsed due to syntax errors.</exception>
        /// <example>
        /// <code>
        /// string content = @"
        /// = Document Title
        /// 
        /// This is a paragraph with *bold* text.
        /// 
        /// == Section
        /// 
        /// Here's a list:
        /// 
        /// * Item 1
        /// * Item 2
        /// ";
        /// 
        /// var document = processor.ParseFromText(content);
        /// Console.WriteLine($"Title: {document.Header?.Title}");
        /// Console.WriteLine($"Elements: {document.Elements.Count}");
        /// </code>
        /// </example>
        public IDocument ParseFromText(string asciiDocContent)
        {
            if (string.IsNullOrEmpty(asciiDocContent))
                throw new ArgumentException("Content cannot be null or empty", nameof(asciiDocContent));

            return _parser.Parse(asciiDocContent);
        }

        /// <summary>
        /// Parses AsciiDoc content from a string into a document tree using the specified parser options.
        /// </summary>
        /// <param name="asciiDocContent">The AsciiDoc content to parse. Cannot be null or empty.</param>
        /// <param name="options">The parser options to control parsing behavior. Cannot be null.</param>
        /// <returns>A document tree representing the parsed AsciiDoc content.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="asciiDocContent"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the content cannot be parsed due to syntax errors.</exception>
        public IDocument ParseFromText(string asciiDocContent, ParserOptions options)
        {
            if (string.IsNullOrEmpty(asciiDocContent))
                throw new ArgumentException("Content cannot be null or empty", nameof(asciiDocContent));

            return _parser.Parse(asciiDocContent, options);
        }

        /// <summary>
        /// Parses an AsciiDoc file into a document tree using default parser options.
        /// The file is read entirely into memory before parsing.
        /// </summary>
        /// <param name="filePath">The path to the AsciiDoc file to parse. Cannot be null or empty.</param>
        /// <returns>A document tree representing the parsed AsciiDoc file content.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the file content cannot be parsed due to syntax errors.</exception>
        public IDocument ParseFromFile(string filePath)
        {
            return _parser.ParseFile(filePath);
        }

        /// <summary>
        /// Parses an AsciiDoc file into a document tree using the specified parser options.
        /// </summary>
        /// <param name="filePath">The path to the AsciiDoc file to parse. Cannot be null or empty.</param>
        /// <param name="options">The parser options to control parsing behavior. Cannot be null.</param>
        /// <returns>A document tree representing the parsed AsciiDoc file content.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the file content cannot be parsed due to syntax errors.</exception>
        public IDocument ParseFromFile(string filePath, ParserOptions options)
        {
            return _parser.ParseFile(filePath, options);
        }

        /// <summary>
        /// Converts a document tree to the specified output format using a converter with default options.
        /// This method implements the visitor pattern to traverse and convert the document tree.
        /// </summary>
        /// <typeparam name="T">The output type produced by the converter (e.g., string for HTML).</typeparam>
        /// <param name="document">The document tree to convert. Cannot be null.</param>
        /// <param name="converter">The converter to use for transformation. Cannot be null.</param>
        /// <returns>The converted document in the format specified by the converter.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="document"/> or <paramref name="converter"/> is null.</exception>
        /// <example>
        /// <code>
        /// var document = processor.ParseFromText(asciiDocContent);
        /// var htmlConverter = new HtmlDocumentConverter();
        /// string html = processor.ConvertDocument(document, htmlConverter);
        /// </code>
        /// </example>
        public T ConvertDocument<T>(IDocument document, IDocumentConverter<T> converter)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            return converter.Convert(document);
        }

        /// <summary>
        /// Converts a document tree to the specified output format using a converter with custom options.
        /// </summary>
        /// <typeparam name="T">The output type produced by the converter (e.g., string for HTML).</typeparam>
        /// <param name="document">The document tree to convert. Cannot be null.</param>
        /// <param name="converter">The converter to use for transformation. Cannot be null.</param>
        /// <param name="options">The converter options to customize output. Cannot be null.</param>
        /// <returns>The converted document in the format specified by the converter.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when any parameter is null.</exception>
        public T ConvertDocument<T>(IDocument document, IDocumentConverter<T> converter, IConverterOptions options)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return converter.Convert(document, options);
        }

        /// <summary>
        /// Parses AsciiDoc content and immediately converts it to the specified output format.
        /// This is a convenience method combining parsing and conversion in a single operation.
        /// </summary>
        /// <typeparam name="T">The output type produced by the converter (e.g., string for HTML).</typeparam>
        /// <param name="asciiDocContent">The AsciiDoc content to process. Cannot be null or empty.</param>
        /// <param name="converter">The converter to use for transformation. Cannot be null.</param>
        /// <returns>The converted document in the format specified by the converter.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="asciiDocContent"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="converter"/> is null.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the content cannot be parsed.</exception>
        public T ProcessText<T>(string asciiDocContent, IDocumentConverter<T> converter)
        {
            var document = ParseFromText(asciiDocContent);
            return ConvertDocument(document, converter);
        }

        /// <summary>
        /// Parses AsciiDoc content and immediately converts it to the specified output format with custom options.
        /// </summary>
        /// <typeparam name="T">The output type produced by the converter (e.g., string for HTML).</typeparam>
        /// <param name="asciiDocContent">The AsciiDoc content to process. Cannot be null or empty.</param>
        /// <param name="converter">The converter to use for transformation. Cannot be null.</param>
        /// <param name="options">The converter options to customize output. Cannot be null.</param>
        /// <returns>The converted document in the format specified by the converter.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="asciiDocContent"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="converter"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the content cannot be parsed.</exception>
        public T ProcessText<T>(string asciiDocContent, IDocumentConverter<T> converter, IConverterOptions options)
        {
            var document = ParseFromText(asciiDocContent);
            return ConvertDocument(document, converter, options);
        }

        /// <summary>
        /// Parses an AsciiDoc file and immediately converts it to the specified output format.
        /// This is a convenience method combining file parsing and conversion in a single operation.
        /// </summary>
        /// <typeparam name="T">The output type produced by the converter (e.g., string for HTML).</typeparam>
        /// <param name="filePath">The path to the AsciiDoc file to process. Cannot be null or empty.</param>
        /// <param name="converter">The converter to use for transformation. Cannot be null.</param>
        /// <returns>The converted document in the format specified by the converter.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="converter"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the file content cannot be parsed.</exception>
        public T ProcessFile<T>(string filePath, IDocumentConverter<T> converter)
        {
            var document = ParseFromFile(filePath);
            return ConvertDocument(document, converter);
        }

        /// <summary>
        /// Parses an AsciiDoc file and immediately converts it to the specified output format with custom options.
        /// </summary>
        /// <typeparam name="T">The output type produced by the converter (e.g., string for HTML).</typeparam>
        /// <param name="filePath">The path to the AsciiDoc file to process. Cannot be null or empty.</param>
        /// <param name="converter">The converter to use for transformation. Cannot be null.</param>
        /// <param name="options">The converter options to customize output. Cannot be null.</param>
        /// <returns>The converted document in the format specified by the converter.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="converter"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="AsciiDocSharp.Parser.ParseException">Thrown when the file content cannot be parsed.</exception>
        public T ProcessFile<T>(string filePath, IDocumentConverter<T> converter, IConverterOptions options)
        {
            var document = ParseFromFile(filePath);
            return ConvertDocument(document, converter, options);
        }
    }
}