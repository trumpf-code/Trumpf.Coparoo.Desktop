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
    using System;

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Base class for root nodes, i.e. nodes in the UI tree that are located from a process.
    /// </summary>
    public class ProcessObjectNode : RootObjectNode, IProcessObjectNode
    {
        private string processName = null;
        private IProcess process = null;

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root.
        /// </summary>
        public override ISearchPattern SearchPattern
        {
            get { return new ProcessPattern { ProcessName = ProcessName }; }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// It's the same as the root process.
        /// </summary>
        public override IControl Root
        {
            get { return Process.Cast<IControl>(); }
        }

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found
        /// It's the same as the root process.
        /// </summary>
        public override IControl TryRoot
        {
            get
            {
                var result = TryProcess;
                return result == null ? null : result.Cast<IControl>();
            }
        }

        /// <summary>
        /// Gets the process.
        /// </summary>
        public IProcess Process
        {
            get { return process ?? Driver.Find<IProcess>(SearchPattern); }
        }

        /// <summary>
        /// Gets the process name.
        /// </summary>
        public virtual string ProcessName
        {
            get { return processName; }
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        protected override IObject Parent
        {
            get { throw new InvalidOperationException("A process node has no parent"); }
        }

        /// <summary>
        /// Gets the process or return null, if it cannot be found.
        /// </summary>
        protected IProcess TryProcess
        {
            get
            {
                IProcess result;
                return process ?? (Driver.TryFind(SearchPattern, out result) ? result : null);
            }
        }

        /// <summary>
        /// Gets the root node if possible, and otherwise return null.
        /// </summary>
        protected override IObjectTreeNode TryParent
        {
            get { throw new InvalidOperationException("A process node has no parent"); }
        }

        /// <summary>
        /// Initializes the process object node.
        /// </summary>
        /// <param name="processName">The process name.</param>
        public void Init(string processName)
        {
            if (process != null)
            {
                throw new InvalidOperationException("A process has already been set.");
            }

            this.processName = processName;
        }

        /// <summary>
        /// Initializes the process object node.
        /// </summary>
        /// <param name="process">The process.</param>
        public void Init(IProcess process)
        {
            if (processName != null)
            {
                throw new InvalidOperationException("A process name has already been set.");
            }

            this.process = process;
        }
    }
}