// Copyright 2016, 2017, 2018, 2019, 2020 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System.Collections.Generic;
    using System.Linq;

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// A restricted search pattern.
    /// </summary>
    /// <typeparam name="TControlPattern">The underlying control pattern type.</typeparam>
    public abstract class Search<TControlPattern> : ISearchPattern where TControlPattern : ControlPattern, new()
    {
        #region Fields and properties
        private TControlPattern pattern = new TControlPattern();

        /// <summary>
        /// Gets the control pattern.
        /// </summary>
        protected TControlPattern Pattern
        {
            get { return pattern; }
        }
        #endregion

        /// <summary>
        /// Set the enabled constraint to true.
        /// </summary>
        /// <returns>The search pattern.</returns>
        public Search<TControlPattern> AndIsEnabled()
        {
            Pattern.Enabled = true;
            return this;
        }

        /// <summary>
        /// Set the visible constraint to true.
        /// </summary>
        /// <returns>The search pattern.</returns>
        public Search<TControlPattern> AndIsVisible()
        {
            Pattern.Visible = true;
            return this;
        }

        /// <summary>
        /// Get the pattern items.
        /// </summary>
        /// <returns>List of pattern items.</returns>
        public IEnumerable<KeyValuePair<string, object>> GetPatternItems()
        {
            return pattern.GetPatternItems();
        }

        /// <summary>
        /// Adds a custom property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The search pattern.</returns>
        public Search<TControlPattern> And(string name, object value)
        {
            Pattern.Add(name, value);
            return this;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            string result = string.Empty;

            string sep = string.Empty;
            foreach (var p in Pattern.GetPatternItems())
            {
                string key;
                string value;
                switch (p.Key)
                {
                    case "ClrFullClassName":
                        key = "Class";
                        value = p.Value.ToString().Split(new char[] { '.' }).Last();
                        break;
                    case "WinFormsControlName":
                    case "WPFControlName":
                        key = "ControlName";
                        value = p.Value.ToString();
                        break;
                    default:
                        key = p.Key;
                        value = p.Value.ToString();
                        break;
                }

                result += key + "=" + value + sep;
                sep = "; ";
            }

            return result;
        }

        /// <summary>
        /// Add another search constraint.
        /// </summary>
        /// <param name="additionalSearch">Additional search constraint.</param>
        /// <returns>The conjunction.</returns>
        public Search<TControlPattern> And(ISearchPattern additionalSearch)
        {
            if (additionalSearch != null)
            {
                foreach (var p in additionalSearch.GetPatternItems())
                {
                    And(p.Key, p.Value);
                }
            }

            return this;
        }

        /// <summary>
        /// Adds a constraint.
        /// </summary>
        /// <param name="initial">The initial pattern without regular expression symbols.</param>
        /// <param name="constraint">The OR-constraint to add without regular expression symbols.</param>
        /// <returns>The compound expression.</returns>
        protected string AddOrConstraint(string initial, string constraint)
        {
            if (string.IsNullOrEmpty(initial))
            {
                throw new System.InvalidOperationException("Constraint must not be empty. Use the corresponding AND method first.");
            }

            if (!initial.StartsWith("regexp:"))
            {
                initial = "regexp:(" + ToRegexString(initial) + ")";
            }

            return initial + "|" + "(" + ToRegexString(constraint) + ")";
        }

        /// <summary>
        /// Replace symbols as needed.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The string without plus and dot.</returns>
        private string ToRegexString(string text)
        {
            // + and . are special symbols
            return text.Replace("+", "\\+").Replace(".", "\\.");
        }
    }
}
