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

namespace Trumpf.Coparoo.Desktop.Core
{
    using SmartBear.TestLeft.TestObjects;
    
    /// <summary>
    /// Interface for the process object node class.
    /// </summary>
    public interface IProcessObjectNode : IRootObjectNode
    {
        /// <summary>
        /// Gets the process.
        /// </summary>
        IProcess Process { get; }

        /// <summary>
        /// Gets the process name.
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// Initializes the process object node.
        /// The process name is ignored if a process has been set.
        /// </summary>
        /// <param name="processName">The process name.</param>
        void Init(string processName);

        /// <summary>
        /// Initializes the node with the process search pattern.
        /// </summary>
        /// <param name="process">The process.</param>
        void Init(IProcess process);
    }
}