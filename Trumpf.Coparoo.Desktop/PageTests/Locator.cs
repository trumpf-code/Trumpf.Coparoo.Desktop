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

namespace Trumpf.Coparoo.Desktop.PageTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Core;
    using Exceptions;

    /// <summary>
    /// UI object type locators.
    /// </summary>
    internal static class Locate
    {
        private static Dictionary<Type, Type[]> childToparentsMap;
        private static Type[] pageObjectTypes;
        private static Type[] controlObjectTypes;
        private static Type[] uiObjectTypes;
        private static Func<Type, bool> pageObjectSelector = t => t.GetInterfaces().Contains(typeof(IPageObject));
        private static Func<Type, bool> controlObjectSelector = t => t.GetInterfaces().Contains(typeof(IControlObject));
        private static HashSet<Assembly> assembliesWithLoadErrors = new HashSet<Assembly>();

        public static List<Assembly> AssembliesToResolveTypes = new List<Assembly>();

        private static IEnumerable<Assembly> appDomainAssembliesNotInResolveTypes => AppDomain.CurrentDomain.GetAssemblies().Where(a => AssembliesToResolveTypes.Any(assembly => assembly.GetName().Name == a.GetName().Name) == false);
        /// <summary>
        /// Gets the retrievable app domain types.
        /// </summary>
        internal static Type[] Types
        {
            get
            {
                return AssembliesToResolveTypes.Union(appDomainAssembliesNotInResolveTypes).SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        if (assembliesWithLoadErrors.Add(assembly))
                        {
                            Trace.TraceInformation($"Loading types from assembly '{assembly.FullName}' failed. Ignore this message if the assembly does not contain control or page object implementations that need to be resolved at runtime. (Exception {ex.GetType().Name}. Message: {ex.Message}.)" + Environment.NewLine + string.Join(Environment.NewLine, ex.LoaderExceptions.Select(f => "- " + f.Message)));
                        }

                        return new Type[] { };
                    }
                    catch (Exception ex)
                    {
                        if (assembliesWithLoadErrors.Add(assembly))
                        {
                            Trace.TraceInformation($"Loading types from assembly '{assembly.FullName}' failed. Ignore this message if the assembly does not contain control or page object implementations that need to be resolved at runtime. (Exception {ex.GetType().Name}. Message: {ex.Message}.)");
                        }

                        return new Type[] { };
                    }
                }).ToArray();
            }
        }

        /// <summary>
        /// Gets the page object types in the current app domain.
        /// </summary>
        internal static Type[] PageObjectTypes()
            => pageObjectTypes ?? (pageObjectTypes = UIObjectTypes(pageObjectSelector));

        /// <summary>
        /// Gets the control object types in the current app domain.
        /// </summary>
        internal static Type[] ControlObjectTypes()
            => controlObjectTypes ?? (controlObjectTypes = UIObjectTypes(controlObjectSelector));

        /// <summary>
        /// Gets the cached child to parent page object type map.
        /// </summary>
        private static Dictionary<Type, Type[]> ChildToParent
            => childToparentsMap ?? (childToparentsMap = ChildToParentUncached());

        /// <summary>
        /// Compute the child to parent page object type map.
        /// </summary>
        private static Dictionary<Type, Type[]> ChildToParentUncached()
            => PageObjectTypes()
                .Select(pageObjectType => new { PageObjectType = pageObjectType, ChildOfInterfaces = pageObjectType.GetInterfaces().Where(f => f.IsGenericType && f.GetGenericTypeDefinition() == typeof(IChildOf<>)) })
                .Where(e => !e.PageObjectType.IsAbstract && e.ChildOfInterfaces.Any())
                .Select(e => new { e.PageObjectType, Parents = e.ChildOfInterfaces.Select(i => i.GenericTypeArguments.First()).ToArray() })
                .Select(e => new { e.PageObjectType, ResolvedParent = e.Parents.Select(parent => Resolve(parent)) })
                .ToDictionary(e => e.PageObjectType, e => e.ResolvedParent.ToArray());

        /// <summary>
        /// Gets the UI object types in the current app domain.
        /// </summary>
        /// <param name="selector">The selection predicate.</param>
        /// <returns>The type array.</returns>
        internal static Type[] UIObjectTypes(Func<Type, bool> selector = null)
        {
            selector = selector ?? (t => true);
            Predicate<Type> mainSelector = t =>
            {
                try
                {
                    return t.IsClass && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null && t.GetInterfaces().Contains(typeof(IUIObject));
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Skipping type <" + t.Name + "> during search for UIObjects: " + e.Message);
                    return false;
                }
            };

            uiObjectTypes = uiObjectTypes ?? Types.Where(t => mainSelector(t)).ToArray();
            return uiObjectTypes.Where(selector).ToArray();
        }

        /// <summary>
        /// Gets the page object types.
        /// </summary>
        /// <param name="parentToFind">The parent page object.</param>
        /// <returns>The child types.</returns>
        internal static IEnumerable<Type> ChildTypes(IPageObject parentToFind)
        {
            var notMatchingTypeYetMatchingQualifiedName = ChildToParent.Where(childAndParents => !childAndParents.Value.Contains(parentToFind.GetType()) && childAndParents.Value.Select(parent => parent.AssemblyQualifiedName).Contains(parentToFind.GetType().AssemblyQualifiedName));
            if (notMatchingTypeYetMatchingQualifiedName.Any())
            {
                var similarParents = notMatchingTypeYetMatchingQualifiedName.First().Value;
                var similarParent = similarParents.First(parent => parent.AssemblyQualifiedName == parentToFind.GetType().AssemblyQualifiedName);
                var childOfSimilarParent = notMatchingTypeYetMatchingQualifiedName.First().Key;
                throw new AmbiguousParentObjectFoundException(parentToFind, similarParent, childOfSimilarParent);
            }

            var resultByEquality = ChildToParent.Where(e => e.Value.Contains(parentToFind.GetType())).Select(e => e.Key);
            return resultByEquality;
        }

        /// <summary>
        /// Clear all caches.
        /// </summary>
        internal static void ClearCaches()
        {
            childToparentsMap = null;
            pageObjectTypes = null;
            controlObjectTypes = null;
            uiObjectTypes = null;
        }

        /// <summary>
        /// Resolve any page object interface type to an implementation.
        /// </summary>
        /// <param name="parentType">The type to resolve.</param>
        /// <returns>The resolved type.</returns>
        private static Type Resolve(Type parentType)
        {
            if (!parentType.IsInterface)
            {
                return parentType;
            }
            else
            {
                var matches = PageObjectTypes().Where(p => parentType.IsAssignableFrom(p));
                if (matches.Count() == 0)
                {
                    throw new ChildOfUsageException(parentType, "Could not resolve interface " + parentType.Name + " in " + typeof(IChildOf<>).Name + ".");
                }
                else
                {
                    if (matches.Count() > 1)
                    {
                        Trace.WriteLine("Found more than one match for " + parentType.Name + "; take one of them.");
                    }

                    return matches.First();
                }
            }
        }
    }
}