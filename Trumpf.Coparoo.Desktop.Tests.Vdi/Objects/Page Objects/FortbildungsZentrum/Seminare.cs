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

namespace VDI.Coparoo.Desktop
{
    using System.Collections.Generic;

    using ControlObjects;
    using Trumpf.Coparoo.Desktop;
    using Trumpf.Coparoo.Desktop.Web;

    public class Seminare : PageObject, IChildOf<FortbildungsZentrum>
    {
        protected override Search SearchPattern
            => Search.ByClassName("vdi-event-list-view");

        protected override void AutoGoto()
        {
            Parent.Goto();
            On<FortbildungsZentrum>().WeitereSeminare.Click();
        }

        public IButton SucheAnzeigen
            => this.Find<Button>(Search.ByContentText("Suche anzeigen"));

        public IInputBox Volltextsuche
            => this.Find<InputBox>();

        public IEnumerable<ISeminarZeile> SeminarZeilen
            => this.FindAll<SeminarZeile>();

        public void SearchFor(string searchText)
        {
            Volltextsuche.ScrollTo();
            Volltextsuche.Content = searchText;
            SucheAnzeigen.Click();
        }
    }
}