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

namespace Trumpf.Coparoo.Desktop.Nodes
{
    using System.Drawing;

    using SmartBear.TestLeft;
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// The node interface
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Gets the node representing this tree node in the UI.
        /// </summary>
        IControl Node { get; }

        /// <summary>
        /// Gets the node representing this tree node in the UI, or null if not found
        /// </summary>
        IControl TryNode { get; }

        /// <summary>
        /// Gets the image for the node
        /// </summary>
        Image Image { get; }

        /// <summary>
        /// Gets the search patter used to locate the node starting from the root
        /// </summary>
        ISearchPattern RootToNodeSearchPattern { get; }

        /// <summary>
        /// Gets the UI sub-tree rooted in this page object
        /// </summary>
        /// <param name="depth">The tree depth</param>
        /// <returns>The tree</returns>
        string Tree(int depth);

        /// <summary>
        /// Initialize this object.
        /// The parent node is used to search nodes without, hence disabling any caching.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <returns>This object.</returns>
        INode Init(INode parent);

        /// <summary>
        /// Reset the TestExecute driver
        /// </summary>
        /// <param name="restartTestExecuteProcesses">Whether to restart all TestExecute related processes</param>
        void ResetDriver(bool restartTestExecuteProcesses);
    }
}