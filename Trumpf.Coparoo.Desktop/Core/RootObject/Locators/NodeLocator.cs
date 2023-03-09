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

namespace Trumpf.Coparoo.Desktop.Core
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// The page object locator class.
    /// </summary>
    internal class NodeLocator : INodeLocator
    {
        private readonly Dictionary<int, IControl> cache = new Dictionary<int, IControl>();
        private readonly Dictionary<int, Image> pictures = new Dictionary<int, Image>();

        /// <summary>
        /// Clear the register.
        /// </summary>
        public void Clear()
        {
            cache.Clear();
        }

        /// <summary>
        /// Register the page object type with the given object.
        /// </summary>
        /// <param name="hash">The object hash.</param>
        /// <param name="node">The object to register.</param>
        public void Register(int hash, IControl node)
        {
            cache[hash] = node;
        }

        /// <summary>
        /// Try get the registered objects for the given page object type.
        /// </summary>
        /// <param name="hash">The object hash.</param>
        /// <param name="result">The object, if any.</param>
        /// <returns>Whether a registered object was found.</returns>
        public bool TryGet(int hash, out IControl result)
        {
            return cache.TryGetValue(hash, out result);
        }

        /// <inheritdoc/>
        public void AddPicture(int hash, Image value)
        {
            pictures[hash] = value;
        }

        /// <inheritdoc/>
        public Image Picture(int hash)
        {
            Image result;
            return pictures.TryGetValue(hash, out result) ? result : null;
        }
    }
}