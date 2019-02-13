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

#if DEBUG
namespace Trumpf.Coparoo.Desktop.Tests.DialogWaitFor
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using Waiting;

    /// <summary>
    /// Dialog wait for tests
    /// </summary>
    [TestFixture]
    public class DialogWaitForManualTests
    {
        /// <summary>
        /// Wait with action text.
        /// </summary>
        [Test]
        public void DialogWaitForTenActions()
            => DialogWait.ForAction(actionText(10), expectationText(10));

        /// <summary>
        /// Wait with action text.
        /// </summary>
        [Test]
        public void DialogWaitForThreeActions()
            => DialogWait.ForAction(actionText(3), expectationText(3));

        /// <summary>
        /// Wait with action text.
        /// </summary>
        [Test]
        public void DialogWaitForTwoActions()
            => DialogWait.ForAction(actionText(2), expectationText(2));

        /// <summary>
        /// Wait with action text.
        /// </summary>
        [Test]
        public void DialogWaitForOneAction()
            => DialogWait.ForAction(actionText(1), expectationText(1));

        private string actionText(int i)
            => text("action", i);

        private string expectationText(int i)
            => text("expectation", i);

        private string text(string t, int i) => Enumerable.Range(0, i - 1)
            .Select(e => $"{t} {e}")
            .Aggregate(t, (a, b) => a + Environment.NewLine + b);
    }
}
#endif