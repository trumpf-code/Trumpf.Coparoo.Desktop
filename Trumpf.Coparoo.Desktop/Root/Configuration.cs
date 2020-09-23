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

namespace Trumpf.Coparoo.Desktop
{
    using System;

    using Stashbox;

    /// <summary>
    /// The configuration class.
    /// </summary>
    public class Configuration
    {
        private bool enableImages = false;
        private TimeSpan waitTime = TimeSpan.FromSeconds(20);
        private TimeSpan positiveWaitTime = TimeSpan.FromSeconds(2);
        private bool showWaitingDialog = true;
        private TimeSpan scrollSleep = TimeSpan.FromMilliseconds(100);
        private int maximumScrolls = 10;
        private TimeSpan scrollDetectionTimeout = TimeSpan.FromSeconds(1);
        private bool enableAutoScroll = false;
        private bool enableAutoGoto = false;
        private Action<string> logAction = null;
        private int nodeSearchDepth = 20;
        private int controlSearchDepth = 20;
        internal StashboxContainer resolver = new StashboxContainer();

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        internal Configuration()
        {
        }

        /// <summary>
        /// Gets the dependency registrator.
        /// </summary>
#pragma warning disable CS3003
        public IDependencyRegistrator DependencyRegistrator
#pragma warning restore CS3003
            => resolver;

        /// <inheritdoc/>
        public TimeSpan WaitTimeout
        {
            get { return waitTime; }
            set { waitTime = value; }
        }

        /// <inheritdoc/>
        public TimeSpan PositiveWaitTimeout
        {
            get { return positiveWaitTime; }
            set { positiveWaitTime = value; }
        }

        /// <inheritdoc/>
        public bool ShowWaitingDialog
        {
            get { return showWaitingDialog; }
            set { showWaitingDialog = value; }
        }

        /// <inheritdoc/>
        public int PageObjectSearchDepth
        {
            get { return nodeSearchDepth; }
            set { nodeSearchDepth = value; }
        }

        /// <inheritdoc/>
        public int ControlSearchDepth
        {
            get { return controlSearchDepth; }
            set { controlSearchDepth = value; }
        }

        /// <inheritdoc/>
        public bool EnableImages
        {
            get { return enableImages; }
            set { enableImages = value; }
        }

        /// <inheritdoc/>
        public TimeSpan ScrollSleep
        {
            get { return scrollSleep; }
            set { scrollSleep = value; }
        }

        /// <inheritdoc/>
        public int MaximumScrolls
        {
            get { return maximumScrolls; }
            set { maximumScrolls = value; }
        }

        /// <inheritdoc/>
        public TimeSpan ScrollDetectionTimeout
        {
            get { return scrollDetectionTimeout; }
            set { scrollDetectionTimeout = value; }
        }

        /// <summary>
        /// Gets or set a value indicating whether to automatically scroll to a control object if it is not visible on screen when an operation is issued on it.
        /// </summary>
        public bool EnableAutoScroll
        {
            get { return enableAutoScroll; }
            set { enableAutoScroll = value; }
        }

        /// <summary>
        /// Gets or set a value indicating whether to automatically call goto on the parent page object it a page object is not visible on screen.
        /// </summary>
        public bool AutoGoto
        {
            get { return enableAutoGoto; }
            set { enableAutoGoto = value; }
        }

        /// <summary>
        /// Gets or set log action. The default is Trace.WriteLine.
        /// </summary>
        public Action<string> LogAction
        {
            get { return logAction ?? (line => System.Diagnostics.Trace.WriteLine(line)); }
            set { logAction = value; }
        }    
    }
}
