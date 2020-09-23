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

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;
    using SmartBear.TestLeft.TestObjects.Web;

    /// <summary>
    /// Base class for root nodes, i.e. nodes in the UI tree that are located from a process.
    /// </summary>
    public class WebPageObjectNode : RootObjectNode, IWebPageObjectNode
    {
        private string objectIdentifier;
        private string url;
        private IWebBrowser browser;
        private IWebPage webPage;

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root.
        /// </summary>
        private ISearchPattern WebBrowserSearchPattern
        {
            get { return new WebBrowserPattern { ObjectIdentifier = objectIdentifier }; }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// It's the same as the root process.
        /// </summary>
        public override IControl Root
        {
            get { return WebPage.Cast<IControl>(); }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found
        /// It's the same as the root process.
        /// </summary>
        public override IControl TryRoot
        {
            get
            {
                var result = TryWebPage;
                return result == null ? null : result.Cast<IControl>();
            }
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        protected override IObject Parent
        {
            get { throw new InvalidOperationException("A process node has no parent"); }
        }

        /// <summary>
        /// Gets the process.
        /// </summary>
        public IWebPage WebPage
        {
            get
            {
                return webPage ?? Driver.Find<IWebBrowser>(WebBrowserSearchPattern).Find<IWebPage>(SearchPattern);
            }
        }

        /// <summary>
        /// Gets the process or return null, if it cannot be found.
        /// </summary>
        protected IWebPage TryWebPage
        {
            get { return webPage ?? (Driver.TryFind(WebBrowserSearchPattern, 1, 0, out browser) && browser.TryFind(SearchPattern, 1, 0, out webPage) ? webPage : null); }
        }

        /// <summary>
        /// Gets the root node if possible, and otherwise return null.
        /// </summary>
        protected override IObjectTreeNode TryParent
        {
            get { throw new InvalidOperationException("A web page has no parent"); }
        }

        /// <summary>
        /// Open the web page.
        /// </summary>
        /// <param name="url">The url to open.</param>
        public void Open(string url)
        {
            if (Driver.TryFind(WebBrowserSearchPattern, 1, 0, out browser))
            {
                webPage = browser.ToUrl(url);
            }
        }

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root.
        /// </summary>
        public override ISearchPattern SearchPattern
        {
            get { return new WebPagePattern { URL = url }; }
        }

        /// <summary>
        /// Initializes the web page object node.
        /// </summary>
        /// <param name="objectIdentifier">The object identifier.</param>
        /// <param name="url">The URL.</param>
        public void Init(string objectIdentifier, string url)
        {
            this.objectIdentifier = objectIdentifier;
            this.url = url;
        }
    }
}