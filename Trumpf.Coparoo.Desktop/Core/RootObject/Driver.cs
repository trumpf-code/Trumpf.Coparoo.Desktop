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
    using SmartBear.TestLeft;

    /// <summary>
    /// The single TestExecute driver.
    /// </summary>
    internal static class Driver
    {
        private static IDriver driver;

        /// <summary>
        /// Gets or sets the TestExecute driver.
        /// </summary>
        public static IDriver Value
        {
            get { return driver ?? (driver = DefaultDriver); }
            set { driver = value; }
        }

        /// <summary>
        /// Gets the default TestExecute driver.
        /// </summary>
        private static IDriver DefaultDriver
        {
            get
            {
                var result = new LocalDriver();
                result.Options.UIAutomation.AddWindow("*");
                return result;
            }
        }
    }
}