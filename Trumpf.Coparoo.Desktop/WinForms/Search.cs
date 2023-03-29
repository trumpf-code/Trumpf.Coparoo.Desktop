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

namespace Trumpf.Coparoo.Desktop.WinForms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;
    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects.WinForms;

    /// <summary>
    /// A restricted search pattern for Windows Forms test objects.
    /// </summary>
    public class Search : Search<WinFormsPattern>
    {
        private static Dictionary<Type, Search> searchTypeCache = new Dictionary<Type, Search>();

        #region Helpers
        /// <summary>
        /// Gets the pattern matching any node.
        /// </summary>
        private static Search Any
        {
            get { return new Search() { }; }
        }
        #endregion

        #region Adaptors
        /// <summary>
        /// Create search pattern from the given pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>The search.</returns>
        public static Search By(ISearchPattern pattern)
        {
            return Any.And(pattern) as Search;
        }
        #endregion

        #region Creator methods
        /// <summary>
        /// Gets the enabled search pattern.
        /// </summary>
        /// <returns>The search pattern.</returns>
        public static Search IsEnabled()
        {
            return Any.AndIsEnabled();
        }

        /// <summary>
        /// Gets the visible on screen search pattern.
        /// </summary>
        /// <returns>The search pattern.</returns>
        public static Search IsVisibleOnScreen()
        {
            return Any.AndIsVisibleOnScreen();
        }

        /// <summary>
        /// Create a search pattern for the given constraint by control type.
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <param name="includeChildren">Whether to include derived types.</param>
        /// <returns>The search pattern.</returns>
        public static Search By<T>(bool includeChildren = false)
        {
            Search result;
            if (!includeChildren)
            {
                result = Any.AndBy<T>();
            }
            else if (!searchTypeCache.TryGetValue(typeof(T), out result))
            {
                result = Any.AndBy<T>();
                var instances = PageTests.Locate
                    .Types
                    .Where(t => typeof(T).IsAssignableFrom(t))
                    .Where(t => !t.IsAbstract)
                    .Where(t => t != typeof(T))
                    .ToArray();

                foreach (var i in instances)
                {
                    result.OrBy(i);
                }

                searchTypeCache.Add(typeof(T), result);
            }

            return result;
        }

        /// <summary>
        /// Create a search pattern for the given constraint by the fully class name.
        /// </summary>
        /// <param name="clrFullClassName">The full class name.</param>
        /// <returns>The search pattern.</returns>
        public Search ByClrFullClassName(string clrFullClassName)
            => Any.ByClrFullClassName(clrFullClassName);

        /// <summary>
        /// Create a search pattern for the given constraint by control name.
        /// </summary>
        /// <param name="controlName">The WPF control name to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByControlName(string controlName)
        {
            return Any.AndByControlName(controlName);
        }
        #endregion

        #region And methods
        /// <summary>
        /// Gets the extended search pattern.
        /// </summary>
        /// <returns>The search pattern.</returns>
        public new Search AndIsEnabled()
        {
            return base.AndIsEnabled() as Search;
        }

        /// <summary>
        /// Gets the extended search pattern.
        /// </summary>
        /// <returns>The search pattern.</returns>
        public Search AndIsVisibleOnScreen()
        {
            Pattern.Add("VisibleOnScreen", true);
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by control type.
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <returns>The search pattern.</returns>
        public Search AndBy<T>()
        {
            Pattern.ClrFullClassName = typeof(T).FullName;
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by control type.
        /// </summary>
        /// <param name="clrFullClassName">The full class name.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByClrFullClassName(string clrFullClassName)
        {
            Pattern.ClrFullClassName = clrFullClassName;
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by control name.
        /// </summary>
        /// <param name="controlName">The WPF control name to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByControlName(string controlName)
        {
            Pattern.WinFormsControlName = controlName;
            return this;
        }

        /// <summary>
        /// Adds a custom property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The search pattern.</returns>
        public new Search And(string name, object value)
        {
            base.And(name, value);
            return this;
        }
        #endregion

        #region Or methods
        /// <summary>
        /// Adds to a search pattern an additional search by control type.
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <returns>The search pattern.</returns>
        public Search OrBy<T>()
        {
            return OrBy(typeof(T));
        }

        /// <summary>
        /// Adds to a search pattern an additional search by control type.
        /// </summary>
        /// <param name="t">The type to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search OrBy(Type t)
        {
            Pattern.ClrFullClassName = AddOrConstraint(Pattern.ClrFullClassName, t.FullName);
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by control name.
        /// </summary>
        /// <param name="controlName">The WPF control name to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search OrByControlName(string controlName)
        {
            Pattern.WinFormsControlName = AddOrConstraint(Pattern.WinFormsControlName, controlName);
            return this;
        }
        #endregion
    }
}