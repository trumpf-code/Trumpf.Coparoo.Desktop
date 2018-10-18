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

namespace Trumpf.Coparoo.Desktop.WinForms
{
    /// <summary>
    /// Interface for a control object for <see cref="System.Windows.Forms.Control"/>.
    /// </summary>
    public interface IControlObject : Core.IControlObject
    {
        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.Control.Name"/> of the control.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets <see cref="System.Windows.Forms.Control.Text"/> of the control.
        /// </summary>
        string Text { get; set; }
    }
}