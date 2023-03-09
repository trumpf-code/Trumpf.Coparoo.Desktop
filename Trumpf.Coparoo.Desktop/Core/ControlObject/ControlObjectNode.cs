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
    using System.Linq;

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// A unique control object node in the UI tree.
    /// </summary>
    public class ControlObjectNode : UIObjectNode, IControlObjectNode
    {
        private ISearchPattern pattern;
        private Predicate<IControl> predicate;

        /// <summary>
        /// Gets the predicate selector.
        /// </summary>
        public Predicate<IControl> Predicate
        {
            get { return predicate == null ? _ => true : predicate; }
        }

        /// <summary>
        /// Gets the root to node search pattern.
        /// </summary>
        public override ISearchPattern SearchPattern
        {
            get { return pattern; }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// </summary>
        internal override IControl RootUncached
        {
            get { return predicate == null ? base.RootUncached : Matches().ElementAt(Index); }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found.
        /// </summary>
        internal override IControl TryRootUncached
        {
            get { return predicate == null ? base.TryRootUncached : Matches().ElementAtOrDefault(Index); }
        }

        /// <summary>
        /// Gets a value indicating whether to cache the node
        /// Control object must not be cached.
        /// </summary>
        protected override bool EnableCaching
        {
            get { return false; }
        }

        /// <summary>
        /// Find all controls matching the search pattern and the predicate.
        /// </summary>
        /// <param name="parentHint">The parent or null if it should be searched.</param>
        /// <returns>Matching controls.</returns>
        protected override IEnumerable<IControl> Matches(IObjectTreeNode parentHint = null)
        {
            return base.Matches(parentHint).Where(c => Predicate(c));
        }

        /// <summary>
        /// Initialize the control object.
        /// </summary>
        /// <param name="pattern">The search pattern used to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        public virtual void Init(ISearchPattern pattern, Predicate<IControl> predicate = null)
        {
            this.pattern = pattern;
            this.predicate = predicate;
        }
    }
}