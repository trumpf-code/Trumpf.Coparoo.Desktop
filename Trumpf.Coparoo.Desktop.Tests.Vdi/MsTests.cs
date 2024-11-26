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

#if DEBUG
namespace VDI.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Trumpf.Coparoo.Desktop.Waiting;
    using Trumpf.Coparoo.Desktop.Web;
    using Trumpf.Coparoo.Waiting;
    using VDI.Coparoo.Desktop;

    [TestClass]
    public class WebPageAutomationTests
    {
        [TestMethod]
        public void TestLeftVdiTestWithExplicitNavigation()
        {
            VDIWebPage vdi = new VDIWebPage();
            vdi.Open();
            vdi.On<VDI>().VisibleOnScreen.WaitFor();
            vdi.On<Menu>().FortbildungsZentrum.Click();
            vdi.On<FortbildungsZentrum>().WeitereSeminare.Click();
            vdi.On<Seminare>().Volltextsuche.ScrollTo();
            vdi.On<Seminare>().Volltextsuche.Content = "praxis qualitätssicherung";
            vdi.On<Seminare>().SucheAnzeigen.Click();
            ConditionDialog.For(() => vdi.On<Seminare>().SeminarZeilen.Count(), c => c == 3, "3 seminars in list", TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1), true, null);
        }

        [TestMethod]
        public void TestLeftVdiTestWithImplicitNavigation()
        {
            VDIWebPage vdi = new VDIWebPage();
            vdi.Goto<Seminare>().SearchFor("praxis qualitätssicherung");
            ConditionDialog.For(() => vdi.On<Seminare>().SeminarZeilen.Count(), c => c == 3, "3 seminars in list");
        }

        [TestMethod]
        public void TestLeftAbstractVdiTestWithImplicitNavigation()
        {
            IVDIWebPage vdi = WebPageObject.Resolve<IVDIWebPage>();
            vdi.Goto<ISeminare>().SearchFor("praxis qualitätssicherung");
            ConditionDialog.For(() => vdi.On<ISeminare>().SeminarZeilen.Count(), c => c == 3, "3 seminars in list");
            vdi.WritePdfTree();
        }

        [TestMethod]
        public void TestLeftCreatePageObjectTree()
            => new VDIWebPage().WritePdfTree();
    }
}
#endif