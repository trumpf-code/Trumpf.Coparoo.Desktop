using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trumpf.Coparoo.Desktop.CompactClassResolver
{
    /// <summary>  
    /// A lightweight dependency injection container that supports registering  
    /// instances as well as types (with constructor injection). Dependencies are  
    /// recursively resolved. Unregistered dependencies or circular references  
    /// result in exceptions.  
    /// </summary>  
    public class CompactClassResolverContainer : IDependencyRegistrator
    {
        // Stores singleton instances registered via RegisterInstance.  
        private readonly Dictionary<Type, object> instanceRegistry = new Dictionary<Type, object>();

        // Stores type mappings. For a given key type, the resolved concrete type is stored.  
        private readonly Dictionary<Type, Type> typeRegistry = new Dictionary<Type, Type>();

        /// <summary>  
        /// Registers a pre-created instance as a singleton for type T.  
        /// </summary>  
        /// <typeparam name="T">Type of the instance being registered.</typeparam>  
        /// <param name="instance">The instance to register.</param>  
        public void RegisterInstance<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            instanceRegistry[typeof(T)] = instance;
        }

        /// <summary>  
        /// Registers a concrete type so that it may be resolved later. The type must be a concrete (non‑abstract) class.  
        /// </summary>  
        /// <param name="type">The concrete type to register.</param>  
        public void Register(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsInterface || type.IsAbstract)
                throw new ArgumentException("Cannot register an interface or abstract type without a concrete implementation.", nameof(type));

            // Self-binding: when T is requested, an instance of T will be created.  
            typeRegistry[type] = type;
        }

        /// <summary>  
        /// Registers a mapping between an abstract type or interface T1 and  
        /// a concrete implementation T2.  
        /// </summary>  
        /// <typeparam name="T1">The service type (typically an interface or abstract class).</typeparam>  
        /// <typeparam name="T2">The concrete implementation type.</typeparam>  
        public void Register<T1, T2>()
        {
            // Ensure T2 is a concrete type.  
            if (typeof(T2).IsInterface || typeof(T2).IsAbstract)
                throw new ArgumentException("T2 must be a concrete type.", nameof(T2));

            // Ensure T2 implements T1.  
            if (!typeof(T1).IsAssignableFrom(typeof(T2)))
                throw new ArgumentException($"{typeof(T2).Name} is not assignable to {typeof(T1).Name}.");

            typeRegistry[typeof(T1)] = typeof(T2);
        }

        /// <summary>  
        /// Resolves an instance of type T by recursively constructing and injecting its dependencies.  
        /// </summary>  
        /// <typeparam name="T">The type to resolve.</typeparam>  
        /// <returns>An instance of type T.</returns>  
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T), new HashSet<Type>());
        }

        /// <summary>  
        /// Resolves an instance of the specified type by recursively constructing and injecting its dependencies.  
        /// </summary>  
        /// <param name="type">The type to resolve.</param>  
        /// <returns>An instance of the requested type.</returns>  
        public object Resolve(Type type)
        {
            return Resolve(type, new HashSet<Type>());
        }

        /// <summary>  
        /// Internal method that performs the actual resolution of a type. It inspects the available constructor(s)  
        /// and attempts to resolve each parameter recursively. A set of currently resolving types is passed along to  
        /// detect circular dependencies.  
        /// </summary>  
        /// <param name="type">The type to resolve.</param>  
        /// <param name="resolvingStack">A set tracking the current resolution path.</param>  
        /// <returns>An instance of the requested type.</returns>  
        private object Resolve(Type type, HashSet<Type> resolvingStack)
        {
            // Detect circular dependencies.  
            if (resolvingStack.Contains(type))
                throw new ResolutionFailedException("Circular dependency detected for type: " + type.FullName);

            // If an instance is already registered, return it.  
            if (instanceRegistry.TryGetValue(type, out object instance))
                return instance;

            // If a type mapping exists, use its associated concrete type.  
            if (typeRegistry.TryGetValue(type, out Type mappedType))
            {
                type = mappedType;
            }
            else
            {
                // If no binding exists for the requested type, resolution fails.  
                throw new ResolutionFailedException("Type not registered: " + type.FullName);
            }

            // Add the type to the current resolution chain.  
            resolvingStack.Add(type);

            // Retrieve public constructors.  
            ConstructorInfo[] constructors = type.GetConstructors();

            // If no public constructor is available, try to use non-public ones.  
            if (constructors.Length == 0)
                constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // Sort constructors by descending number of parameters.  
            ConstructorInfo[] sortedConstructors = constructors.OrderByDescending(c => c.GetParameters().Length).ToArray();

            Exception lastException = null;

            // Attempt to invoke one of the constructors.  
            foreach (ConstructorInfo constructor in sortedConstructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                object[] parameterInstances = new object[parameters.Length];
                bool canResolveAll = true;

                // Attempt to resolve every parameter required by the chosen constructor.  
                for (int i = 0; i < parameters.Length; i++)
                {
                    try
                    {
                        parameterInstances[i] = Resolve(parameters[i].ParameterType, resolvingStack);
                    }
                    catch (ResolutionFailedException ex)
                    {
                        canResolveAll = false;
                        lastException = ex;
                        break;
                    }
                }

                // If all parameters were successfully resolved, invoke the constructor.  
                if (canResolveAll)
                {
                    try
                    {
                        instance = constructor.Invoke(parameterInstances);
                        break;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }
                }
            }

            // Remove the type from the tracking set regardless of success.  
            resolvingStack.Remove(type);

            // If no constructor could be successfully invoked, throw an exception.  
            if (instance == null)
                throw new ResolutionFailedException("Failed to resolve type: " + type.FullName, lastException);

            return instance;
        }
    }
}