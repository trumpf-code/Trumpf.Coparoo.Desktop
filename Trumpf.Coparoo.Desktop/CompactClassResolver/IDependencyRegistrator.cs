using System;

namespace Trumpf.Coparoo.Desktop.CompactClassResolver
{
    /// <summary>  
    /// Provides the API for dependency registration.  
    /// </summary>  
    public interface IDependencyRegistrator
    {
        /// <summary>  
        /// Registers a pre-constructed instance (singleton).  
        /// </summary>  
        /// <typeparam name="T">Type of the instance.</typeparam>  
        /// <param name="instance">The instance to register.</param>  
        void RegisterInstance<T>(T instance);

        /// <summary>  
        /// Registers a concrete type so that it can be resolved.  
        /// The type must be a non-abstract, non‐interface type.  
        /// </summary>  
        /// <param name="type">The concrete type to register.</param>  
        void Register(Type type);

        /// <summary>  
        /// Registers a mapping between an abstract type or interface T1 and  
        /// a concrete implementation T2. T2 must be a concrete type and implement T1.  
        /// </summary>  
        void Register<T1, T2>();
    }
}