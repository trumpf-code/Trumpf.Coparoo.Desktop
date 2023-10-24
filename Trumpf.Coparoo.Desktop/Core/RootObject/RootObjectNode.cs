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
    /// Root page object node class.
    /// </summary>
    public abstract class RootObjectNode : PageObjectNodeBase, IRootObjectNode, IRootObjectNodeInternal
    {
        private NodeLocator nodeLocator = new NodeLocator();
        private Configuration configuration = new Configuration();
        private Statistics statistics = new Statistics();

        /// <summary>
        /// Gets the node locator.
        /// </summary>
        INodeLocator IRootObjectNodeInternal.NodeLocator
        {
            get { return nodeLocator; }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Configuration Configuration
        {
            get { return configuration; }
        }

        /// <summary>
        /// Gets the statistics.
        /// </summary>
        public Statistics Statistics
        {
            get { return statistics; }
        }

        /// <summary>
        /// Gets or sets the TestExecute driver.
        /// </summary>
        public IDriver Driver
        {
            get { return Core.Driver.Value; }
            set { Core.Driver.Value = value; }
        }
    }
}