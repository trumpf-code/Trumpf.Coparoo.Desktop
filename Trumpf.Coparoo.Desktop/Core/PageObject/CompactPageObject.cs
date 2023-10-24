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
    using SmartBear.TestLeft;

    /// <summary>
    /// Base class of (node-less) page objects.
    /// </summary>
    /// <typeparam name="SearchPatternType">The type to search for.</typeparam>
    public abstract class CompactPageObject<SearchPatternType> : PageObject<CompactPageObjectNode> where SearchPatternType : ISearchPattern
    {
        /// <inheritdoc/>
        public new IUIObjectNode Node => base.Node;

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root.
        /// </summary>
        protected abstract SearchPatternType SearchPattern { get; }

        /// <summary>
        /// Initialize this object.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <returns>The initialized page object.</returns>
        internal override IUIObject Init(IUIObject parent)
        {
            base.Init(parent);
            base.Node.Init(SearchPattern);

            return this;
        }
    }
}