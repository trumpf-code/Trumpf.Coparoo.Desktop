// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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

    using Exceptions;
    using Coparoo.Desktop;
    using Statistics;
    using Trumpf.Coparoo.Desktop.Core;

    /// <summary>
    /// Page object test class runner.
    /// </summary>
    public static class TestRunners
    {
        private static Type[] testClassTypes;
        private static Dictionary<Type, PageObjectStatistic> pageObjectStatistic = new Dictionary<Type, PageObjectStatistic>();

        /// <summary>
        /// Gets the page object test class types of the entire APP-Domain.
        /// </summary>
        private static Type[] TestClassTypes
        {
            get { return testClassTypes = testClassTypes ?? Locate.Types.Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPageObjectTestClass<>))).ToArray(); }
        }

        /// <summary>
        /// Gets the test method statistics.
        /// </summary>
        public static IEnumerable<TestMethodStatistic> TestMethodStatistics => pageObjectStatistic.SelectMany(e => e.Value.TestMethodStatistics);

        /// <summary>
        /// Run all tests with the <c>PageTestAttribute</c>.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <param name="methodFilter">The test method filter predicate.</param>
        /// <param name="pageTestClassFilter">The page test class filter predicate.</param>
        /// <returns>This page object.</returns>
        public static IPageObject TestBottomUp(this IPageObject source, Predicate<MethodInfo> methodFilter = null, Predicate<IPageObjectTests> pageTestClassFilter = null)
        {
            Trace.WriteLine("Run tests for " + source.GetType().FullName);

            // run tests for every child
            foreach (var child in source.Children())
            {
                child.TestBottomUp(methodFilter, pageTestClassFilter);
            }

            // run tests for this page object
            source.Test(methodFilter, pageTestClassFilter);

            return source;
        }

        /// <summary>
        /// Run tests for this page object.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <param name="methodFilter">The test method filter predicate.</param>
        /// <param name="pageTestClassFilter">The page test class filter predicate.</param>
        /// <returns>This page object.</returns>
        public static IPageObject Test(this IPageObject source, Predicate<MethodInfo> methodFilter = null, Predicate<IPageObjectTests> pageTestClassFilter = null)
        {
            methodFilter = methodFilter ?? (_ => true);
            pageTestClassFilter = pageTestClassFilter ?? (_ => true);

            foreach (Type classWithTests in source.TestClasses())
            {
                Trace.WriteLine("Found page test class for current class " + source.GetType().ToString() + ": " + classWithTests.ToString());

                // create and initialize page test class
                IPageObjectTestsInternal instance;
                var root = ((IUIObjectInternal)source).Root;
                try
                {
                    var configuration = root.Configuration;
                    configuration
                        .DependencyRegistrator
                        .RegisterType(classWithTests);

                    instance = (IPageObjectTestsInternal)configuration.resolver.Resolve(classWithTests);
                }
                catch (Stashbox.Exceptions.ResolutionFailedException exception)
                {
                    throw new TypeResolutionFailedException(exception, $"Configure the resolver via '{nameof(Configuration.DependencyRegistrator)}' in class '{root.GetType().FullName}'.");
                }

                // init page test class
                instance.Init(source);

                // check if tests should be executed according to the page test filter
                if (pageTestClassFilter(instance))
                {
                    Trace.WriteLine("Page test class filter returned true; running tests...");
                }
                else
                {
                    Trace.WriteLine("Page test class filter returned true; skipping tests...");
                    continue;
                }

                // check if tests should be executed according to the runnable predicate
                if (instance.Runnable)
                {
                    Trace.WriteLine("Runnable returned true; running tests...");
                }
                else
                {
                    Trace.WriteLine("Runnable returned false; skipping tests");
                    continue;
                }

                // get page test methods, and order them from top to bottom
                // ordering does not respect inheritence
                var methods = classWithTests.GetMethods().Where(method => method.GetCustomAttributes(typeof(PageTestAttribute), false).Length > 0);
                var methodsByLine = methods.OrderBy(method => method.GetCustomAttribute<PageTestAttribute>(true).Line);
                var matchingMethodsByLine = methodsByLine.Where(method => methodFilter(method));

                int testMethodsCount = matchingMethodsByLine.Count();
                if (testMethodsCount > 0)
                {
                    Trace.WriteLine("Found " + testMethodsCount + " test methods");
                }
                else
                {
                    Trace.WriteLine("No test method found; skipping test class");
                    continue;
                }

                Trace.WriteLine($"Execute function {nameof(IPageObjectTests.BeforeFirstTest)}...");
                instance.BeforeFirstTest();

                // check if tests should be executed
                if (!instance.ReadyToRun)
                {
                    throw new TestNotReadyToRunException(classWithTests.FullName);
                }
                else
                {
                    Trace.WriteLine("Tests are ready to run...");
                }

                // create class statistics object
                TestClassStatistic testClassStatistic = new TestClassStatistic(classWithTests);

                // execute test methods
                try
                {
                    foreach (var testMethod in matchingMethodsByLine)
                    {
                        if (instance.IsTestRunnable(testMethod))
                        {
                            Test(testClassStatistic, instance, testMethod);
                        }
                        else
                        {
                            Trace.WriteLine($"Skipping test: '{nameof(instance.IsTestRunnable)}' returned false for test method '{testMethod.Name}'.");
                        }
                    }

                    Trace.WriteLine($"Execute function {nameof(IPageObjectTests.AfterLastTest)}...");
                    instance.AfterLastTest();
                }
                finally
                {
                    var pageObjectStatistic = PageObjectStatistic(source);
                    pageObjectStatistic += testClassStatistic;
                }
            }

            return source;
        }

        /// <summary>
        /// Gets the test classes associated with this page object.
        /// </summary>
        /// <param name="source">The source page object.</param>
        /// <returns>The test classes associated with this page object.</returns>
        internal static IEnumerable<Type> TestClasses(this IPageObject source)
        {
            var interfaces = source.GetType().GetInterfaces();
            var pageObjectInterfaces = interfaces.Where(e => e.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IPageObject)));
            var matchingTypes = pageObjectInterfaces.Union(new[] { source.GetType() });
            var result = TestClassTypes.Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPageObjectTestClass<>) && matchingTypes.Contains(i.GenericTypeArguments.First()))).ToArray();
            return result;
        }

        /// <summary>
        /// Clear the page object statistic.
        /// </summary>
        /// <param name="pageObject">The page object.</param>
        internal static void ClearPageObjectStatistic(IPageObject pageObject)
        {
            pageObjectStatistic[pageObject.GetType()] = new PageObjectStatistic();
        }

        /// <summary>
        /// Gets the page object statistic for a certain page object type.
        /// </summary>
        /// <param name="pageObject">The page object.</param>
        /// <returns>The page object statistic.</returns>
        internal static PageObjectStatistic PageObjectStatistic(IPageObject pageObject)
        {
            var t = pageObject.GetType();
            if (!pageObjectStatistic.ContainsKey(t))
            {
                pageObjectStatistic[t] = new PageObjectStatistic();
            }

            return pageObjectStatistic[t];
        }

        /// <summary>
        /// Run tests and collect statistics.
        /// </summary>
        /// <param name="testClassStatistic">The test class statistics.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="testMethod">The test method.</param>
        private static void Test(TestClassStatistic testClassStatistic, IPageObjectTests instance, MethodInfo testMethod)
        {
            Trace.WriteLine("Executing page test <" + testMethod.Name + ">");

            var startTimeForCurrentIteration = DateTime.Now;
            try
            {
                // execute test method
                testMethod.Invoke(instance, null);

                // add method statistics
                testClassStatistic += new TestMethodStatistic() { MethodInfo = testMethod, PageType = instance.PageType, Info = "none", Start = startTimeForCurrentIteration, Success = true };
            }
            catch (Exception e)
            {
                Exception exceptionForTrace = e;

                if (e is System.Reflection.TargetInvocationException && e.InnerException != null)
                {
                    exceptionForTrace = e.InnerException;
                }

                Trace.WriteLine(string.Format("Exception in page test {0}: {1}", testMethod.Name, e.Message));
                Trace.WriteLine(exceptionForTrace.StackTrace);
                Trace.WriteLine(exceptionForTrace.ToString());

                // add method statistics
                testClassStatistic += new TestMethodStatistic() { MethodInfo = testMethod, PageType = instance.PageType, Info = e.GetType().Name, Start = startTimeForCurrentIteration, Success = false };

                throw;
            }

            Trace.WriteLine("Page test finished: " + testMethod.Name);
            Trace.WriteLine("------------------------------------------------------------");
        }
    }
}