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
    using System;
    using System.Linq;

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Base class of control objects.
    /// </summary>
    /// <typeparam name="TSearch">The type to search for.</typeparam>
    public abstract class CompactControlObject<TSearch> : ControlObject<ControlObjectNode> where TSearch : ISearchPattern, new()
    {
        /// <inheritdoc/>
        public new IUIObjectNode Node
            => base.Node;

        /// <summary>
        /// Gets the search pattern used to locate the control object.
        /// </summary>
        protected abstract TSearch SearchPattern { get; }

        /// <inheritdoc/>
        public override void Init(ISearchPattern pattern, Predicate<IControl> predicate)
        {
            ControlPattern conjunction = new ControlPattern();

            if (pattern != null)
            {
                foreach (var p in pattern.GetPatternItems())
                {
                    conjunction.Add(p.Key, p.Value);
                }
            }

            foreach (var p in SearchPattern.GetPatternItems())
            {
                if (!conjunction.GetPatternItems().Any(e => e.Key == p.Key))
                {
                    conjunction.Add(p.Key, p.Value);
                }
            }

            base.Init(conjunction, predicate);
        }
    }
}