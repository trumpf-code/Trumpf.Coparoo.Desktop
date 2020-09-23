// Copyright 2016, 2017, 2018, 2019, 2020 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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

namespace Trumpf.Coparoo.Desktop.Web
{
    using System;
    using System.Collections.Generic;

    using Core;
    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects.Web;

    /// <summary>
    /// A restricted search pattern for web test objects.
    /// </summary>
    public class Search : Search<WebElementPattern>
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
        /// Gets the content text search pattern.
        /// </summary>
        /// <param name="value">The content text to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByContentText(string value)
        {
            return Any.AndByContentText(value);
        }

        /// <summary>
        /// Gets the object identifier search pattern.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByObjectIdentifier(string value)
        {
            return Any.AndByObjectIdentifier(value);
        }

        /// <summary>
        /// Create a search pattern for the given constraint by control type.
        /// </summary>
        /// <param name="value">The type to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByObjectType(string value)
        {
            return Any.AndByObjectType(value);
        }

        /// <summary>
        /// Create a search pattern for the given object label.
        /// </summary>
        /// <param name="value">The object label to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByObjectLabel(string value)
        {
            return Any.AndByObjectLabel(value);
        }

        /// <summary>
        /// Create a search pattern for the given class name.
        /// </summary>
        /// <param name="value">The class name to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByClassName(string value)
        {
            return Any.AndByClassName(value);
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
        /// Gets the extended search pattern.
        /// </summary>
        /// <param name="value">The content text to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByContentText(string value)
        {
            Pattern.contentText = value;
            return this;
        }

        /// <summary>
        /// Gets the object identifier search pattern.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByObjectIdentifier(object value)
        {
            Pattern.ObjectIdentifier = value;
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by object type.
        /// </summary>
        /// <param name="value">The object type to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByObjectType(string value)
        {
            Pattern.ObjectType = value;
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by object label.
        /// </summary>
        /// <param name="value">The object label to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByObjectLabel(string value)
        {
            Pattern.ObjectLabel = value;
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by class name.
        /// </summary>
        /// <param name="value">The class name to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByClassName(string value)
        {
            Pattern.Add("className", value);
            return this;
        }
        #endregion

        #region Or methods
        /// <summary>
        /// Adds an additional search pattern.
        /// </summary>
        /// <param name="value">The object label to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search OrByObjectType(string value)
        {
            Pattern.ObjectType = AddOrConstraint(Pattern.ObjectType, value);
            return this;
        }
        #endregion
    }
}