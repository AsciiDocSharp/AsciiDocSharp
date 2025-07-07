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

namespace AsciiDoc.Net.Parser
{
    /// <summary>
    /// Represents a lexical token in the AsciiDoc parsing process.
    /// Tokens are the fundamental units produced by the tokenizer and consumed by the parser
    /// to build the document tree structure.
    /// </summary>
    /// <remarks>
    /// Tokens maintain position information for error reporting and provide the raw text
    /// value for further processing by the parser. The tokenization phase handles the
    /// first level of AsciiDoc syntax recognition.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example token usage during parsing
    /// while (tokenizer.HasMoreTokens)
    /// {
    ///     IToken token = tokenizer.NextToken();
    ///     Console.WriteLine($"{token.Type}: '{token.Value}' at line {token.Line}");
    ///     
    ///     if (token.Type == TokenType.Header)
    ///     {
    ///         // Process header token
    ///         var section = CreateSection(token.Value);
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IToken
    {
        /// <summary>
        /// Gets the type of this token, indicating its syntactic role in the AsciiDoc structure.
        /// The token type determines how the parser will interpret and process the token.
        /// </summary>
        /// <value>The token type from the <see cref="TokenType"/> enumeration.</value>
        TokenType Type { get; }

        /// <summary>
        /// Gets the raw text value of this token as it appears in the source.
        /// This includes any formatting characters and whitespace that may be significant for parsing.
        /// </summary>
        /// <value>The raw token text from the source document.</value>
        string Value { get; }

        /// <summary>
        /// Gets the line number where this token appears in the source document (1-based).
        /// Used for error reporting and debugging purposes.
        /// </summary>
        /// <value>The line number, starting from 1.</value>
        int Line { get; }

        /// <summary>
        /// Gets the column position where this token starts on its line (1-based).
        /// Used for precise error location reporting.
        /// </summary>
        /// <value>The column position, starting from 1.</value>
        int Column { get; }

        /// <summary>
        /// Gets the absolute character position of this token in the source document (0-based).
        /// Useful for calculating token spans and extracting source text ranges.
        /// </summary>
        /// <value>The absolute character position, starting from 0.</value>
        int Position { get; }

        /// <summary>
        /// Gets the length of this token in characters.
        /// Combined with Position, this defines the exact span of the token in the source.
        /// </summary>
        /// <value>The token length in characters.</value>
        int Length { get; }
    }

    /// <summary>
    /// Defines the types of lexical tokens recognized during AsciiDoc tokenization.
    /// These token types correspond to different AsciiDoc syntax elements and guide
    /// the parser in constructing the appropriate document tree structure.
    /// </summary>
    /// <remarks>
    /// Token types are organized by their syntactic role: structural elements (headers, lists),
    /// content types (text, code), formatting (emphasis, strong), and special constructs
    /// (macros, links, tables). The tokenizer assigns these types based on pattern matching.
    /// </remarks>
    public enum TokenType
    {
        /// <summary>Indicates the end of the input stream.</summary>
        EndOfFile,
        
        /// <summary>A line break or newline character sequence.</summary>
        NewLine,
        
        /// <summary>Plain text content without special formatting.</summary>
        Text,
        
        /// <summary>A section header (= Title, == Subtitle, etc.).</summary>
        Header,
        
        /// <summary>A list item marker and content (* item, 1. item, etc.).</summary>
        ListItem,
        
        /// <summary>A description list item (term:: description).</summary>
        DescriptionListItem,
        
        /// <summary>An empty line used for paragraph separation.</summary>
        EmptyLine,
        
        /// <summary>Code block delimiters (----, ````, etc.).</summary>
        CodeBlockDelimiter,
        
        /// <summary>Content within a code block (literal text).</summary>
        CodeContent,
        
        /// <summary>Emphasis markup (_text_ or __text__).</summary>
        Emphasis,
        
        /// <summary>Strong/bold markup (*text* or **text**).</summary>
        Strong,
        
        /// <summary>Highlight markup (#text#).</summary>
        Highlight,
        
        /// <summary>Superscript markup (^text^).</summary>
        Superscript,
        
        /// <summary>Subscript markup (~text~).</summary>
        Subscript,
        
        /// <summary>Inline code markup (`code`).</summary>
        InlineCode,
        
        /// <summary>Link markup (https://example.com[text]).</summary>
        Link,
        
        /// <summary>Image markup (image::path[alt]).</summary>
        Image,
        
        /// <summary>Table delimiters and structure (|===).</summary>
        TableDelimiter,
        
        /// <summary>A row of table data (|cell|cell|).</summary>
        TableRow,
        
        /// <summary>Block quote delimiters (____) and content.</summary>
        BlockQuoteDelimiter,
        
        /// <summary>Sidebar delimiters (****) and content.</summary>
        SidebarDelimiter,
        
        /// <summary>Example block delimiters (====) and content.</summary>
        ExampleDelimiter,
        
        /// <summary>Attribute definition lines (:attr: value).</summary>
        AttributeLine,
        
        /// <summary>Attribute block content ([role="value"]).</summary>
        AttributeBlockLine,
        
        /// <summary>Admonition blocks (NOTE:, WARNING:, etc.).</summary>
        AdmonitionBlock,
        
        /// <summary>Anchor definitions ([[anchor-id]]).</summary>
        Anchor,
        
        /// <summary>Cross-reference links (&lt;&lt;anchor-id&gt;&gt;).</summary>
        CrossReference,
        
        /// <summary>Block-level macro invocations (include::file[]).</summary>
        BlockMacro,
        
        /// <summary>Inline macro invocations (kbd:[Ctrl+C]).</summary>
        InlineMacro,
        
        /// <summary>Table of contents macro (toc::[]).</summary>
        TableOfContents,
        
        /// <summary>Footnote macro (footnote:[text] or footnoteref:[id,text]).</summary>
        Footnote,
        
        /// <summary>Unrecognized or malformed input (used for error recovery).</summary>
        Unknown
    }
}