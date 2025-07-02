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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AsciiDoc.NET.Core.Implementation
{
    public abstract class DocumentElementBase : IDocumentElement
    {
        private readonly List<IDocumentElement> _children;
        private IDocumentElement _parent;

        protected DocumentElementBase(string elementType)
        {
            ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
            Attributes = new DocumentAttributes();
            _children = new List<IDocumentElement>();
        }

        protected DocumentElementBase(string elementType, IDocumentAttributes attributes)
        {
            ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
            Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
            _children = new List<IDocumentElement>();
        }

        public string ElementType { get; }
        public IDocumentAttributes Attributes { get; }
        public IDocumentElement Parent => _parent;
        public IReadOnlyList<IDocumentElement> Children => new ReadOnlyCollection<IDocumentElement>(_children);

        public virtual void AddChild(IDocumentElement child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (child is DocumentElementBase childBase)
            {
                childBase._parent = this;
            }

            _children.Add(child);
        }

        public virtual void RemoveChild(IDocumentElement child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (_children.Remove(child) && child is DocumentElementBase childBase)
            {
                childBase._parent = null;
            }
        }

        public abstract void Accept(IDocumentVisitor visitor);
    }
}