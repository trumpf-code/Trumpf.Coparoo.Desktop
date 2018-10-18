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

namespace Trumpf.Coparoo.Desktop.WinForms.Controls
{
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Checkbox control object.
    /// </summary>
    public class CheckBox : ViewControlObject<System.Windows.Forms.CheckBox>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CheckBox"/> is checked.
        /// </summary>
        public bool Checked
        {
            get
            {
                return Node.Cast<ICheckBox>().wState == CheckState.Checked;
            }
            set
            {
                if (value != Checked)
                {
                    Click();
                }
            }
        }
    }
}