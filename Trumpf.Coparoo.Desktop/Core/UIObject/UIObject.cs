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

using SmartBear.TestLeft.TestObjects;
using SmartBear.TestLeft;
using System;
using Trumpf.Coparoo.Desktop.Core;
using Trumpf.Coparoo.Desktop.Extensions;
using System.Collections.Generic;

namespace Trumpf.Coparoo.Desktop
{
    public static class IUIObjectExtensions
    {
        /// <summary>
        /// Gets the parent of this UI object.
        /// </summary>
        public static IUIObject Parent(this IUIObject source)
        {
            return (source as IUIObjectInternal).Parent;
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="depth">The maximum search depth.</param>
        /// <returns>The control object.</returns>
        public static TControl Find<TControl>(this IUIObject source, ISearchPattern pattern = null, Predicate<IControl> predicate = null, int? depth = null) where TControl : IControlObject
        {
            var result = (TControl)Activator.CreateInstance(source.RootInternal().UIObjectInterfaceResolver.Resolve<TControl>());
            (result as IUIObjectInternal).Init(source);
            (result as IControlObjectInternal).Init(pattern, predicate);
            (result as IUIObjectInternal).Init(depth ?? ((IUIObjectInternal)source).ControlSearchDepth, null);
            return result;
        }

        /// <summary>
        /// Gets all matching controls.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="pattern">The search pattern to locate the control.</param>
        /// <param name="predicate">Additional control predicate in case the search pattern yields multiple matches.</param>
        /// <param name="depth">The maximum search depth.</param>
        /// <returns>The control enumeration.</returns>
        public static IEnumerable<TControl> FindAll<TControl>(this IUIObject source, ISearchPattern pattern = null, Predicate<IControl> predicate = null, int? depth = null) where TControl : IControlObject
        {
            int next = 0;
            while (true)
            {
                TControl result = source.Find<TControl>(pattern, predicate);
                (result as IUIObjectInternal).Init(depth, null);
                (result as IUIObjectInternal).Index = next++;
                if (!result.Exists)
                {
                    break;
                }

                yield return result;
            }
        }
    }
}
namespace Trumpf.Coparoo.Desktop.Core
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Extensions;
    using SmartBear.TestLeft.TestObjects;
    using Trumpf.Coparoo.Desktop.Waiting;
    using Trumpf.Coparoo.Desktop.Core.Waiting;
    using Trumpf.Coparoo.Desktop.Diagnostics;

    /// <summary>
    /// Base class of all page objects.
    /// </summary>
    /// <typeparam name="TNode">The node type of the page object.</typeparam>
    public abstract class UIObject<TNode> : IUIObjectInternal where TNode : class, IUIObjectNode, new()
    {
        private int? mPageObjectSearchDepth;
        private int? mControlObjectSearchDepth;

        private static MethodInfo theOnMethod;
        private IUIObjectNode node;

        /// <summary>
        /// Gets the root to control search depth.
        /// </summary>
        int IUIObjectInternal.PageObjectSearchDepth => PageObjectSearchDepth;

        /// <summary>
        /// Gets the root to control search depth.
        /// </summary>
        protected virtual int PageObjectSearchDepth => mPageObjectSearchDepth ?? this.RootInternal().Configuration.PageObjectSearchDepth;

        /// <summary>
        /// Gets the root to control search depth.
        /// </summary>
        int IUIObjectInternal.ControlSearchDepth => ControlSearchDepth;

        /// <summary>
        /// Gets the root to control search depth.
        /// </summary>
        protected virtual int ControlSearchDepth => mControlObjectSearchDepth ?? this.RootInternal().Configuration.ControlSearchDepth;

        /// <summary>
        /// Sets the 0-based control index.
        /// </summary>
        int IUIObjectInternal.Index
        {
            set { InternalNode.Index = value; }
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        IUIObjectNode IUIObject.Node
        {
            get { return node; }
        }

        /// <summary>
        /// Gets a value indicating whether the page object's node is visible on the screen.
        /// </summary>
        public Wool VisibleOnScreen => WoolFor(() => IsVisibleOnScreen, nameof(VisibleOnScreen));

        /// <summary>
        /// Gets a value indicating whether the page object's node exists in the UI tree.
        /// </summary>
        public virtual Wool Exists => WoolFor(() => UiNodeExists, nameof(Exists));

        /// <summary>
        /// Gets the horizontal position of the page object's left edge in screen coordinates.
        /// </summary>
        public virtual int ScreenLeft => Node.Cast<IControl>().ScreenLeft;

        /// <summary>
        /// Gets the vertical position of the page object's top edge in screen coordinates.
        /// </summary>
        public virtual int ScreenTop => Node.Cast<IControl>().ScreenTop;

        /// <summary>
        /// Gets the width of the UI object.
        /// </summary>
        public int Width => Node.Cast<IControl>().Width;

        /// <summary>
        /// Gets the height of the UI object.
        /// </summary>
        public int Height => Node.Cast<IControl>().Height;

        /// <summary>
        /// Gets or sets a value indicating whether the UI object can respond to user interaction.
        /// </summary>
        public Wool Enabled => WoolFor(() => Node.Cast<IControl>().Enabled, nameof(Enabled));

        /// <summary>
        /// Gets or sets a value indicating whether the control and all its child controls are displayed.
        /// </summary>
        public Wool Visible => WoolFor(() => Node.Cast<IControl>().Visible, nameof(Visible));

        /// <summary>
        /// Gets the parent of this page object.
        /// </summary>
        public IUIObject Parent { get; private set; }

        /// <summary>
        /// Gets a picture of the page object in its current state.
        /// </summary>
        public Image Picture => Node.Cast<IControl>().Picture();

        /// <summary>
        /// Gets the typed root node of the page object.
        /// This is the object this page object accesses to interact with UI elements.
        /// </summary>
        protected TNode Node
        {
            get { return (TNode)node; }
        }

        /// <summary>
        /// Gets the internal node.
        /// </summary>
        internal IUIObjectNodeInternal InternalNode => (IUIObjectNodeInternal)node;

        /// <summary>
        /// Gets a value indicating whether the page object's node is visible on the screen.
        /// </summary>
        protected virtual bool IsVisibleOnScreen
        {
            get
            {
                var node = InternalNode.TryRoot;
                var result = node != null && node.VisibleOnScreen();
                return result;
            }
        }

        /// <inheritdoc/>
        internal abstract bool UiNodeExists { get; }

        /// <summary>
        /// Initialize the control object.
        /// </summary>
        /// <param name="pageObjectSearchDepth">The maximum search depth used to locate the page object starting from the parent page object.</param>
        /// <param name="controlObjectSearchDepth">The maximum search depth used to locate the control object starting from the parent page object.</param>
        void IUIObjectInternal.Init(int? pageObjectSearchDepth, int? controlObjectSearchDepth)
        {
            mControlObjectSearchDepth = controlObjectSearchDepth;
            mPageObjectSearchDepth = pageObjectSearchDepth;
        }

        /// <summary>
        /// Initializes this object.
        /// </summary>
        /// <param name="parent">Parent page object.</param>
        /// <param name="root">The root node.</param>
        /// <returns>The initialized object.</returns>
        internal virtual IUIObject Init(IUIObject parent, IUIObjectNode root)
        {
            Parent = parent;
            node = root;

            return this;
        }

        /// <summary>
        /// Get the page object.
        /// </summary>
        /// <param name="pageType">The target page object type.</param>
        /// <returns>The page object.</returns>
        public IPageObject On(Type pageType)
        {
            if (theOnMethod == null)
            {
                var currentMethodName = MethodBase.GetCurrentMethod().Name;
                var methods = GetType().GetMethods();
                theOnMethod = methods.Single(e => e.Name == currentMethodName && !e.GetParameters().Any() && e.IsGenericMethod);
            }

            var result = theOnMethod.MakeGenericMethod(pageType).Invoke(this, null);
            return result as IPageObject;
        }

        /// <summary>
        /// Get the page object.
        /// </summary>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The page object.</returns>
        public TPageObject On<TPageObject>() where TPageObject : IPageObject
        {
            return On<TPageObject>(_ => true);
        }

        /// <summary>
        /// Get the page object.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The page object.</returns>
        public TPageObject On<TPageObject>(Predicate<TPageObject> condition) where TPageObject : IPageObject
        {
            return this.RootInternal().PageObjectLocator.Find(condition);
        }

        /// <inheritdoc/>
        public bool TrySnap()
        {
            return InternalNode.TrySnap();
        }

        /// <inheritdoc/>
        bool IUIObjectInternal.TryUnsnap()
        {
            return InternalNode.TryUnsnap();
        }

        /// <summary>
        /// Goto the page object.
        /// If the current page object cannot directly navigate to the target, it may forward it to its child page objects.
        /// Throws if the page object cannot be navigated to.
        /// </summary>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The target page object.</returns>
        public TPageObject Goto<TPageObject>() where TPageObject : IPageObject
        {
            return Goto<TPageObject>(_ => true);
        }

        /// <summary>
        /// Goto the page object.
        /// If the current page object cannot directly navigate to the target, it may forward it to its child page objects.
        /// Throws if the page object cannot be navigated to.
        /// </summary>
        /// <param name="condition">The condition that must evaluate true for target page object.</param>
        /// <typeparam name="TPageObject">The target page object type.</typeparam>
        /// <returns>The target page object.</returns>
        public TPageObject Goto<TPageObject>(Predicate<TPageObject> condition) where TPageObject : IPageObject
        {
            TPageObject r = On(condition);
            r.Goto();
            return r;
        }

        /// <inheritdoc/>
        IUIObject IUIObjectInternal.Init(IUIObject parent)
        {
            return Init(parent);
        }

        /// <summary>
        /// Initialize this object.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <returns>The initialized page object.</returns>
        internal virtual IUIObject Init(IUIObject parent)
        {
            bool enable = parent.Root().Configuration.EnableImages;
            TNode node = new TNode();
            ((IUIObjectNodeInternal)node).Init(parent.Node, GetHashCode(), enable, () => PageObjectSearchDepth, () => ControlSearchDepth);
            Init(parent, node);

            return this;
        }

        /// <summary>
        /// Gets an await-object.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The display name used in timeout exceptions.</param>
        /// <returns>The wrapped object.</returns>
        protected IAwait<T> Await<T>(Func<T> function, string name)
        {
            return this.RootInternal().Await(function, name);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return GetType().FullName.GetHashCode();
        }

        /// <inheritdoc/>
        public virtual void ScrollTo()
        {
            ScrollTo(this.RootInternal().Configuration.ScrollDetectionTimeout);
        }

        /// <inheritdoc/>
        public virtual void ScrollTo(TimeSpan timeout)
        {
            if (!VisibleOnScreen)
            {
                // find a visible ancestor.
                IUIObject ancestor = this;
                while (!ancestor.VisibleOnScreen)
                {
                    if (ancestor.Parent() is IRootObject)
                    {
                        throw new InvalidOperationException("Could not scroll to " + GetType().Name + ". Found no parent object that is visible on screen.");
                    }

                    ancestor = ancestor.Parent();
                }

                (ancestor as IUIObjectInternal).ScrollTo(this, timeout);
            }
        }

        /// <inheritdoc/>
        void IUIObjectInternal.ScrollTo(IUIObject target, TimeSpan timeout)
        {
            Node.Cast<IControl>().Keys("[Home]");
            Thread.Sleep(this.RootInternal().Configuration.ScrollSleep);

            foreach (var i in Enumerable.Range(0, this.RootInternal().Configuration.MaximumScrolls))
            {
                if (target.VisibleOnScreen.TryWaitFor(timeout))
                {
                    return;
                }

                Node.Cast<IControl>().Keys("[Down]"); // or [PageDown]
                Thread.Sleep(this.RootInternal().Configuration.ScrollSleep);
            }

            throw new InvalidOperationException("Couln't get " + GetType().Name + " visible on screen by scrolling from top to the bottom (ties " + this.RootInternal().Configuration.MaximumScrolls + " times).");
        }

        /// <summary>
        /// Gets a wrapped bool.
        /// </summary>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The wrapped bool.</returns>
        private Wool WoolFor(Func<bool> function, string name)
        {
            return new Wool(new Await<bool>(function, name, GetType(), () => this.RootInternal().Configuration.WaitTimeout, () => this.RootInternal().Configuration.PositiveWaitTimeout, () => this.RootInternal().Configuration.ShowWaitingDialog), () => TrySnap(), () => (this as IUIObjectInternal).TryUnsnap());
        }
    }
}