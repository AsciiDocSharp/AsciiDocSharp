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

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AsciiDoc.Net.Core.Implementation
{
    public class DocumentAttributes : IDocumentAttributes
    {
        private readonly ConcurrentDictionary<string, object> _attributes;

        public DocumentAttributes()
        {
            _attributes = new ConcurrentDictionary<string, object>();
        }

        public DocumentAttributes(IDictionary<string, object> attributes)
        {
            _attributes = new ConcurrentDictionary<string, object>(attributes);
        }

        public string GetAttribute(string name)
        {
            return GetAttribute<string>(name);
        }

        public T GetAttribute<T>(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (_attributes.TryGetValue(name, out var value))
            {
                if (value is T directValue)
                    return directValue;

                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default(T);
                }
            }

            return default(T);
        }

        public bool HasAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return _attributes.ContainsKey(name);
        }

        public IEnumerable<string> AttributeNames => _attributes.Keys;

        public void SetAttribute(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _attributes[name] = value;
        }

        public void RemoveAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _attributes.TryRemove(name, out _);
        }
    }
}