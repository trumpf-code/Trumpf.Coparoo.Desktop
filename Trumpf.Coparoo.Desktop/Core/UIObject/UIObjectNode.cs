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
    using System.Diagnostics;
    using System.Linq;

    using Extensions;
    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;
    using Trumpf.Coparoo.Desktop.Waiting;

    /// <summary>
    /// Page object node class wrapping a UI tree node.
    /// </summary>
    public abstract class UIObjectNode : IUIObjectNode, IUIObjectNodeInternal
    {
        private Func<int> mPageObjectSearchDepth;
        private Func<int> mControlObjectSearchDepth;
        private IControl mNode;
        private int index = 0;
        private IUIObjectNode mParent;
        private int mHash;
        private bool mEnableImages;
        private IEnumerable<IControl> matches = null;
        private IRootObjectNode mRootNode = null;

        /// <summary>
        /// Gets the internal parent.
        /// </summary>
        private IUIObjectNodeInternal InternalParent => (IUIObjectNodeInternal)mParent;

        /// <summary>
        /// Sets the 0-based control index.
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// </summary>
        public virtual IControl Root
        {
            get { return mNode ?? GetRoot(() => RootUncached); }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found.
        /// </summary>
        public virtual IControl TryRoot
        {
            get { return mNode ?? GetRoot(() => TryRootUncached); }
        }

        /// <summary>
        /// Gets the process node.
        /// </summary>
        public IRootObjectNode RootNode
        {
            get { return mRootNode ?? (mRootNode = this is IRootObjectNode ? this as IRootObjectNode : InternalParent.RootNode); }
        }

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root.
        /// </summary>
        public abstract ISearchPattern SearchPattern { get; }

        /// <summary>
        /// Gets a value indicating whether to cache the node.
        /// </summary>
        protected virtual bool EnableCaching
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the root node.
        /// The root must always be reevaluated on get. There for not setter must be defined.
        /// </summary>
        protected virtual IObject Parent
        {
            get { return mParent; }
        }

        /// <summary>
        /// Gets the root node if possible, and otherwise returns null.
        /// </summary>
        protected virtual IObjectTreeNode TryParent
        {
            get { return InternalParent.TryRoot; }
        }

        /// <summary>
        /// Gets the maximum search depth for locating node starting from the root.
        /// </summary>
        public virtual int PageObjectSearchDepth => mPageObjectSearchDepth();

        /// <summary>
        /// Gets the maximum search depth for locating controls starting from the node.
        /// </summary>
        public virtual int ControlSearchDepth => mControlObjectSearchDepth();

        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// The node this class represents (for TestLeft of type IControl or similar)
        /// The node has to be searched from the <see cref="Parent"/> node
        /// This node must locate itself relative to the root node.
        /// </summary>
        internal virtual IControl RootUncached
        {
            get { return Index == 0 ? Parent.Cast<IObjectTreeNode>().Find<IControl>(SearchPattern, PageObjectSearchDepth) : Matches().ElementAt(Index); }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found
        /// The node this class represents (for TestLeft of type IControl or similar)
        /// The node has to be searched from the <see cref="Parent"/> node
        /// This node must locate itself relative to the root node.
        /// </summary>
        internal virtual IControl TryRootUncached
        {
            get
            {
                IObjectTreeNode parent = TryParent;
                if (parent == null)
                {
                    return null;
                }
                else if (index == 0)
                {
                    IControl node;
                    try
                    {
                        parent.TryFind(SearchPattern, PageObjectSearchDepth, 0, out node);
                    }
                    catch (ObjectTreeNodeNotFoundException)
                    {
                        // the parent disappeared
                        node = null;
                    }

                    return node;
                }
                else
                {
                    return Matches(parent).ElementAtOrDefault(Index);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether to snap node automatically.
        /// </summary>
        protected virtual bool EnableAutoSnap
        {
            get { return false; }
        }

        /// <summary>
        /// Refresh the UI tree rooted in this node.
        /// </summary>
        public void Refresh()
        {
            CallMethod("Refresh");
        }

        /// <summary>
        /// Determine whether the node is accessible, i.e. its properties can be retrieved.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Whether the node is accessible or not.</returns>
        internal static bool Accessible(IControl node)
        {
            if (node == null)
            {
                return false;
            }

            try
            {
                var temp = node.Enabled; // if we can access the property, we consider the node "accessible"
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Find all controls matching the search pattern.
        /// </summary>
        /// <param name="parentHint">The parent or null if it should be searched.</param>
        /// <returns>Matching controls.</returns>
        protected virtual IEnumerable<IControl> Matches(IObjectTreeNode parentHint = null)
        {
            parentHint = parentHint ?? TryParent;
            if (parentHint == null)
            {
                return Enumerable.Empty<IControl>();
            }
            else
            {
                if (matches == null)
                {
                    WaitHelper.For(() => (matches = parentHint.FindAll<IControl>(SearchPattern, PageObjectSearchDepth)).Any());
                }

                return matches;
            }
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return mHash;
        }

        /// <summary>
        /// Initialize this object.
        /// The parent node is used to search nodes without, hence disabling any caching.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="hash">The node hash.</param>
        /// <param name="enableImages">Whether to take images.</param>
        /// <param name="pageObjectSearchDepth">The page object search depth.</param>
        /// <param name="controlObjectSearchDepth">The control object search depth.</param>
        /// <returns>This object.</returns>
        public IUIObjectNode Init(IUIObjectNode parent, int hash, bool enableImages, Func<int> pageObjectSearchDepth, Func<int> controlObjectSearchDepth)
        {
            mHash = hash;
            mEnableImages = enableImages;
            mParent = parent;
            mPageObjectSearchDepth = pageObjectSearchDepth;
            mControlObjectSearchDepth = controlObjectSearchDepth;

            return this;
        }

        /// <inheritdoc/>
        public bool TrySnap()
        {
            mNode = GetRoot(() => TryRootUncached);
            return mNode != null;
        }

        /// <inheritdoc/>
        public bool TryUnsnap()
        {
            var before = mNode;
            mNode = null;
            return before != mNode;
        }

        /// <summary>
        /// Find a control from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>The control or null if fails.</returns>
        public TControl Find<TControl>(ISearchPattern pattern) where TControl : class, IObjectTreeNode
        {
            return Find<TControl>(pattern, ControlSearchDepth);
        }

        /// <summary>
        /// Find a control from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="depth">The search depth.</param>
        /// <returns>The control or null.</returns>
        public TControl Find<TControl>(ISearchPattern pattern, int depth) where TControl : class, IObjectTreeNode
        {
            return Root.Find<TControl>(pattern, depth);
        }

        /// <summary>
        /// Find a control from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="result">The control or null.</param>
        /// <returns>Whether the control was found.</returns>
        public bool TryFind<TControl>(ISearchPattern pattern, out TControl result) where TControl : class, IObjectTreeNode
        {
            return TryFind(pattern, out result, ControlSearchDepth);
        }

        /// <summary>
        /// Find a control from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="result">The control or null.</param>
        /// <param name="depth">The search depth.</param>
        /// <returns>Whether the control was found.</returns>
        public bool TryFind<TControl>(ISearchPattern pattern, out TControl result, int depth) where TControl : class, IObjectTreeNode
        {
            return Root.TryFind(pattern, depth, 0, out result);
        }

        /// <summary>
        /// Find all controls from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>The controls.</returns>
        public IReadOnlyList<TControl> FindAll<TControl>(ISearchPattern pattern) where TControl : class, IObjectTreeNode
        {
            return FindAll<TControl>(pattern, ControlSearchDepth);
        }

        /// <summary>
        /// Find all controls from the page object root node.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="depth">The search depth (null for default).</param>
        /// <returns>The controls.</returns>
        public IReadOnlyList<TControl> FindAll<TControl>(ISearchPattern pattern, int depth) where TControl : class, IObjectTreeNode
        {
            return Root.MyFindAll<TControl>(pattern, depth);
        }

        /// <summary>
        /// Cast the root node to an interface of another type.
        /// </summary>
        /// <typeparam name="T">The interface that will be implemented in the returned object.</typeparam>
        /// <returns>An object of the interface.</returns>
        public T Cast<T>() where T : class
        {
            return Root.Cast<T>();
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        /// <param name="findNode">The find node function.</param>
        /// <returns>The node control.</returns>
        private IControl GetRoot(Func<IControl> findNode)
        {
            IControl node;
            bool cached = ((IRootObjectNodeInternal)RootNode).NodeLocator.TryGet(GetHashCode(), out node);
            bool accessible = cached && Accessible(node);
            bool visibleOnScreen = accessible && node.VisibleOnScreen();

            var statistics = RootNode.Statistics;

            statistics.AccessCounter += 1;
            statistics.InCache += cached ? 1 : 0;
            statistics.AccessibleNodes += accessible ? 1 : 0;
            statistics.VisibleOnScreenNodes += visibleOnScreen ? 1 : 0;
            statistics.CachingEnabledCounter += EnableCaching ? 1 : 0;

            if (!visibleOnScreen)
            {
                // find node
                node = findNode();

                // store node if caching is enabled
                // not storing any node effectively deactivates caching
                if (node != null && EnableCaching)
                {
                    ((IRootObjectNodeInternal)RootNode).NodeLocator.Register(GetHashCode(), node);
                }

                // take picture
                var nodeLocator = (RootNode as IRootObjectNodeInternal).NodeLocator;
                int hash = GetHashCode();
                if (node != null && mEnableImages && nodeLocator.Picture(hash) == null)
                {
                    try
                    {
                        nodeLocator.AddPicture(hash, node.Picture());
                        Trace.WriteLine("Took an image for " + hash);
                    }
                    catch
                    {
                        Trace.WriteLine("Failed to take an image for " + hash);
                    }
                }
            }

            mNode = EnableAutoSnap ? node : mNode;

            return node;
        }

        /// <summary>
        /// Get the property.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The value.</returns>
        public T GetProperty<T>(string name, params object[] parameters)
        {
            return Root.GetProperty<T>(name, parameters);
        }

        /// <summary>
        /// Call the method.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The value.</returns>
        public T CallMethod<T>(string name, params object[] parameters)
        {
            return Root.CallMethod<T>(name, parameters);
        }

        /// <summary>
        /// Call the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        public void CallMethod(string name, params object[] parameters)
        {
            Root.CallMethod(name, parameters);
        }

        /// <summary>
        /// Set the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="parameters">The parameters.</param>
        public void SetProperty(string name, object value, params object[] parameters)
        {
            Root.SetProperty(name, value, parameters);
        }
    }
}