﻿// Copyright 2016 - 2023 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System.Drawing;

    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// The node object locator interface.
    /// </summary>
    public interface INodeLocator
    {
        /// <summary>
        /// Clear the register.
        /// </summary>
        void Clear();

        /// <summary>
        /// Register the node object type with the given hash code.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="node">The object to register.</param>
        void Register(int hash, IControl node);

        /// <summary>
        /// Try get the registered node for the given hash code.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="result">The object, if any.</param>
        /// <returns>Whether a registered object was found.</returns>
        bool TryGet(int hash, out IControl result);

        /// <summary>
        /// Add the picture for the UI object type.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="value">The image to add.</param>
        void AddPicture(int hash, Image value);

        /// <summary>
        /// Gets the picture for the UI object type.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <returns>The image, or null if none exists.</returns>
        Image Picture(int hash);
    }
}