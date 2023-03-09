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

namespace Trumpf.Coparoo.Desktop.WPF
{
    /// <summary>
    /// Base class of (node-less) page objects.
    /// </summary>
    /// <typeparam name="TView">The type to search for.</typeparam>
    public abstract class ViewControlObject<TView> : ControlObject
    {
        /// <inheritdoc/>
        protected override Search SearchPattern
        {
            get { return Search.By<TView>(true); }
        }
    }
}