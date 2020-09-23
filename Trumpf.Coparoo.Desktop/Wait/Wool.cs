﻿// Copyright 2016, 2017, 2018, 2019, 2020 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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

namespace Trumpf.Coparoo.Desktop.Waiting
{
    using System;

    /// <summary>
    /// Boolean equipped with throwing and non-throwing waiting methods.
    /// </summary>
    public class Wool
    {
        private IAwait<bool> waiter;
        private Func<bool> snap;
        private Action unsnap;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wool"/> class.
        /// </summary>
        /// <param name="waiter">The bool to wait for.</param>
        /// <param name="snap">Snap the object.</param>
        /// <param name="unsnap">Unsnap the object.</param>
        internal Wool(IAwait<bool> waiter, Func<bool> snap, Action unsnap)
        {
            this.waiter = waiter;
            this.snap = snap;
            this.unsnap = unsnap;
        }

        /// <summary>
        /// Gets a fresh value evaluation.
        /// </summary>
        public bool Value => waiter.Value;

        /// <summary>
        /// Implicit conversion to the underlying type.
        /// </summary>
        /// <param name="other">The value to convert.</param>
        public static implicit operator bool(Wool other)
        {
            return other.Value;
        }

        #region TryWait
        /// <summary>
        /// Try wait until the page object is visible on screen .
        /// </summary>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitFor()
        {
            return waiter.TryWaitFor(value => value);
        }

        /// <summary>
        /// Try wait until the page object is visible on screen .
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitFor(TimeSpan timeout)
        {
            return waiter.TryWaitFor(value => value, timeout);
        }

        /// <summary>
        /// Try wait until the page object is not visible on screen .
        /// </summary>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitForFalse()
        {
            return ActSnapped(() => waiter.TryWaitFor(value => !value));
        }

        /// <summary>
        /// Try wait until the page object is not visible on screen .
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Whether the page object is visible on screen.</returns>
        public bool TryWaitForFalse(TimeSpan timeout)
        {
            return ActSnapped(() => waiter.TryWaitFor(value => !value, timeout));
        }
        #endregion

        #region Wait
        /// <summary>
        /// Wait until the property evaluates to true.
        /// </summary>
        public void WaitFor()
        {
            waiter.WaitFor(value => value, "is true");
        }

        /// <summary>
        /// Wait until the property evaluates to true.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void WaitFor(TimeSpan timeout)
        {
            waiter.WaitFor(value => value, "is true", timeout);
        }

        /// <summary>
        /// Wait until the page object is not visible on screen anymore.
        /// </summary>
        public void WaitForFalse()
        {
            ActSnapped(() => waiter.WaitFor(value => !value, "is false"));
        }

        /// <summary>
        /// Wait until the page object is not visible on screen anymore.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void WaitForFalse(TimeSpan timeout)
        {
            ActSnapped(() => waiter.WaitFor(value => !value, "is false", timeout));
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Execute an action in snapped context.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void ActSnapped(Action action)
        {
            ActSnapped(() =>
            {
                action();
                return true;
            });
        }

        /// <summary>
        /// Execute a function in snapped context.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <returns>The function result.</returns>
        private bool ActSnapped(Func<bool> function)
        {
            var result = snap() ? function() : true;
            unsnap();
            return result;
        }

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