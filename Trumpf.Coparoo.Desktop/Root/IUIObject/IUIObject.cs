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

namespace Trumpf.Coparoo.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Core;
    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;
    using Waiting;

    /// <summary>
    /// Interface for the UI object base class.
    /// </summary>
    public interface IUIObject
    {
        /// <summary>
        /// Gets the parent of this UI object.
        /// </summary>
        IUIObject Parent { get; }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        IUIObjectNode Node { get; }

        /// <summary>
        /// Gets a value indicating whether the UI object's node is visible on the screen.
        /// </summary>
        Wool VisibleOnScreen { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the UI object can respond to user interaction.
        /// </summary>
        Wool Enabled { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the control and all its child controls are displayed.
        /// </summary>
        Wool Visible { get; }

        /// <summary>
        /// Gets a value indicating whether the UI object's node exists in the UI tree.
        /// </summary>
        Wool Exists { get; }

        /// <summary>
        /// Gets the horizontal position of the UI object's left edge in screen coordinates.
        /// </summary>
        int ScreenLeft { get; }

        /// <summary>
        /// Gets the vertical position of the UI object's top edge in screen coordinates.
        /// </summary>
        int ScreenTop { get; }

        /// <summary>
        /// Gets the width of the UI object.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the UI object.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets a picture of the UI object.
        /// </summary>
        Image Picture { get; }

        /// <summary>
        /// Get the specific page object.
        /// </summary>
        /// <param name="pageType">Type of the page object.</param>
        /// <returns>Type of page object.</returns>
        IPageObject On(Type pageType);

        /// <summary>
        /// Get the specific page object.
        /// </summary>
        /// <typeparam name="TPageObject">Type of the page object.</typeparam>
        /// <returns>Type of page object.</returns>
        TPageObject On<TPageObject>() where TPageObject : IPageObject;

        /// <summary>
        /// Get the specific page object.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
        /// <typeparam name="TPageObject">Type of the page object.</typeparam>
        /// <returns>Type of page object.</returns>
        TPageObject On<TPageObject>(Predicate<TPageObject> condition) where TPageObject : IPageObject;

        /// <summary>
        /// Snap the page object, i.e. associate it with a specific node in the UI tree. This disables lazy node evaluation.
        /// </summary>
        /// <returns>Whether the snap succeeded.</returns>
        bool TrySnap();

        /// <summary>
        /// Goto the page object.
        /// Throws if the page object cannot be navigated to.
        /// </summary>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The target page object.</returns>
        TPageObject Goto<TPageObject>() where TPageObject : IPageObject;

        /// <summary>
        /// Goto the page object.
        /// Throws if the page object cannot be navigated to.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for target page object.</param>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The target page object.</returns>
        TPageObject Goto<TPageObject>(Predicate<TPageObject> condition) where TPageObject : IPageObject;

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="depth">The maximum search depth.</param>
        /// <returns>The control object.</returns>
        TControl Find<TControl>(ISearchPattern pattern = null, Predicate<IControl> predicate = null, int? depth = null) where TControl : IControlObject;

        /// <summary>
        /// Gets all matching controls.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="depth">The maximum search depth.</param>
        /// <returns>The control enumeration.</returns>
        IEnumerable<TControl> FindAll<TControl>(ISearchPattern pattern = null, Predicate<IControl> predicate = null, int? depth = null) where TControl : IControlObject;

        /// <summary>
        /// Scroll to the UI object.
        /// </summary>
        void ScrollTo();

        /// <summary>
        /// Scroll to the UI object.
        /// </summary>
        /// <param name="timeout">The timeout to wait for the object to become visible between page downs.</param>
        void ScrollTo(TimeSpan timeout);
    }
}