// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Desktop.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for page objects.
    /// </summary>
    public static class IPageObjectExtensions
    {
        /// <summary>
        /// Order the controls from left to right.
        /// </summary>
        /// <typeparam name="TSource">The enumerated type.</typeparam>
        /// <param name="source">The enumeration to order from left to right.</param>
        /// <returns>The ordered enumeration from screen left to right.</returns>
        public static IOrderedEnumerable<TSource> OrderPagesFromLeftToRight<TSource>(this IEnumerable<TSource> source) where TSource : IUIObject
        {
            return source.OrderBy(e => e.ScreenLeft);
        }

        /// <summary>
        /// Order the controls from top to bottom.
        /// </summary>
        /// <typeparam name="TSource">The enumerated type.</typeparam>
        /// <param name="source">The enumeration to order.</param>
        /// <returns>The ordered enumeration.</returns>
        public static IOrderedEnumerable<TSource> OrderPagesFromTopToBottom<TSource>(this IEnumerable<TSource> source) where TSource : IUIObject
        {
            return source.OrderBy(e => e.ScreenTop);
        }

        /// <summary>
        /// Order the controls from top left to bottom right.
        /// </summary>
        /// <typeparam name="TSource">The enumerated type.</typeparam>
        /// <param name="source">The enumeration to order.</param>
        /// <returns>The ordered enumeration.</returns>
        public static IOrderedEnumerable<TSource> OrderPagesFromTopLeftToBottomRight<TSource>(this IEnumerable<TSource> source) where TSource : IUIObject
        {
            return source.OrderBy(e => e.ScreenTop).ThenBy(e => e.ScreenLeft);
        }
    }
}