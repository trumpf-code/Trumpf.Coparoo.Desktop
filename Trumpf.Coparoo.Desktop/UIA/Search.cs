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

namespace Trumpf.Coparoo.Desktop.UIA
{
    using Core;
    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects.UIAutomation;

    /// <summary>
    /// A restricted search pattern for objects accessible through UI Automation.
    /// </summary>
    public class Search : Search<UIAPattern>
    {
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
        /// <param name="classname">The class name to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByClassname(string classname)
        {
            return Any.AndByClassname(classname);
        }

        /// <summary>
        /// Create a search pattern for the given object identifier.
        /// </summary>
        /// <param name="objectIdentifier">The object identifier to search for.</param>
        /// <returns>The search pattern.</returns>
        public static Search ByObjectIdentifier(string objectIdentifier)
        {
            return Any.AndByObjectIdentifier(objectIdentifier);
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
        /// <param name="classname">The class name to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByClassname(string classname)
        {
            Pattern.ClassName = classname;
            return this;
        }

        /// <summary>
        /// Adds to a search pattern an additional search by object identifier.
        /// </summary>
        /// <param name="objectIdentifier">The object identifier to search for.</param>
        /// <returns>The search pattern.</returns>
        public Search AndByObjectIdentifier(string objectIdentifier)
        {
            Pattern.ObjectIdentifier = objectIdentifier;
            return this;
        }
        #endregion
    }
}
