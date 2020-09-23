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

namespace Trumpf.Coparoo.Desktop.Web
{
    /// <summary>
    /// Interface for web page objects.
    /// </summary>
    public interface IWebPageObject : IRootObject
    {
        /// <summary>
        /// Gets the object identifier, e.g. <c>iexplore</c>.
        /// </summary>
        string ObjectIdentifier { get; }

        /// <summary>
        /// Gets the url of the tab to access.
        /// Examples: <c>https://www.vdi-suedwest.de/</c> or <c>regexp:(https://www.vdi-suedwest.de/.*)</c>.
        /// </summary>
        string UrlPattern { get; }

        /// <summary>
        /// Open the web page in a new tab (the browser must already run).
        /// </summary>
        void Open();
    }
}