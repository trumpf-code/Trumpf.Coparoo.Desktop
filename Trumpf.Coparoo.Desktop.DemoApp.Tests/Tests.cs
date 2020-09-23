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

#if DEBUG
namespace Trumpf.Coparoo.Desktop.DemoApp.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Coparoo.Desktop;
    using Coparoo.Desktop.PageTests;

    [TestClass]
    public class Tests
    {
        static IDemoApp app;

        [TestInitialize]
        public void Init()
        {
            if (app == null)
            {
                var pageObjectImpl = Path.GetFullPath(@".\Trumpf.Coparoo.Desktop.DemoApp.exe");
                System.Reflection.Assembly.LoadFile(pageObjectImpl);
                app = ProcessObject.Resolve<IDemoApp>();
            }
        }

        [TestMethod]
        public void DerivedButtons()
        {
            var m = app.On<IMainWindow>();
            m.VisibleOnScreen.WaitFor();

            var b = m.Buttons;

            foreach (var a in b)
            {
                a.Click();
                Thread.Sleep(1000);
            }

            Assert.AreEqual(3, b.Count());
        }

        [TestMethod]
        public void ClickTypedInterfaceButton()
        {
            IDemoApp app = ProcessObject.Resolve<IDemoApp>();
            app.Configuration.WaitTimeout = TimeSpan.FromMinutes(1);
            app.Configuration.PositiveWaitTimeout = TimeSpan.FromMilliseconds(500);
            app.On<IMainWindow>().VisibleOnScreen.WaitFor(TimeSpan.FromMinutes(1));
            app.On<IMainWindow>().TypedButton.Click();
        }

        [TestMethod]
        public void ASimpleUiTest()
        {
            IDemoApp app = ProcessObject.Resolve<IDemoApp>();
            app.Configuration.WaitTimeout = TimeSpan.FromMinutes(1);
            app.Configuration.PositiveWaitTimeout = TimeSpan.FromMilliseconds(500);
            app.On<IMainWindow>().VisibleOnScreen.WaitFor(TimeSpan.FromMinutes(1));
            app.On<IMainWindow>().ResetButton.Click();
            app.On<IMainWindow>().IncrementButton.Text.WaitFor(text => text == "0", "Text is '0'");

            foreach (var i in Enumerable.Range(1, 4))
            {
                app.On<IMainWindow>().IncrementButton.Click();
                app.On<IMainWindow>().IncrementButton.Text.WaitFor(text => text == i.ToString(), "Caption should be " + i);
            }

            app.On<IMainWindow>().ResetButton.Click();
            app.On<IMainWindow>().IncrementButton.Text.WaitFor(text => text == "0", "Text is '0'");
        }

        [TestMethod]
        public void TestOfTests()
        {
            var app = ProcessObject.Resolve<IDemoApp>();
            app.TestBottomUp();
            app.WritePdfTree("TestBottomUp.pdf");
        }

        [TestMethod]
        public void MainWindowTests()
        {
            var app = ProcessObject.Resolve<IDemoApp>();
            app.Goto<IMainWindow>().Test();
            app.WritePdfTree();
        }
    }
}
#endif