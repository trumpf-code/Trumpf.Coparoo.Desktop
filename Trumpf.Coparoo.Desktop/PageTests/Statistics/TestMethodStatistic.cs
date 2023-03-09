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

namespace Trumpf.Coparoo.Desktop.PageTests.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Test methods statistic class.
    /// </summary>
    public class TestMethodStatistic
    {
        private bool? success = null;

        /// <summary>
        /// Gets or sets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary>
        /// Gets the page test attribute.
        /// </summary>
        public PageTestAttribute PageTestAttribute
        {
            get { return MethodInfo.GetCustomAttribute<PageTestAttribute>(true); }
        }

        /// <summary>
        /// Gets or sets the test start time.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets the test stop time.
        /// </summary>
        public DateTime Stop { get; private set; }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Gets a value indicating whether the success state was set.
        /// </summary>
        public bool SuccessHasValue
        {
            get { return success.HasValue; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the test was successful.
        /// </summary>
        public bool Success
        {
            get
            {
                if (!success.HasValue)
                {
                    throw new Exception("success not yet set");
                }

                return success.Value;
            }

            set
            {
                // false remains false
                // true can become false
                // no value can become any value
                if (!success.HasValue || (success.Value && !value))
                {
                    success = value;
                }

                Stop = DateTime.Now;
            }
        }

        /// <summary>
        /// Gets or sets the page type.
        /// </summary>
        public Type PageType { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            List<string> details = new List<string>() { "Test: " + MethodInfo.Name, "Info: " + Info };
            details.AddRange(!SuccessHasValue ? new List<string>() : new List<string>() { "Success: " + Success.ToString(), "Start: " + Start.ToLongTimeString(), "Stop: " + Stop.ToLongTimeString(), "Duration: " + (Stop - Start).TotalSeconds.ToString("0.00") + " seconds" });
            return string.Join("\\n", details);
        }
    }
}