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

namespace Trumpf.Coparoo.Desktop.Core
{
    using System;
    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// The internal UI node interface.
    /// </summary>
    internal interface IUIObjectNodeInternal
    {
        /// <summary>
        /// Gets the root to page object search depth.
        /// </summary>
        int PageObjectSearchDepth { get; }

        /// <summary>
        /// Gets the root to control object search depth.
        /// </summary>
        int ControlSearchDepth { get; }

        /// <summary>
        /// Gets the process node.
        /// </summary>
        IRootObjectNode RootNode { get; }

        /// <summary>
        /// Sets the 0-based control index.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found.
        /// </summary>
        IControl TryRoot { get; }

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root.
        /// </summary>
        ISearchPattern SearchPattern { get; }

        /// <summary>
        /// Initialize this object.
        /// The parent node is used to search nodes without, hence disabling any caching.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="hash">The node hash.</param>
        /// <param name="enableImages">Whether to take an image.</param>
        /// <param name="pageObjectSearchDepth">The page object search depth.</param>
        /// <param name="controlObjectSearchDepth">The control object search depth.</param>
        /// <returns>This object.</returns>
        IUIObjectNode Init(IUIObjectNode parent, int hash, bool enableImages, Func<int> pageObjectSearchDepth, Func<int> controlObjectSearchDepth);

        /// <summary>
        /// Snap the node object, i.e. associate it with a specific node in the UI tree. This disables lazy node evaluation.
        /// </summary>
        /// <returns>Whether the snap succeeded.</returns>
        bool TrySnap();

        /// <summary>
        /// Unsnap the object.
        /// </summary>
        /// <returns>Whether the node was snapped before.</returns>
        bool TryUnsnap();
    }
}