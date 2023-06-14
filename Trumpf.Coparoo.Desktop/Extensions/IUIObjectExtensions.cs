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

using SmartBear.TestLeft.TestObjects;
using SmartBear.TestLeft;
using System;
using Trumpf.Coparoo.Desktop.Core;
using System.Collections.Generic;

namespace Trumpf.Coparoo.Desktop.Extensions
{
    public static class IUIObjectExtensions
    {
        public static IUIObjectNode Node(this IUIObject source)
        {
            return (source as IUIObjectInternal).Node;
        }

        /// <summary>
        /// Gets the parent of this UI object.
        /// </summary>
        public static IUIObject Parent(this IUIObject source)
        {
            return (source as IUIObjectInternal).Parent;
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="depth">The maximum search depth.</param>
        /// <returns>The control object.</returns>
        public static TControl Find<TControl>(this IUIObject source, ISearchPattern pattern = null, Predicate<IControl> predicate = null, int? depth = null) where TControl : IControlObject
        {
            var result = (TControl)Activator.CreateInstance(source.RootInternal().UIObjectInterfaceResolver.Resolve<TControl>());
            (result as IUIObjectInternal).Init(source);
            (result as IControlObjectInternal).Init(pattern, predicate);
            (result as IUIObjectInternal).Init(depth ?? ((IUIObjectInternal)source).ControlSearchDepth, null);
            return result;
        }

        /// <summary>
        /// Gets all matching controls.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="depth">The maximum search depth.</param>
        /// <returns>The control enumeration.</returns>
        public static IEnumerable<TControl> FindAll<TControl>(this IUIObject source, ISearchPattern pattern = null, Predicate<IControl> predicate = null, int? depth = null) where TControl : IControlObject
        {
            int next = 0;
            while (true)
            {
                TControl result = source.Find<TControl>(pattern, predicate);
                (result as IUIObjectInternal).Init(depth, null);
                (result as IUIObjectInternal).Index = next++;
                if (!result.Exists)
                {
                    break;
                }

                yield return result;
            }
        }

        /// <summary>
        /// Gets the root page object.
        /// </summary>
        /// <returns>The root page object.</returns>
        public static IRootObject Root(this IUIObject source)
            => source is IRootObject ? source as IRootObject : source.Parent().Root();

        /// <summary>
        /// Gets the type a type is resolved to.
        /// </summary>
        /// <typeparam name="TControl">The type to resolve.</typeparam>
        /// <param name="source">The source node.</param>
        /// <returns>The resolved type.</returns>
        public static Type Resolve<TControl>(this IUIObject source)
            where TControl : IControlObject
            => ((IUIObjectInternal)source).RootInternal().UIObjectInterfaceResolver.Resolve<TControl>();
    }
}
