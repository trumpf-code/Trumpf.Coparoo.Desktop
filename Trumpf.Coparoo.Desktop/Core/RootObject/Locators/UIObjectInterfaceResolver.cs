﻿// Copyright 2016 - 2023 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System.Reflection;

    using Trumpf.Coparoo.Desktop;
    using Trumpf.Coparoo.Desktop.Exceptions;

    /// <summary>
    /// The UI object interface resolver.
    /// </summary>
    internal class UIObjectInterfaceResolver : IUIObjectInterfaceResolver
    {
        private IRootObject rootObject;
        private static Dictionary<Type, Type> resolveCache = new Dictionary<Type, Type>();
        private static Type[] controlTypesCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIObjectInterfaceResolver"/> class.
        /// </summary>
        /// <param name="rootObject">The process object.</param>
        public UIObjectInterfaceResolver(IRootObject rootObject) => this.rootObject = rootObject;

        /// <summary>
        /// Resolve the control type.
        /// </summary>
        /// <typeparam name="TControl">The control type to resolve.</typeparam>
        /// <returns>The control type with parameter-less default constructor.</returns>
        public Type Resolve<TControl>() where TControl : IControlObject
            => ResolveControlType<TControl>(true);

        /// <summary>
        /// Resolve the control type.
        /// </summary>
        /// <param name="toResolve">The control type to resolve.</param>
        /// <param name="retryOnce">Whether to retry once.</param>
        /// <returns>The control type with parameter-less default constructor.</returns>
        private Type ResolveControlType(Type toResolve, bool retryOnce)
        {
            Type result;
            if (!toResolve.IsInterface)
            {
                result = toResolve;
            }
            else if (resolveCache.TryGetValue(toResolve, out result))
            {
                // do nothing
            }
            else
            {
                controlTypesCache = controlTypesCache ?? PageTests.Locate.ControlObjectTypes().Where(e => e.IsAssignableFrom(e)).ToArray();

                Type[] matches = toResolve.GenericTypeArguments.Length == 0
                    ? controlTypesCache.Where(e => toResolve.IsAssignableFrom(e)).ToArray()
                    : controlTypesCache
                        .Where(e => e.GetTypeInfo().GenericTypeParameters.Length == toResolve.GenericTypeArguments.Length)
                        .Select(candidateType => TryResolve(candidateType, toResolve.GenericTypeArguments))
                        .Where(e => e != null && toResolve.IsAssignableFrom(e))
                        .ToArray();

                if (!matches.Any() && !retryOnce)
                {
                    throw new ControlObjectNotFoundException(toResolve);
                }
                else if (!matches.Any() && retryOnce)
                {
                    // reset caches
                    PageTests.Locate.ClearCaches();
                    controlTypesCache = null;

                    result = ResolveControlType(toResolve, false);
                }
                else
                {
                    // try to resolve to the lowest base class of all matches
                    // if matches contains a single type only then that type will be returned
                    result = LowestType(matches);

                    if (!toResolve.IsAssignableFrom(result))
                    {
                        throw new AmbiguousControlObjectMatchException(toResolve, matches, result);
                    }

                    if (matches.Count() >= 2)
                    {
                        Trace.TraceInformation($"Multiple matches found for control object interface <{toResolve.FullName}>: {string.Join(", ", matches.Select(e => e.FullName))}; selecting \"lowest\" match: '{result.FullName}'.");
                    }

                    resolveCache.Add(toResolve, result);
                }
            }

            return result;
        }

        private static Type TryResolve(Type candidateType, Type[] genericTypeArguments)
        {
            try
            {
                return candidateType.MakeGenericType(genericTypeArguments);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Resolve the control type.
        /// </summary>
        /// <typeparam name="TControl">The control type to resolve.</typeparam>
        /// <param name="retryOnce">Whether to retry once.</param>
        /// <returns>The control type with parameter-less default constructor.</returns>
        private Type ResolveControlType<TControl>(bool retryOnce) where TControl : IControlObject
            => ResolveControlType(typeof(TControl), retryOnce);

        /// <summary>
        /// Gets the "lowest" assignable type.
        /// If one type is passed that will be returned as well.
        /// </summary>
        /// <param name="types">The type.</param>
        /// <returns>The "lowest" type.</returns>
        private static Type LowestType(params Type[] types)
        {
            // source: https://stackoverflow.com/a/701880, MIT
            Type ret = types[0];

            for (int i = 1; i < types.Length; ++i)
            {
                if (types[i].IsAssignableFrom(ret))
                    ret = types[i];
                else
                {
                    while (!ret.IsAssignableFrom(types[i]))
                        ret = ret.BaseType;
                }
            }

            return ret;
        }
    }
}