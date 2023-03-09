// Copyright 2016 - 2023 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Extension methods for TestLeft controls.
    /// </summary>
    public static class IControlExtensions
    {
        /// <summary>
        /// Order the controls from left to right.
        /// </summary>
        /// <typeparam name="TSource">The enumerated type.</typeparam>
        /// <param name="source">The enumeration to order from left to right.</param>
        /// <returns>The ordered enumeration from screen left to right.</returns>
        public static IOrderedEnumerable<TSource> OrderFromLeftToRight<TSource>(this IEnumerable<TSource> source) where TSource : IControl
        {
            return source.OrderBy(e => e.ScreenLeft);
        }

        /// <summary>
        /// Order the controls from top to bottom.
        /// </summary>
        /// <typeparam name="TSource">The enumerated type.</typeparam>
        /// <param name="source">The enumeration to order.</param>
        /// <returns>The ordered enumeration.</returns>
        public static IOrderedEnumerable<TSource> OrderFromTopToBottom<TSource>(this IEnumerable<TSource> source) where TSource : IControl
        {
            return source.OrderBy(e => e.ScreenTop);
        }

        /// <summary>
        /// Order the controls from top left to bottom right.
        /// </summary>
        /// <typeparam name="TSource">The enumerated type.</typeparam>
        /// <param name="source">The enumeration to order.</param>
        /// <returns>The ordered enumeration.</returns>
        public static IOrderedEnumerable<TSource> OrderFromTopLeftToBottomRight<TSource>(this IEnumerable<TSource> source) where TSource : IControl
        {
            return source.OrderBy(e => e.ScreenTop).ThenBy(e => e.ScreenLeft);
        }

        /// <summary>
        /// Gets a value indicating whether the object is visible on screen
        /// Return false if the invoked object is missing (whereas IControl.VisibleOnScreen throws in that case)
        /// If the node may disappear during the call to this function, this function should be used instead of IControl.VisibleOnScreen.
        /// </summary>
        /// <param name="node">The node the check.</param>
        /// <returns>A value indicating whether the object is visible to the user.</returns>
        internal static bool VisibleOnScreen(this IControl node)
        {
            try
            {
                return node.VisibleOnScreen;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The invoked object is missing"))
                {
                    return false;
                }

                throw;
            }
        }
    }
}