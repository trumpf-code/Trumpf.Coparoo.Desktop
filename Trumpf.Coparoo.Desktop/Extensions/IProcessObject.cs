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
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading;

    using SmartBear.TestLeft;
    using Trumpf.Coparoo.Desktop.Waiting;

    /// <summary>
    /// Extension methods for process objects.
    /// </summary>
    public static class IProcessObjectExtensions
    {
        private static readonly string[] smartbearProcesses = new string[] { "TCHookX64", "TestExecute" };

        private const string ServiceName = "TestComplete 12 Service";

        private static readonly Predicate<Exception> defaultRetryCondition =
                e =>
                    e.Message.Contains("No more instances can be launched at the moment") ||
                    e.Message.Contains("Error code: 127") ||
                    e.Message.Contains("The server committed a protocol violation") ||
                    e.Message.Contains("The process TestExecute started, but the REST server did not start") |
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
        public static void InitializeLocalDriver(this IProcessObject processObject, int maxAttempts, TimeSpan retryInterval)
            => Do(
                () => { processObject.Driver = defaultDriverCreator(); },
                defaultRetryCondition,
                () => { },
                retryInterval,
                maxAttempts
                );

        /// <summary>
        /// Initializes the process object.
        /// </summary>
        /// <param name="processObject">The process object to initialize.</param>
        /// <param name="driverCreator">The driver creator.</param>
        /// <param name="retryOnCondition">The retry condition.</param>
        /// <param name="maxAttempts">The maximum number of retries.</param>
        /// <param name="retryInterval">The waiting time between retries.</param>
        public static void HardInit(this IProcessObject processObject, Func<IDriver> driverCreator = null, Predicate<Exception> retryOnCondition = null, int maxAttempts = 5, TimeSpan retryInterval = default)
        {
            driverCreator = driverCreator ?? defaultDriverCreator;
            retryOnCondition = retryOnCondition ?? defaultRetryCondition;
            retryInterval = retryInterval != default ? retryInterval : TimeSpan.FromMinutes(1);

            StopServiceAndKillSmartbearProcesses();

            Do(
                () => { processObject.Driver = driverCreator(); },
                retryOnCondition,
                StopServiceAndKillSmartbearProcesses,
                retryInterval,
                maxAttempts
                );
        }

        /// <summary>
        /// Retry on exception. Before retry execute recovery action.
        /// </summary>
        /// <typeparam name="E">The exception to retry on.</typeparam>
        /// <param name="action">The action to execute and potentially retry.</param>
        /// <param name="when">The exception condition.</param>
        /// <param name="recoveryAction">The recovery action to execute before a retry.</param>
        /// <param name="retryInterval">The retry interval.</param>
        /// <param name="maxAttempts">The number of attempts (1 = no retry).</param>
        private static void Do<E>(Action action, Predicate<E> when, Action recoveryAction, TimeSpan retryInterval, int maxAttempts) where E : Exception
        {
            RetryAndRecover(() => { action(); return 0; }, when, recoveryAction, retryInterval, maxAttempts);
        }

        /// <summary>
        /// Restart Test Execute
        /// </summary>
        private static void StopServiceAndKillSmartbearProcesses()
        {
            // stop test complete service
            try
            {
                ServiceController service = new ServiceController(ServiceName);
                if (service.Status != ServiceControllerStatus.Stopped && service.Status != ServiceControllerStatus.StopPending)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(1));
                }
            }
            catch (InvalidOperationException)
            {
                // service is not available
            }
            catch (Win32Exception)
            {
                // system API failure
            }
            catch (ArgumentException)
            {
                // invalid name
            }

            // kill remaining smartbear processes
            foreach (var name in smartbearProcesses)
            {
                foreach (var process in Process.GetProcessesByName(name))
                {
                    try
                    {
                        process.CloseMainWindow();
                        Thread.Sleep(1000);

                        if (!process.HasExited)
                        {
                            process.Kill();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }

                Wait.For(() => Process.GetProcessesByName(name).Count() == 0);
            }
        }

        /// <summary>
        /// Retry on exception. Before retry execute recovery action.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <typeparam name="E">The exception to retry on.</typeparam>
        /// <param name="function">The action to execute and potentially retry.</param>
        /// <param name="when">The exception condition.</param>
        /// <param name="recoveryAction">The recovery action to execute before a retry.</param>
        /// <param name="retryInterval">The retry interval.</param>
        /// <param name="maxAttempts">The number of attempts (1 = no retry).</param>
        /// <returns>The result.</returns>
        private static T RetryAndRecover<T, E>(Func<T> function, Predicate<E> when, Action recoveryAction, TimeSpan retryInterval, int maxAttempts) where E : Exception
        {
            int remainingAttempts = maxAttempts;
            var exceptions = new List<Exception>();

            while (true)
            {
                try
                {
                    return function();
                }
                catch (E e)
                {
                    exceptions.Add(e);
                    if (remainingAttempts-- > 0 && (when == null || when(e)))
                    {
                        Thread.Sleep((int)retryInterval.TotalMilliseconds / 2);
                        recoveryAction?.Invoke();
                        Thread.Sleep((int)retryInterval.TotalMilliseconds / 2);
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