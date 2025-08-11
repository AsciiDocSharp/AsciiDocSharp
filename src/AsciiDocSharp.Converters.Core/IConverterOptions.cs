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

using System.Collections.Generic;

namespace AsciiDocSharp.Converters.Core
{
    /// <summary>
    /// Defines the base contract for converter configuration options.
    /// This interface provides common settings that apply to all converter types,
    /// while specific converters can extend this with format-specific options.
    /// </summary>
    /// <remarks>
    /// Converter options allow fine-grained control over the output generation process.
    /// They support both standard settings (encoding, formatting) and extensibility
    /// through custom properties for converter-specific features.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Creating custom converter options
    /// public class HtmlConverterOptions : IConverterOptions
    /// {
    ///     public string OutputEncoding { get; set; } = "UTF-8";
    ///     public bool PrettyPrint { get; set; } = true;
    ///     public IDictionary&lt;string, object&gt; CustomProperties { get; } = new Dictionary&lt;string, object&gt;();
    ///     
    ///     // HTML-specific options
    ///     public bool IncludeDoctype { get; set; } = true;
    ///     public string CssFramework { get; set; } = "default";
    /// }
    /// 
    /// // Usage
    /// var options = new HtmlConverterOptions
    /// {
    ///     PrettyPrint = false,
    ///     IncludeDoctype = false
    /// };
    /// options.CustomProperties["syntax-highlighter"] = "prism";
    /// </code>
    /// </example>
    public interface IConverterOptions
    {
        /// <summary>
        /// Gets or sets the output text encoding for the converted content.
        /// This setting affects how the output is encoded when written to files or streams.
        /// </summary>
        /// <value>
        /// The encoding name (e.g., "UTF-8", "ASCII", "UTF-16"). 
        /// Default implementations typically use "UTF-8".
        /// </value>
        /// <example>
        /// <code>
        /// var options = new HtmlConverterOptions
        /// {
        ///     OutputEncoding = "UTF-8"  // Ensures proper Unicode support
        /// };
        /// </code>
        /// </example>
        string OutputEncoding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the output should be formatted for human readability.
        /// When enabled, converters add indentation, line breaks, and spacing to make the output more readable.
        /// When disabled, converters produce more compact output optimized for size.
        /// </summary>
        /// <value>
        /// <c>true</c> to enable pretty printing with formatting and indentation;
        /// <c>false</c> to produce compact output. Default varies by converter implementation.
        /// </value>
        /// <example>
        /// <code>
        /// // Pretty printed HTML (readable)
        /// var prettyOptions = new HtmlConverterOptions { PrettyPrint = true };
        /// // Output: 
        /// // &lt;div class="paragraph"&gt;
        /// //   &lt;p&gt;Content here&lt;/p&gt;
        /// // &lt;/div&gt;
        /// 
        /// // Compact HTML (smaller size)
        /// var compactOptions = new HtmlConverterOptions { PrettyPrint = false };
        /// // Output: &lt;div class="paragraph"&gt;&lt;p&gt;Content here&lt;/p&gt;&lt;/div&gt;
        /// </code>
        /// </example>
        bool PrettyPrint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to output a complete HTML document or just the content body.
        /// When true, the output includes DOCTYPE, html, head, and body tags with basic styling.
        /// When false, only the converted document content is returned without HTML document wrapper.
        /// </summary>
        /// <value>
        /// <c>false</c> to produce flat HTML content suitable for embedding in web applications;
        /// <c>true</c> to produce a complete standalone HTML document. Default is false.
        /// </value>
        /// <example>
        /// <code>
        /// // Flat output
        /// var flatOptions = new ConverterOptions { OutputFullDocument = false };
        /// // Output: &lt;h1&gt;Title&lt;/h1&gt;&lt;p&gt;Content&lt;/p&gt;
        /// 
        /// // Full document output
        /// var fullOptions = new ConverterOptions { OutputFullDocument = true };
        /// // Output: &lt;!DOCTYPE html&gt;&lt;html&gt;&lt;head&gt;...&lt;/head&gt;&lt;body&gt;...&lt;/body&gt;&lt;/html&gt;
        /// </code>
        /// </example>
        bool OutputFullDocument { get; set; }

        /// <summary>
        /// Gets a dictionary of custom properties for converter-specific configuration.
        /// This extensibility mechanism allows converters to support additional settings
        /// without breaking the base interface contract.
        /// </summary>
        /// <value>
        /// A dictionary mapping property names to their values. Property interpretation
        /// is specific to each converter implementation. Never returns null.
        /// </value>
        /// <example>
        /// <code>
        /// var options = new HtmlConverterOptions();
        /// 
        /// // Configure syntax highlighting
        /// options.CustomProperties["syntax-highlighter"] = "prism";
        /// options.CustomProperties["highlight-theme"] = "dark";
        /// 
        /// // Configure table styling
        /// options.CustomProperties["table-framework"] = "bootstrap";
        /// options.CustomProperties["table-classes"] = "table table-striped";
        /// 
        /// // Configure custom CSS
        /// options.CustomProperties["custom-css-file"] = "/assets/custom.css";
        /// options.CustomProperties["inline-css"] = true;
        /// </code>
        /// </example>
        IDictionary<string, object> CustomProperties { get; }
    }
}