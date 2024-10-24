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

namespace Trumpf.Coparoo.Desktop.Core.Waiting
{
    using System;

    using Exceptions;
    using Trumpf.Coparoo.Desktop.Waiting;

    /// <summary>
    /// Object retrieval with throwing and non-throwing waiting methods.
    /// </summary>
    public class Await<T> : IAwait<T>
    {
        private Func<T> getValue;
        private string name;
        private Type owner;
        private Func<TimeSpan> positiveTimeout;
        private Func<TimeSpan> waitTimeout;
        private Func<bool> showDialog;
        private IDialogWaiter dialogWaiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Await{T}"/> class.
        /// </summary>
        /// <param name="getValue">The function to evaluate for truth.</param>
        /// <param name="name">The property name for logging purposes.</param>
        /// <param name="owner">The owner type.</param>
        /// <param name="waitTimeout">The pollingPeriod.</param>
        /// <param name="positiveTimeout">The positive pollingPeriod for dialog-waits.</param>
        /// <param name="showDialog">Whether to show a dialog while waiting.</param>
        /// <param name="dialogWaiter">The dialog waiter to use.</param>
        internal Await(Func<T> getValue, string name, Type owner, Func<TimeSpan> waitTimeout, Func<TimeSpan> positiveTimeout, Func<bool> showDialog, IDialogWaiter dialogWaiter)
        {
            this.getValue = getValue;
            this.name = name;
            this.owner = owner;
            this.positiveTimeout = positiveTimeout;
            this.waitTimeout = waitTimeout;
            this.showDialog = showDialog;
            this.dialogWaiter = dialogWaiter;
        }

        /// <summary>
        /// Gets a fresh value evaluation.
        /// </summary>
        public T Value => getValue();

        /// <summary>
        /// Implicit conversion to the underlying type.
        /// </summary>
        /// <param name="other">The value to convert.</param>
        public static implicit operator T(Await<T> other)
        {
            return other.Value;
        }

        #region TryWait
        /// <summary>
        /// Try wait until the page object is visible on screen .
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitFor(Predicate<T> expectation)
        {
            return TryWait.For(() => expectation(Value), waitTimeout());
        }

        /// <summary>
        /// Try wait until the page object is visible on screen .
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <param name="timeout">The pollingPeriod.</param>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitFor(Predicate<T> expectation, TimeSpan timeout)
        {
            return TryWait.For(() => expectation(Value), timeout);
        }
        #endregion

        #region Wait
        /// <summary>
        /// Wait until the property evaluates to <c>true</c>.
        /// Show a dialog.
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <param name="expectationText">Expectation text, i.e. the predicate expressed as a human-readable text.</param>
        public void WaitFor(Predicate<T> expectation, string expectationText)
        {
            WaitFor(expectation, expectationText, waitTimeout());
        }

        /// <summary>
        /// Wait until the property evaluates to <c>true</c>.
        /// Show a dialog.
        /// </summary>
        /// <param name="expectation">Expectation predicate.</param>
        /// <param name="expectationText">Expectation text, i.e. the predicate expressed as a human-readable text.</param>
        /// <param name="timeout">The pollingPeriod.</param>
        public void WaitFor(Predicate<T> expectation, string expectationText, TimeSpan timeout)
        {
            string message = $"{name} in {owner.Name}: {expectationText}";
            if (showDialog() && dialogWaiter != null)
            {
                dialogWaiter.WaitFor(() => Value, value => expectation(value), message, timeout, positiveTimeout(), TimeSpan.FromMilliseconds(100));
            }
            else
            {
                if (!TryWaitFor(expectation, timeout))
                {
                    throw new WaitForException(owner, message);
                }
            }
        }
        #endregion

        #region Others
        /// <summary>
        /// Gets the value as string.
        /// </summary>
        /// <returns>The value.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
        #endregion
    }
}