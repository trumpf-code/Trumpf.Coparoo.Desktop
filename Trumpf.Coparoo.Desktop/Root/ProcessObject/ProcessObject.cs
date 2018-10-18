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

namespace Trumpf.Coparoo.Desktop
{
    using Core;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Root process object class for any UI technology (WPF, WinForms etc.).
    /// </summary>
    public class ProcessObject : RootObject<ProcessObjectNode>, IProcessObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessObject"/> class.
        /// </summary>
        public ProcessObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessObject"/> class.
        /// </summary>
        /// <param name="value">The process node.</param>
        public ProcessObject(IProcess value)
        {
            base.Node.Init(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessObject"/> class.
        /// </summary>
        /// <param name="value">The process name.</param>
        public ProcessObject(string value)
        {
            base.Node.Init(value);
        }

        /// <inheritdoc/>
        public new IProcessObjectNode Node => base.Node;

        /// <inheritdoc/>
        protected override bool IsVisibleOnScreen
        {
            get { return false; }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected string ProcessName
        {
            get { return Node.ProcessName; }
            set { base.Node.Init(value); }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected IProcess Process
        {
            get { return Node.Process; }
            set { base.Node.Init(value); }
        }

        /// <inheritdoc/>
        internal override bool UiNodeExists => base.Node.TryRoot != null;
    }
}