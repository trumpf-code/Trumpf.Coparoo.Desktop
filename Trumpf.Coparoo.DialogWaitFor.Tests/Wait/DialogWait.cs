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

namespace Trumpf.Coparoo.Desktop.Tests.DialogWaitFor
{
    using System;
    using System.Linq;

    using Exceptions;
    using NUnit.Framework;
    using Waiting;

    /// <summary>
    /// Dialog wait for tests
    /// </summary>
    [TestFixture]
    public class DialogWaitForTests
    {
        private readonly TimeSpan @long = TimeSpan.FromSeconds(2);
        private readonly TimeSpan medium = TimeSpan.FromSeconds(1);
        private readonly TimeSpan none = TimeSpan.FromSeconds(0);
        private readonly TimeSpan @short = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Positive case
        /// </summary>
        [Test]
        public void IfTheConditionIsTrue_ThenNoExceptionIsThrown_FastContinue()
            => DialogWait.For(() => true, "Empty", @long, TimeSpan.FromSeconds(0), @short);

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsTrue_ThenNoExceptionIsThrown_ClickThrough()
            => DialogWait.For(() => true, "Door is closed", @long, @long, @short, true);

        /// <summary>
        /// Positive case
        /// </summary>
        [Test]
        public void IfTheConditionIsTrue_ThenNoExceptionIsThrown()
            => DialogWait.For(() => true, "Empty", @long, medium, @short);

        /// <summary>
        /// Positive case
        /// </summary>
        [Test]
        public void LongExpectationText_IsShownInTwoLines()
            => DialogWait.For(() => true, "We wait until the expected condition turns to true, so we can continue with the test.", @long, medium, @short);

        /// <summary>
        /// If the condition is false, exception is thrown with exception message.
        /// </summary>
        [Test]
        public void IfTheConditionIsFalse_ExceptionIsThrown_WithExceptionMessage()
        {
            Assert.Throws<DialogWaitForTimeoutException>(() =>
            {
                try
                {
                    DialogWait.For(() => false, "Order list is empty", @long, medium, @short);
                }
                catch (DialogWaitForTimeoutException e)
                {
                    Assert.AreEqual("Timeout of " + @long.TotalSeconds.ToString("0.00") + " seconds exceeded when waiting for 'Order list is empty'", e.Message);
                    throw;
                }
            });
        }

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsNull_ThenTimeout()
            => Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(null, "Empty", @long, medium, @short));

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsFalse_ThenTimeout()
            => Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => false, "Empty", @long, medium, @short));

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsFalse_ThenTimeout_LazyCondition()
        {
            int i = 0;
            Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => { System.Threading.Thread.Sleep(1000); return i++; }, j => j == 2, "Empty", @long, medium, @short));
        }

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsFalse_ThenTimeout_ZeroPollingTime()
            => Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => false, "Empty", @long, medium, TimeSpan.Zero));

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsFalseAndMaxPositiveTimeout_ThenTimeout()
            // no negative timeout should be displayed on the UI
            => Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => false, "Empty", @long, TimeSpan.MaxValue, @short));

        /// <summary>
        /// Positive case
        /// </summary>
        [Test]
        public void IfTheConditionIsTrueAndMaxNegativeTimeout_ThenNoExceptionIsThrown()
            // no negative timeout should be displayed on the UI
            => DialogWait.For(() => true, "Empty", TimeSpan.MaxValue, @long, @short);

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsTrue_ThenNoExceptionIsThrown_TimeoutBeforeFirstPoll()
            => Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => false, "Empty", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsTrue_ThenNoExceptionIsThrown_ZeroTimes()
            => Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => false, "Empty", TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0)));

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsTrue_ThenNoExceptionIsThrown_NegativeTimes()
            => Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => false, "Empty", TimeSpan.FromSeconds(-10), TimeSpan.FromSeconds(-10), TimeSpan.FromSeconds(-10)));

        /// <summary>
        /// Positive case
        /// </summary>
        [Test]
        public void Repeat_IfTheConditionIsTrue_ThenNoExceptionIsThrown()
            => Enumerable.Range(0, 50).ToList().ForEach(_ => DialogWait.For(() => true, "Empty", @long, none, @short));

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsFlipsBad_ThenTimeout()
        {
            bool b = false;
            Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => (b = !b), "Empty", @long, medium, TimeSpan.FromMilliseconds(500)));
        }

        /// <summary>
        /// Nested wait
        /// </summary>
        [Test]
        public void IfTheNestedConditionsAreTrue_ThenNoExceptionIsThrown()
        {
            DialogWait.For(
                () =>
                {
                    try
                    {
                        DialogWait.For(() => true, "sub wait", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(100));
                        System.Threading.Thread.Sleep(250);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                "main wait",
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(1000));
        }

        /// <summary>
        /// Negative case
        /// </summary>
        [Test]
        public void IfTheConditionIsFlipsBad_ThenTimeout_ShowCurrentValue()
        {
            int i = 0;
            Assert.Throws<DialogWaitForTimeoutException>(() => DialogWait.For(() => ++i, v => v % 2 == 0, "Even", medium, medium, @short, false, null));
        }

        /// <summary>
        /// Wait with action text.
        /// </summary>
        [Test]
        public void IfTheConditionIsTrue_ThenNoExceptionIsThrown_WithActionText()
        {
            int exp = 10;
            int i = 0;
            DialogWait.ForAction("don't do anything", () => i != exp ? i++ : i, value => value == exp, $"value is {exp}");
        }
    }
}
