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
using AsciiDocSharp.Core;
using AsciiDocSharp.Core.Elements;

namespace AsciiDocSharp.Parser
{
    /// <summary>
    /// Interface for processing include directives in AsciiDoc documents.
    /// Handles file inclusion with circular reference detection and path resolution.
    /// </summary>
    public interface IIncludeProcessor
    {
        /// <summary>
        /// Processes an include directive and returns the parsed document elements.
        /// </summary>
        /// <param name="includeMacro">The include macro to process</param>
        /// <param name="basePath">The base path for resolving relative includes</param>
        /// <param name="includeStack">Stack of files being included to detect circular references</param>
        /// <returns>List of document elements from the included file</returns>
        IReadOnlyList<IDocumentElement> ProcessInclude(IIncludeMacro includeMacro, string basePath, IReadOnlyList<string> includeStack);

        /// <summary>
        /// Resolves the full path for an include file.
        /// </summary>
        /// <param name="filePath">The file path from the include directive</param>
        /// <param name="basePath">The base path for resolving relative paths</param>
        /// <returns>The resolved absolute file path</returns>
        string ResolveIncludePath(string filePath, string basePath);

        /// <summary>
        /// Checks if a file path would create a circular reference.
        /// </summary>
        /// <param name="filePath">The file path to check</param>
        /// <param name="includeStack">Current include stack</param>
        /// <returns>True if circular reference would be created</returns>
        bool WouldCreateCircularReference(string filePath, IReadOnlyList<string> includeStack);
    }
}