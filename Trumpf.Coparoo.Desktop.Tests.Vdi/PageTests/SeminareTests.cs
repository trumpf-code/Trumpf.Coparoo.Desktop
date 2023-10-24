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

namespace VDI.Tests
{
    using System.Linq;

    using Trumpf.Coparoo.Desktop.PageTests;
    using VDI.Coparoo.Desktop;

    public class SeminareTests : PageObjectTests<Seminare>
    {
        [PageTest]
        public void CheckSeminarCount()
        {
            Page.On<Seminare>().SearchFor("praxis qualitätssicherung");
            Await(() => Page.On<Seminare>().SeminarZeilen.Count()).WaitFor(c => c == 3, "3 seminars in list");
        }
    }
}