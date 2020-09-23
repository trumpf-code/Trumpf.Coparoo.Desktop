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

namespace Trumpf.Coparoo.Desktop
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for the page object base class.
    /// </summary>
    public interface IPageObject : IUIObject
    {
        /// <summary>
        /// Gets the parent page object.
        /// </summary>
        new IPageObject Parent { get; }

        /// <summary>
        /// Gets the child page objects.
        /// Ignoring generic type definition page objects.
        /// </summary>
        /// <returns>The child page objects.</returns>
        IEnumerable<IPageObject> Children();

        /// <summary>
        /// Gets the child page objects.
        /// If a child page object is a generic type definition that matches with the hint type, this type is returned.
        /// The hint is necessary since the type parameters cannot be "guessed".
        /// </summary>
        /// <typeparam name="TPageObjectChildHint">The hint type for a generic type definition page object child.</typeparam>
        /// <returns>The child page objects.</returns>
        IEnumerable<IPageObject> Children<TPageObjectChildHint>();

        /// <summary>
        /// Goto the page object, i.e. perform necessary action to make the page object visible on screen.
        /// </summary>
        void Goto();
    }
}