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
using AsciiDocSharp.Core.Elements;

namespace AsciiDocSharp.Core.Implementation
{
    public class IncludeMacro : Macro, IIncludeMacro
    {
        public IncludeMacro(string target, IReadOnlyDictionary<string, string> parameters, MacroType macroType)
            : base("include", target, parameters, macroType)
        {
            FilePath = target;
            LevelOffset = GetParameterOrDefault("leveloffset", string.Empty);
            Lines = GetParameterOrDefault("lines", string.Empty);
            Tags = GetParameterOrDefault("tags", string.Empty);
            IndentLevel = GetParameterOrDefault("indent", string.Empty);
            Optional = ParseBoolParameter("optional");
        }

        public string FilePath { get; }
        public string LevelOffset { get; }
        public string Lines { get; }
        public string Tags { get; }
        public string IndentLevel { get; }
        public bool Optional { get; }

        private string GetParameterOrDefault(string key, string defaultValue)
        {
            return Parameters.TryGetValue(key, out var value) ? value : defaultValue;
        }

        private bool ParseBoolParameter(string key, bool defaultValue = false)
        {
            if (Parameters.TryGetValue(key, out var value))
            {
                return value.Equals("true", System.StringComparison.OrdinalIgnoreCase) || 
                       value.Equals("1", System.StringComparison.Ordinal) ||
                       value.Equals("yes", System.StringComparison.OrdinalIgnoreCase);
            }
            return defaultValue;
        }

        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}