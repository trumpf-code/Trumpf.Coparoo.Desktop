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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Logging.DotTree;
    using PageTests;

    /// <summary>
    /// Base class of all page objects.
    /// </summary>
    /// <typeparam name="TNode">The node type of the page object.</typeparam>
    public abstract class PageObject<TNode> : UIObject<TNode>, IPageObjectInternal, IPageObject where TNode : class, IPageObjectNode, new()
    {
        /// <summary>
        /// Gets a value indicating whether the page object shall be returned, e.g. by the ON-method.
        /// </summary>
        bool IPageObjectInternal.OnCondition => OnCondition;

        /// <summary>
        /// Gets a value indicating whether the page object shall be returned, e.g. by the ON-method.
        /// The default is: yes.
        /// </summary>
        protected virtual bool OnCondition => true;

        /// <summary>
        /// Gets the parent of this page object.
        /// </summary>
        public new IPageObject Parent => (IPageObject)base.Parent;

        /// <inheritdoc/>
        internal override bool UiNodeExists => UIObjectNode.Accessible(InternalNode.TryRoot);

        /// <summary>
        /// Gets or sets the page object image.
        /// </summary>
        Image IPageObjectInternal.Picture
        {
            get { return RootInternal.NodeLocator.Picture(GetHashCode()); }
        }

        /// <summary>
        /// Gets the dot tree representation
        /// Clears picture and statistics for this page objects and its (transitive) children.
        /// The tree does not contain generic type definition page objects.
        /// </summary>
        /// <returns>The dot tree representation of the page object tree.</returns>
        DotTree IPageObjectInternal.DotTree
        {
            get
            {
                // get page object image, if it exists
                DotTree result = new DotTree(
                    new Node
                    {
                        NodeType = Logging.DotTree.Node.NodeTypes.PageObject,
                        Id = GetType().FullName, Caption = GetType().Name,
                        Picture = ((IPageObjectInternal)this).Picture
                    });

                foreach (var dotTree in TestRunners.PageObjectStatistic(this).DotTrees)
                {
                    // merge trees
                    result += dotTree;

                    // connect trees
                    result += new Edge
                    {
                        Child = dotTree.Root.Id,
                        Parent = result.Root.Id
                    };
                }

                // clear statistics
                TestRunners.ClearPageObjectStatistic(this);

                foreach (var child in Children())
                {
                    var dotTree = ((IPageObjectInternal)child).DotTree;

                    // merge trees
                    result += dotTree;

                    // connect trees
                    result += new Edge
                    {
                        Child = dotTree.Root.Id,
                        Parent = result.Root.Id,
                        Label = string.Empty
                    };
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the child page objects.
        /// Ignoring generic type definition page objects.
        /// </summary>
        /// <returns>The child page objects.</returns>
        public IEnumerable<IPageObject> Children()
        {
            return Children<object>();
        }

        /// <summary>
        /// Gets the child page objects.
        /// If a child page object is a generic type definition that matches with the hint type, this type is returned.
        /// The hint is necessary since the type parameters cannot be "guessed".
        /// </summary>
        /// <typeparam name="TPageObjectChildHint">The hint type for a generic type definition page object child.</typeparam>
        /// <returns>The child page objects.</returns>
        public IEnumerable<IPageObject> Children<TPageObjectChildHint>()
        {
            List<IPageObject> result = new List<IPageObject>();
            foreach (var childPageObject in Locate.ChildTypes(this).Union(RootInternal.DynamicChildren(GetType())))
            {
                Type toAdd;
                Type hintType = typeof(TPageObjectChildHint);
                if (!childPageObject.IsGenericTypeDefinition)
                {
                    toAdd = childPageObject;
                }
                else if (hintType.IsGenericType && hintType.GetGenericTypeDefinition() == childPageObject)
                {
                    // check if the type definition of the page object we are searching for, is the same as the child page object
                    // use the type parameters passed via the type argument
                    toAdd = hintType;
                }
                else
                {
                    continue;
                }

                // create and init child page object
                result.Add(((IUIObjectInternal)Activator.CreateInstance(toAdd)).Init(this) as IPageObject);
            }

            return result;
        }

        /// <summary>
        /// Goto the page object, i.e. perform necessary action to make the page object visible on screen, do nothing if the page is already visible on screen.
        /// </summary>
        public virtual void Goto()
        {
            if (RootInternal.Configuration.AutoGoto && !VisibleOnScreen)
            {
                AutoGoto();
                VisibleOnScreen.WaitFor();
            }
        }

        /// <summary>
        /// Goto action for the page object, i.e. perform necessary action to make the page object visible on screen.
        /// </summary>
        protected virtual void AutoGoto()
        {
            if (Parent != null)
            {
                Parent.Goto();
            }
        }
    }
}