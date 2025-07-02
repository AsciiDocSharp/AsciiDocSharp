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

using AsciiDoc.NET.Core;
using AsciiDoc.NET.Core.Elements;
using AsciiDoc.NET.Parser.Implementation;
using AsciiDoc.NET.Converters.Html;
using System.Linq;
using Xunit;

namespace AsciiDoc.NET.Tests.Unit
{
    public class ChecklistTests
    {
        [Fact]
        public void ParseChecklist_UncheckedItems_ParsesCorrectly()
        {
            // Arrange
            var input = @"* [ ] Unchecked item 1
* [ ] Unchecked item 2";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IList>(document.Elements.First());
            
            var list = (IList)document.Elements.First();
            Assert.Equal(2, list.Children.Count());
            
            var firstItem = (IListItem)list.Children.First();
            Assert.True(firstItem.IsCheckbox);
            Assert.False(firstItem.IsChecked);
            Assert.Equal("Unchecked item 1", firstItem.Text);
            
            var secondItem = (IListItem)list.Children.Last();
            Assert.True(secondItem.IsCheckbox);
            Assert.False(secondItem.IsChecked);
            Assert.Equal("Unchecked item 2", secondItem.Text);
        }

        [Fact]
        public void ParseChecklist_CheckedItems_ParsesCorrectly()
        {
            // Arrange
            var input = @"* [x] Checked item 1
* [X] Checked item 2";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IList>(document.Elements.First());
            
            var list = (IList)document.Elements.First();
            Assert.Equal(2, list.Children.Count());
            
            var firstItem = (IListItem)list.Children.First();
            Assert.True(firstItem.IsCheckbox);
            Assert.True(firstItem.IsChecked);
            Assert.Equal("Checked item 1", firstItem.Text);
            
            var secondItem = (IListItem)list.Children.Last();
            Assert.True(secondItem.IsCheckbox);
            Assert.True(secondItem.IsChecked);
            Assert.Equal("Checked item 2", secondItem.Text);
        }

        [Fact]
        public void ParseChecklist_MixedItems_ParsesCorrectly()
        {
            // Arrange
            var input = @"* [x] Completed task
* [ ] Pending task
* Regular list item";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IList>(document.Elements.First());
            
            var list = (IList)document.Elements.First();
            Assert.Equal(3, list.Children.Count());
            
            var checkedItem = (IListItem)list.Children.ElementAt(0);
            Assert.True(checkedItem.IsCheckbox);
            Assert.True(checkedItem.IsChecked);
            Assert.Equal("Completed task", checkedItem.Text);
            
            var uncheckedItem = (IListItem)list.Children.ElementAt(1);
            Assert.True(uncheckedItem.IsCheckbox);
            Assert.False(uncheckedItem.IsChecked);
            Assert.Equal("Pending task", uncheckedItem.Text);
            
            var regularItem = (IListItem)list.Children.ElementAt(2);
            Assert.False(regularItem.IsCheckbox);
            Assert.False(regularItem.IsChecked);
            Assert.Equal("Regular list item", regularItem.Text);
        }

        [Fact]
        public void ConvertChecklist_ToHtml_ProducesCorrectMarkup()
        {
            // Arrange
            var input = @"* [x] Completed task
* [ ] Pending task";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var document = parser.Parse(input);

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<ul>", html);
            Assert.Contains("<input type=\"checkbox\" disabled checked>", html);
            Assert.Contains("<input type=\"checkbox\" disabled>", html);
            Assert.Contains("Completed task", html);
            Assert.Contains("Pending task", html);
            Assert.Contains("</ul>", html);
        }

        [Fact]
        public void ParseChecklist_OrderedList_ParsesCorrectly()
        {
            // Arrange
            var input = @"1. [x] First completed task
2. [ ] Second pending task";

            var parser = new AsciiDocParser();

            // Act
            var document = parser.Parse(input);

            // Assert
            Assert.Single(document.Elements);
            Assert.IsAssignableFrom<IList>(document.Elements.First());
            
            var list = (IList)document.Elements.First();
            Assert.Equal(ListType.Ordered, list.Type);
            Assert.Equal(2, list.Children.Count());
            
            var firstItem = (IListItem)list.Children.First();
            Assert.True(firstItem.IsCheckbox);
            Assert.True(firstItem.IsChecked);
            Assert.Equal("First completed task", firstItem.Text);
            
            var secondItem = (IListItem)list.Children.Last();
            Assert.True(secondItem.IsCheckbox);
            Assert.False(secondItem.IsChecked);
            Assert.Equal("Second pending task", secondItem.Text);
        }

        [Fact]
        public void ConvertChecklist_OrderedList_ProducesCorrectMarkup()
        {
            // Arrange
            var input = @"1. [x] First completed task
2. [ ] Second pending task";

            var parser = new AsciiDocParser();
            var converter = new HtmlDocumentConverter();
            var document = parser.Parse(input);

            // Act
            var html = converter.Convert(document);

            // Assert
            Assert.Contains("<ol>", html);
            Assert.Contains("<input type=\"checkbox\" disabled checked>", html);
            Assert.Contains("<input type=\"checkbox\" disabled>", html);
            Assert.Contains("First completed task", html);
            Assert.Contains("Second pending task", html);
            Assert.Contains("</ol>", html);
        }
    }
}