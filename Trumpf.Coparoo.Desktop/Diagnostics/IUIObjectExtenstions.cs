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

namespace Trumpf.Coparoo.Desktop.Diagnostics
{
    using System;
    using Trumpf.Coparoo.Desktop.Core;
    using Trumpf.Coparoo.Desktop.Extensions;

    /// <summary>
    /// IUIObject extensions
    /// </summary>
    public static class IUIObjectExtenstions
    {
        /// <summary>
        /// Gets the type a type is resolved to.
        /// </summary>
        /// <typeparam name="TControl">The type to resolve.</typeparam>
        /// <param name="source">The source node.</param>
        /// <returns>The resolved type.</returns>
        public static Type Resolve<TControl>(this IUIObject source)
            where TControl : IControlObject
            => ((IUIObjectInternal)source).RootInternal().UIObjectInterfaceResolver.Resolve<TControl>();
    }
}