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

namespace Trumpf.Coparoo.Desktop.Web
{
    using System;

    using Core;

    /// <summary>
    /// Root web page object class.
    /// </summary>
    public abstract class WebPageObject : RootObject<WebPageObjectNode>, IWebPageObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebPageObject"/> class.
        /// </summary>
        public WebPageObject()
        {
            Configuration.PositiveWaitTimeout = TimeSpan.FromSeconds(1);
            Configuration.WaitTimeout = TimeSpan.FromMinutes(1);
            Configuration.EnableAutoScroll = true;
            Configuration.AutoGoto = true;
        }

        /// <inheritdoc/>
        public new IUIObjectNode Node => base.Node;

        /// <summary>
        /// Gets the url of the tab to access.
        /// Examples: <c>https://www.vdi-suedwest.de/</c> or <c>regexp:(https://www.vdi-suedwest.de/.*)</c>.
        /// </summary>
        public virtual string UrlPattern => "regexp:(" + UrlToOpen + ".*)";

        /// <summary>
        /// Gets the object identifier, e.g. <c>iexplore</c>
        /// </summary>
        public abstract string ObjectIdentifier { get; }

        /// <summary>
        /// Gets the url to open.
        /// </summary>
        public abstract string UrlToOpen { get; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        public string Url => base.Node.WebPage.URL;

        /// <summary>
        /// Gets a value indicating whether the web page is visible on screen.
        /// </summary>
        protected override bool IsVisibleOnScreen => false;

        /// <summary>
        /// Open the web page in a new tab (the browser must already run).
        /// </summary>
        public void Open()
        {
            base.Node.Open(UrlToOpen);
        }
    }
}