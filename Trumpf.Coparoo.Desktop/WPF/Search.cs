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

namespace Trumpf.Coparoo.Desktop.WPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;
    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects.WPF;

    /// <summary>
    /// A restricted search pattern for Windows Presentation Foundation test objects.
    /// </summary>
    public class Search : Search<WPFPattern>
    {
        private static Dictionary<Type, Search> searchTypeCache = new Dictionary<Type, Search>();

        #region Helpers
        /// <summary>
        /// Gets the pattern matching any node.
        /// </summary>
        public static Search Any
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
        /// Gets the index search pattern.
        /// </summary>
        /// <param name="index">The index to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByIndex(int index)
        {
            return Any.AndByIndex(index);
        }

        /// <summary>
        /// Create a search pattern for the given constraint by control type.
        /// </summary>
        /// <param name="includeChildren">Whether to include derived types.</param>
        /// <typeparam name="T">The type to search for.</typeparam>
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
        public static Search ByClrFullClassName(string clrFullClassName)
            => Any.AndByClrFullClassName(clrFullClassName);

        /// <summary>
        /// Create a search pattern for the given constraint by control name.
        /// </summary>
        /// <param name="controlName">The WPF control name to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByControlName(string controlName)
        {
            return Any.AndByControlName(controlName);
        }

        /// <summary>
        /// Create a search pattern for the given constraint by automation id.
        /// </summary>
        /// <param name="automationId">The WPF automation Id to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByAutomationId(string automationId)
        {
            return Any.AndByAutomationId(automationId);
        }

        /// <summary>
        /// Create a search pattern for the given constraint by UID.
        /// </summary>
        /// <param name="uid">The UID to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByUid(string uid)
        {
            return Any.AndByUid(uid);
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
        /// Gets the index search pattern.
        /// </summary>
        /// <param name="index">The index to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByIndex(int index)
        {
            Pattern.WPFControlOrdinalNo = (index + 1);
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
        /// <param name="cntrolName">The WPF control name to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByControlName(string cntrolName)
        {
            Pattern.WPFControlName = cntrolName;
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by automation id.
        /// </summary>
        /// <param name="automationId">The WPF automation Id to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByAutomationId(string automationId)
        {
            Pattern.WPFControlAutomationId = automationId;
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by automation id.
        /// </summary>
        /// <param name="uid">The UID to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByUid(string uid)
        {
            Pattern.Uid = uid;
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
            Pattern.WPFControlName = AddOrConstraint(Pattern.WPFControlName, controlName);
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by control name.
        /// </summary>
        /// <param name="automationId">The WPF automation Id to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search OrByAutomationId(string automationId)
        {
            Pattern.WPFControlAutomationId = AddOrConstraint(Pattern.WPFControlAutomationId, automationId);
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by control name.
        /// </summary>
        /// <param name="uid">The UID to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search OrByUid(string uid)
        {
            Pattern.Uid = AddOrConstraint(Pattern.Uid, uid);
            return this;
        }
        #endregion
    }
}