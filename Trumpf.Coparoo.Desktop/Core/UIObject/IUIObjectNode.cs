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

namespace Trumpf.Coparoo.Desktop.Core
{
    using System.Collections.Generic;

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// The UI node interface.
    /// </summary>
    public interface IUIObjectNode : IObject
    {
        /// <summary>
        /// Refresh the UI tree rooted in this node.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Find a control from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>The control or null if fails.</returns>
        TControl Find<TControl>(ISearchPattern pattern) where TControl : class, IObjectTreeNode;

        /// <summary>
        /// Find a control from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="depth">The search depth.</param>
        /// <returns>The control or null.</returns>
        TControl Find<TControl>(ISearchPattern pattern, int depth) where TControl : class, IObjectTreeNode;

        /// <summary>
        /// Find a control from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="result">The control or null.</param>
        /// <returns>Whether the control was found.</returns>
        bool TryFind<TControl>(ISearchPattern pattern, out TControl result) where TControl : class, IObjectTreeNode;

        /// <summary>
        /// Find a control from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="result">The control or null.</param>
        /// <param name="depth">The search depth.</param>
        /// <returns>Whether the control was found.</returns>
        bool TryFind<TControl>(ISearchPattern pattern, out TControl result, int depth) where TControl : class, IObjectTreeNode;

        /// <summary>
        /// Find all controls from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>The controls.</returns>
        IReadOnlyList<TControl> FindAll<TControl>(ISearchPattern pattern) where TControl : class, IObjectTreeNode;

        /// <summary>
        /// Find all controls from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="depth">The search depth (null for default).</param>
        /// <returns>The controls.</returns>
        IReadOnlyList<TControl> FindAll<TControl>(ISearchPattern pattern, int depth) where TControl : class, IObjectTreeNode;
    }
}