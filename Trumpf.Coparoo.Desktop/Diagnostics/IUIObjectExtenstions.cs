using System;
using Trumpf.Coparoo.Desktop.Core;
using Trumpf.Coparoo.Desktop.Extensions;

namespace Trumpf.Coparoo.Desktop.Diagnostics
{
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