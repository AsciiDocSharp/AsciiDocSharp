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

namespace AsciiDoc.NET.Core.Elements
{
    /// <summary>
    /// Represents a passthrough block in an AsciiDoc document.
    /// Passthrough blocks allow raw content to bypass normal AsciiDoc substitutions
    /// and pass directly to the output format.
    /// </summary>
    /// <remarks>
    /// Passthrough blocks are useful for inserting format-specific content (like raw HTML)
    /// or content that should not be processed by AsciiDoc's normal text substitutions.
    /// They can be created using delimited blocks with four plus signs (++++),
    /// or by using the [pass] style with paragraph or open block syntax.
    /// By default, content in passthrough blocks is excluded from substitutions.
    /// </remarks>
    /// <example>
    /// <para>AsciiDoc syntax for delimited passthrough block:</para>
    /// <code>
    /// ++++
    /// &lt;video poster="images/movie-reel.png"&gt;
    ///   &lt;source src="videos/writing-zen.webm" type="video/webm"&gt;
    /// &lt;/video&gt;
    /// ++++
    /// </code>
    /// <para>AsciiDoc syntax for [pass] style:</para>
    /// <code>
    /// [pass]
    /// &lt;del&gt;strike this&lt;/del&gt; is marked as deleted.
    /// </code>
    /// </example>
    public interface IPassthrough : IDocumentElement
    {
        /// <summary>
        /// Gets the raw content of the passthrough block.
        /// This content is passed directly to the output without normal AsciiDoc processing.
        /// </summary>
        /// <value>The raw content string.</value>
        string Content { get; }

        /// <summary>
        /// Gets the optional title of the passthrough block.
        /// Titles are specified using the dot notation before the passthrough block delimiter.
        /// </summary>
        /// <value>The passthrough block title, or null if no title is specified.</value>
        string Title { get; }

        /// <summary>
        /// Gets the substitution options for the passthrough block.
        /// By default, passthrough blocks exclude substitutions, but this can be controlled
        /// using the subs attribute.
        /// </summary>
        /// <value>The substitution options, or null for default behavior (no substitutions).</value>
        string Substitutions { get; }
    }
}