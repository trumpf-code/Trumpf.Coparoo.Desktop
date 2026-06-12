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

namespace Trumpf.Coparoo.Desktop.Core
{
    using System;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Helper methods for targeted stale TestLeft object recovery.
    /// </summary>
    internal static class StaleObjectRecovery
    {
        private const string InvokedObjectIsMissing = "The invoked object is missing";

        /// <summary>
        /// Determine whether the exception is caused by a removed TestLeft object node.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>Whether the exception represents a stale TestLeft node.</returns>
        internal static bool IsStaleObjectException(Exception exception)
        {
            return exception is InvocationException && exception.Message.Contains(InvokedObjectIsMissing);
        }

        /// <summary>
        /// Write a recovery log message without letting logging failures affect UI automation.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="message">The message.</param>
        internal static void Log(Configuration configuration, string message)
        {
            try
            {
                configuration.LogAction(message);
            }
            catch
            {
                // Logging must not change automation behavior.
            }
        }
    }
}