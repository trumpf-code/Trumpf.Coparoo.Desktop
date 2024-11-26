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

namespace Trumpf.Coparoo.Desktop.Waiting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Wait helper throwing on timeout.
    /// </summary>
    internal static class Wait
    {
        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// </summary>
        /// <param name="function">The function which is executed.</param>
        public static void For(Func<bool> function)
        {
            RetryUntilSuccessOrTimeout(function, e => e, TimeSpan.FromSeconds(20));
        }

        /// <summary>
        /// Executes <paramref name="function"/> until its result is not <c>null</c>.
        /// </summary>
        /// <returns>The first result of <paramref name="function"/> which is not <c>null</c>.</returns>
        /// <param name="function">The function which is executed.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="timeout">The maximum waiting time in milliseconds.</param>
        /// <param name="retryPause">Timeout between the condition check rounds.</param>
        /// <typeparam name="T">The type of the variable.</typeparam>
        public static T RetryUntilSuccessOrTimeout<T>(Func<T> function, Predicate<T> condition, TimeSpan? timeout = null, TimeSpan? retryPause = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(20);
            retryPause = retryPause ?? TimeSpan.FromMilliseconds(100);

            Stopwatch stopwatch = Stopwatch.StartNew();
            List<T> result = new List<T>();
            do
            {
                result.Add(function());
                if (condition(result.Last()))
                {
                    return result.Last();
                }

                System.Threading.Thread.Sleep(retryPause.Value);
            }
            while (stopwatch.Elapsed < timeout);

            throw new TimeoutException(string.Format("Condition did not turn true within the maximum waiting time period of {0}s; polling results: {1}", timeout.Value.TotalSeconds, string.Join(", ", result.Select(e => e == null ? "null" : e.ToString()))));
        }
    }
}