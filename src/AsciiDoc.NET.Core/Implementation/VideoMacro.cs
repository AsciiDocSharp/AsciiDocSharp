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

using System.Collections.Generic;
using System.Globalization;
using AsciiDoc.NET.Core.Elements;

namespace AsciiDoc.NET.Core.Implementation
{
    public class VideoMacro : Macro, IVideoMacro
    {
        public VideoMacro(string target, IReadOnlyDictionary<string, string> parameters, MacroType macroType)
            : base("video", target, parameters, macroType)
        {
            Source = target;
            Title = GetParameterOrDefault("title", string.Empty);
            Width = ParseIntParameter("width");
            Height = ParseIntParameter("height");
            Poster = GetParameterOrDefault("poster", string.Empty);
            AutoPlay = ParseBoolParameter("autoplay");
            Controls = ParseBoolParameter("controls", true);
            Loop = ParseBoolParameter("loop");
            Muted = ParseBoolParameter("muted");
            VideoFormat = GetParameterOrDefault("format", DetermineFormatFromExtension(target));
        }

        public string Source { get; }
        public string Title { get; }
        public int? Width { get; }
        public int? Height { get; }
        public string Poster { get; }
        public bool AutoPlay { get; }
        public bool Controls { get; }
        public bool Loop { get; }
        public bool Muted { get; }
        public string VideoFormat { get; }

        private string GetParameterOrDefault(string key, string defaultValue)
        {
            return Parameters.TryGetValue(key, out var value) ? value : defaultValue;
        }

        private int? ParseIntParameter(string key)
        {
            if (Parameters.TryGetValue(key, out var value) && 
                int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            return null;
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

        private static string DetermineFormatFromExtension(string filename)
        {
            var extension = System.IO.Path.GetExtension(filename)?.ToLowerInvariant();
            switch (extension)
            {
                case ".mp4":
                    return "mp4";
                case ".webm":
                    return "webm";
                case ".ogg":
                    return "ogg";
                case ".avi":
                    return "avi";
                case ".mov":
                    return "mov";
                default:
                    return "mp4";
            }
        }

        public override void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}