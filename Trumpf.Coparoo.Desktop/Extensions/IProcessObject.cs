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

namespace Trumpf.Coparoo.Desktop.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;

    using SmartBear.TestLeft;

    /// <summary>
    /// Extension methods for process objects.
    /// </summary>
    public static class IProcessObjectExtensions
    {
        private static readonly Predicate<Exception> defaultRetryCondition =
                e =>
                    e.Message.Contains("No more instances can be launched at the moment") ||
                    e.Message.Contains("Error code: 127") ||
                    e.Message.Contains("The server committed a protocol violation") ||
                    e.Message.Contains("The process TestExecute started, but the REST server did not start") ||
                    (e.GetType() == typeof(Win32Exception) && e.Message.Contains("Access is denied")) ||
                    (e.GetType() == typeof(Win32Exception) && e.Message.Contains("A 32 bit processes cannot access modules of a 64 bit process")) ||
                    (e.GetType() == typeof(ApiException) && e.Message.Contains("TestExecute failed to reach the 'running' state"))
                ;

        private static readonly Func<IDriver> defaultDriverCreator = () => new LocalDriver();

        /// <summary>
        /// Initializes process object's local driver.
        /// Retries initialization up 100 times and wait 10 seconds between retries.
        /// </summary>
        /// <param name="processObject">The process object to initialize.</param>
        public static void InitializeLocalDriver(this IProcessObject processObject)
            => InitializeLocalDriver(processObject, 100, TimeSpan.FromSeconds(10));

        /// <summary>
        /// Initializes process object's local driver.
        /// </summary>
        /// <param name="processObject">The process object to initialize.</param>
        /// <param name="maxAttempts">The maximum number of retries.</param>
        /// <param name="retryInterval">The waiting time between retries.</param>
        /// <returns>List of eexceptions that caused retries.</returns>
        public static List<Exception> InitializeLocalDriver(this IProcessObject processObject, int maxAttempts, TimeSpan retryInterval)
            => Retry(
                () => { processObject.Driver = defaultDriverCreator(); },
                defaultRetryCondition,
                retryInterval,
                maxAttempts
                );

        /// <summary>
        /// Retry on exception. Before retry execute recovery action.
        /// </summary>
        /// <typeparam name="E">The exception to retry on.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="when">The exception condition.</param>
        /// <param name="retryInterval">The retry interval.</param>
        /// <param name="maxAttempts">The number of attempts (1 = no retry).</param>
        /// <returns>List of eexceptions that caused retries.</returns>
        private static List<Exception> Retry<E>(Action action, Predicate<E> when, TimeSpan retryInterval, int maxAttempts) where E : Exception
        {
            int remainingAttempts = maxAttempts;
            var exceptions = new List<Exception>();

            while (true)
            {
                try
                {
                    action();
                    return exceptions;
                }
                catch (E e)
                {
                    exceptions.Add(e);
                    if (remainingAttempts-- > 0 && (when == null || when(e)))
                    {
                        Thread.Sleep(retryInterval);
                    }
                    else
                    {
                        throw new AggregateException(exceptions);
                    }
                }
            }
        }
    }
}