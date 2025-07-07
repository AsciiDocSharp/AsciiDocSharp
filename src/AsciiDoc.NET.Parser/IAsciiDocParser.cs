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

using AsciiDoc.Net.Core;

namespace AsciiDoc.Net.Parser
{
    /// <summary>
    /// Defines the contract for parsing AsciiDoc content into a structured document tree.
    /// This interface represents the first phase of the two-phase processing architecture:
    /// tokenization followed by abstract syntax tree (AST) construction.
    /// </summary>
    /// <remarks>
    /// The parser implementation follows a recursive descent approach with lookahead for
    /// complex constructs. It supports incremental parsing, error recovery, and include
    /// directive processing with circular reference detection.
    /// </remarks>
    /// <example>
    /// <code>
    /// var parser = new AsciiDocParser();
    /// 
    /// // Parse from string
    /// string content = "= Document Title\n\nThis is content.";
    /// IDocument document = parser.Parse(content);
    /// 
    /// // Parse from file with options
    /// var options = new ParserOptions { IncludeDirectivesEnabled = true };
    /// IDocument document = parser.ParseFile("document.adoc", options);
    /// 
    /// // Parse individual element
    /// IDocumentElement element = parser.ParseElement("*bold text*");
    /// </code>
    /// </example>
    public interface IAsciiDocParser
    {
        /// <summary>
        /// Parses AsciiDoc content from a string into a complete document tree using default options.
        /// This method performs full document parsing including header extraction and element processing.
        /// </summary>
        /// <param name="input">The AsciiDoc content to parse. Cannot be null.</param>
        /// <returns>A complete document tree with header and body elements.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
        /// <exception cref="ParseException">Thrown when the input contains unrecoverable syntax errors.</exception>
        /// <example>
        /// <code>
        /// string asciiDoc = @"
        /// = My Document
        /// Author Name &lt;author@example.com&gt;
        /// v1.0, 2024-01-15
        /// 
        /// == Introduction
        /// 
        /// This is the introduction paragraph.
        /// ";
        /// 
        /// IDocument document = parser.Parse(asciiDoc);
        /// Console.WriteLine(document.Header.Title); // "My Document"
        /// Console.WriteLine(document.Elements.Count); // 1 (section)
        /// </code>
        /// </example>
        IDocument Parse(string input);

        /// <summary>
        /// Parses AsciiDoc content from a string into a document tree using the specified parser options.
        /// Options control parsing behavior such as include processing, attribute handling, and error recovery.
        /// </summary>
        /// <param name="input">The AsciiDoc content to parse. Cannot be null.</param>
        /// <param name="options">The parser options to control parsing behavior. Cannot be null.</param>
        /// <returns>A complete document tree with header and body elements.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="input"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ParseException">Thrown when the input contains unrecoverable syntax errors.</exception>
        IDocument Parse(string input, ParserOptions options);

        /// <summary>
        /// Parses an AsciiDoc file into a document tree using default options.
        /// The file is read entirely into memory and then parsed. Include directives are processed relative to the file's directory.
        /// </summary>
        /// <param name="filePath">The path to the AsciiDoc file to parse. Cannot be null or empty.</param>
        /// <returns>A complete document tree with header and body elements.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
        /// <exception cref="ParseException">Thrown when the file content contains unrecoverable syntax errors.</exception>
        /// <example>
        /// <code>
        /// // Parse a file with potential includes
        /// IDocument document = parser.ParseFile("/docs/manual.adoc");
        /// 
        /// // The parser will resolve include directives like:
        /// // include::chapters/introduction.adoc[]
        /// // relative to the manual.adoc file location
        /// </code>
        /// </example>
        IDocument ParseFile(string filePath);

        /// <summary>
        /// Parses an AsciiDoc file into a document tree using the specified parser options.
        /// </summary>
        /// <param name="filePath">The path to the AsciiDoc file to parse. Cannot be null or empty.</param>
        /// <param name="options">The parser options to control parsing behavior. Cannot be null.</param>
        /// <returns>A complete document tree with header and body elements.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
        /// <exception cref="ParseException">Thrown when the file content contains unrecoverable syntax errors.</exception>
        IDocument ParseFile(string filePath, ParserOptions options);

        /// <summary>
        /// Parses a single AsciiDoc element from a string, useful for parsing fragments or inline content.
        /// This method does not create a full document structure but returns the parsed element directly.
        /// </summary>
        /// <param name="input">The AsciiDoc element content to parse. Cannot be null.</param>
        /// <returns>The parsed document element, or null if the input is empty or contains only whitespace.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
        /// <exception cref="ParseException">Thrown when the input contains unrecoverable syntax errors.</exception>
        /// <example>
        /// <code>
        /// // Parse inline formatting
        /// IDocumentElement strong = parser.ParseElement("*bold text*");
        /// 
        /// // Parse a list item
        /// IDocumentElement listItem = parser.ParseElement("* First item");
        /// 
        /// // Parse a paragraph
        /// IDocumentElement paragraph = parser.ParseElement("This is a paragraph with a https://example.com[link].");
        /// </code>
        /// </example>
        IDocumentElement ParseElement(string input);

        /// <summary>
        /// Parses a single document element using an existing parse context.
        /// This method is primarily used internally for recursive parsing and custom parsing scenarios.
        /// </summary>
        /// <param name="context">The parse context containing tokenizer state and parsing environment. Cannot be null.</param>
        /// <returns>The parsed document element, or null if no valid element can be parsed from the current context state.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
        /// <exception cref="ParseException">Thrown when the context contains unrecoverable syntax errors.</exception>
        /// <remarks>
        /// This method is typically used by custom parsers or when implementing advanced parsing scenarios
        /// that require fine-grained control over the parsing context and token stream.
        /// </remarks>
        IDocumentElement ParseElement(IParseContext context);
    }
}