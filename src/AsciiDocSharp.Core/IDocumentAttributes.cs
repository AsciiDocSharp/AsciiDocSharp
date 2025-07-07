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

using System;
using System.Collections.Generic;

namespace AsciiDocSharp.Core
{
    /// <summary>
    /// Provides attribute storage and retrieval for document elements.
    /// Attributes store element metadata like CSS classes, IDs, custom properties,
    /// and AsciiDoc-specific settings (roles, options, etc.).
    /// </summary>
    /// <example>
    /// <code>
    /// // Setting and getting attributes
    /// element.Attributes.SetAttribute("id", "my-section");
    /// element.Attributes.SetAttribute("role", "highlight");
    /// element.Attributes.SetAttribute("width", 500);
    /// 
    /// string id = element.Attributes.GetAttribute("id"); // "my-section"
    /// int width = element.Attributes.GetAttribute&lt;int&gt;("width"); // 500
    /// bool hasClass = element.Attributes.HasAttribute("class"); // false
    /// </code>
    /// </example>
    public interface IDocumentAttributes
    {
        /// <summary>
        /// Gets the value of the specified attribute as a string.
        /// Returns null if the attribute does not exist.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve. Cannot be null or empty.</param>
        /// <returns>The attribute value as a string, or null if not found.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
        string GetAttribute(string name);

        /// <summary>
        /// Gets the value of the specified attribute converted to the specified type.
        /// Returns the default value for the type if the attribute does not exist or cannot be converted.
        /// </summary>
        /// <typeparam name="T">The type to convert the attribute value to.</typeparam>
        /// <param name="name">The name of the attribute to retrieve. Cannot be null or empty.</param>
        /// <returns>The attribute value converted to type T, or default(T) if not found or conversion fails.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
        T GetAttribute<T>(string name);

        /// <summary>
        /// Determines whether an attribute with the specified name exists.
        /// </summary>
        /// <param name="name">The name of the attribute to check. Cannot be null or empty.</param>
        /// <returns>true if the attribute exists; otherwise, false.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
        bool HasAttribute(string name);

        /// <summary>
        /// Gets an enumerable collection of all attribute names currently stored.
        /// The order of names is not guaranteed.
        /// </summary>
        /// <value>An enumerable collection of attribute names.</value>
        IEnumerable<string> AttributeNames { get; }

        /// <summary>
        /// Sets the value of the specified attribute.
        /// If the attribute already exists, its value is updated.
        /// </summary>
        /// <param name="name">The name of the attribute to set. Cannot be null or empty.</param>
        /// <param name="value">The value to set. Can be null to effectively remove the attribute.</param>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
        void SetAttribute(string name, object value);

        /// <summary>
        /// Removes the specified attribute if it exists.
        /// If the attribute does not exist, no action is taken.
        /// </summary>
        /// <param name="name">The name of the attribute to remove. Cannot be null or empty.</param>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
        void RemoveAttribute(string name);
    }
}