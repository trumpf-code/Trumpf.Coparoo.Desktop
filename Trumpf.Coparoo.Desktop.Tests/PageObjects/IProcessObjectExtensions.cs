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

namespace Trumpf.Coparoo.Desktop.Tests.Framework
{
    using NUnit.Framework;
    using System;
    using Trumpf.Coparoo.Desktop.Extensions;

    /// <summary>
    /// Process object extensions tests
    /// </summary>
    [TestFixture]
    public class IProcessObjectExtensionsTests
    {
        /// <summary>
        /// Hard init retrying on true exception predicate
        /// </summary>
        [Test]
        public void IfHardInitIsUsedWithTrueExceptionPredicate_ThenRecoveryActionIsCalledSixTimesOnFault()
        {
            IProcessObject oneProcessObject = new ProcessObject();
            int tryCnt = 0;
            try
            {
                oneProcessObject.HardInit(() => { tryCnt++; throw new InvalidTimeZoneException(); }, (e) => { return true; }, 5, TimeSpan.FromMilliseconds(1));
            }
            catch (Exception)
            {
                Assert.AreEqual(tryCnt, 6);
            }
        }

        /// <summary>
        /// Hard init retrying on matching exception predicate
        /// </summary>
        [Test]
        public void IfHardInitIsUsedWithMatchingExceptionPredicate_ThenRecoveryActionIsCalledSixTimesOnFault()
        {
            IProcessObject oneProcessObject = new ProcessObject();
            int tryCnt = 0;
            try
            {
                oneProcessObject.HardInit(() => { tryCnt++; throw new InvalidTimeZoneException(); }, (e) => { return e.GetType().Equals(typeof(InvalidTimeZoneException)); }, 5, TimeSpan.FromMilliseconds(1));
            }
            catch (Exception)
            {
                Assert.AreEqual(tryCnt, 6);
            }
        }

        /// <summary>
        /// Hard init not retrying on non-matching exception predicate
        /// </summary>
        [Test]
        public void IfHardInitIsUsedWithDifferentNotMatchingExceptionPredicate_ThenRecoveryActionIsCalledOnceOnFault()
        {
            IProcessObject oneProcessObject = new ProcessObject();
            int tryCnt = 0;
            try
            {
                oneProcessObject.HardInit(() => { tryCnt++; throw new InvalidTimeZoneException(); }, (e) => { return e.GetType().Equals(typeof(InvalidOperationException)); }, 5, TimeSpan.FromMilliseconds(1));
            }
            catch (Exception)
            {
                Assert.AreEqual(tryCnt, 1);
            }
        }
    }
}
