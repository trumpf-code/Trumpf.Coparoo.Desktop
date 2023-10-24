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

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;
    using Trumpf.Coparoo.Desktop.Extensions;

    /// <summary>
    /// Base class of all control objects.
    /// </summary>
    /// <typeparam name="TNode">The node type of the control object.</typeparam>
    public abstract class ControlObject<TNode> : UIObject<TNode>, IControlObjectInternal where TNode : class, IControlObjectNode, new()
    {
        /// <inheritdoc/>
        internal override bool UiNodeExists => UIObjectNode.Accessible(InternalNode.TryRoot);

        /// <summary>
        /// Initialize the control object.
        /// </summary>
        /// <param name="pattern">The search pattern used to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        public virtual void Init(ISearchPattern pattern, Predicate<IControl> predicate)
        {
            Node.Init(pattern, predicate);
        }

        /// <summary>
        /// Cast the control object.
        /// </summary>
        /// <typeparam name="TControl">The type to cast to.</typeparam>
        /// <returns>The control object with the new type.</returns>
        public TControl Cast<TControl>() where TControl : IControlObject
        {
            var result = (TControl)Activator.CreateInstance(this.RootInternal().UIObjectInterfaceResolver.Resolve<TControl>());
            (result as IUIObjectInternal).Init(Parent);
            (result as IControlObjectInternal).Init(InternalNode.SearchPattern, Node.Predicate);
            (result as IUIObjectInternal).Init(PageObjectSearchDepth, ControlSearchDepth);
            (result as IUIObjectInternal).Index = InternalNode.Index;
            return result;
        }

        /// <summary>
        /// Click the control.
        /// </summary>
        public virtual void Click()
        {
            if (this.RootInternal().Configuration.EnableAutoScroll)
            {
                ScrollTo();
            }

            Node.Cast<IControl>().Click();
        }
    }
}