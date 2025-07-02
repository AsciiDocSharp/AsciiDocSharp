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

namespace AsciiDoc.NET.Core
{
    /// <summary>
    /// Represents an item in a description list, containing both a term and its description.
    /// Description list items are created from "term:: description" syntax in AsciiDoc.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// CPU:: The brain of the computer.
    /// ]]>
    /// In this example, "CPU" is the Term and "The brain of the computer." is the Description.
    /// </example>
    public interface IDescriptionListItem : IDocumentElement
    {
        /// <summary>
        /// Gets the term being defined in this description list item.
        /// This corresponds to the text before the "::" in AsciiDoc syntax.
        /// </summary>
        string Term { get; }

        /// <summary>
        /// Gets the description or definition of the term.
        /// This corresponds to the text after the "::" in AsciiDoc syntax.
        /// </summary>
        string Description { get; }
    }
}