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

    /// <summary>
    /// Internal interface for the UI object base class.
    /// </summary>
    internal interface IUIObjectInternal : IUIObject
    {
        /// <summary>
        /// Gets the root to control search depth.
        /// </summary>
        int ControlSearchDepth { get; }

        /// <summary>
        /// Gets the root to control search depth.
        /// </summary>
        int PageObjectSearchDepth { get; }

        /// <summary>
        /// Gets the root page object.
        /// </summary>
        IRootObject Root { get; }

        /// <summary>
        /// Sets the 0-based control index.
        /// </summary>
        int Index { set; }

        /// <summary>
        /// Initialize this object.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <returns>The initialized page object.</returns>
        IUIObject Init(IUIObject parent);

        /// <summary>
        /// Initialize the control object.
        /// </summary>
        /// <param name="pageObjectSearchDepth">The maximum search depth used to locate the page object starting from the parent page object.</param>
        /// <param name="controlObjectSearchDepth">The maximum search depth used to locate the control object starting from the parent page object.</param>
        void Init(int? pageObjectSearchDepth, int? controlObjectSearchDepth);

        /// <summary>
        /// Scroll to the UI object.
        /// </summary>
        /// <param name="target">The UI object to scroll to.</param>
        /// <param name="timeout">The timeout to wait for the object to become visible between page downs.</param>
        void ScrollTo(IUIObject target, TimeSpan timeout);

        /// <summary>
        /// Unsnap the object.
        /// </summary>
        /// <returns>Whether the node was snapped before.</returns>
        bool TryUnsnap();
    }
}