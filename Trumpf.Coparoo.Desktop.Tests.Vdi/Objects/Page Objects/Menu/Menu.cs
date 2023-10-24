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
    using System.Linq;

    using ControlObjects;
    using Trumpf.Coparoo.Desktop;
    using Trumpf.Coparoo.Desktop.Extensions;
    using Trumpf.Coparoo.Desktop.Web;

    public class Menu : PageObject, IChildOf<VDI>
    {
        protected override Search SearchPattern
            => Search.ByClassName("vdi-header");

        protected override int ControlSearchDepth
            => 4;

        protected override void AutoGoto()
        {
            Parent.Goto();
            ScrollTo();
        }

        public IButton[] Buttons
            => FindAll<Button>().OrderPagesFromLeftToRight().ToArray();

        public IButton Logo
            => Buttons.ElementAt(0);

        public IButton FortbildungsZentrum
            => Buttons.ElementAt(1);

        public IButton IngenieursVerein
            => Buttons.ElementAt(2);

        public IButton LandesVerband
            => Buttons.ElementAt(3);
    }
}