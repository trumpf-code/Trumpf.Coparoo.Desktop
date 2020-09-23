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

namespace Trumpf.Coparoo.Desktop.Exceptions
{
    using System;

    /// <summary>
    /// Page object not found exception class.
    /// </summary>
    /// <typeparam name="TPageObject">The page object type.</typeparam>
    public class PageObjectNotFoundException<TPageObject> : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageObjectNotFoundException{TPageObject}"/> class.
        /// </summary>
        public PageObjectNotFoundException() 
            : base("Page object " + typeof(TPageObject).ToString() + " not found in the page object tree; is the page object correctly tagged with the " + typeof(IChildOf<>).Name + " interface?")
        {
        }
    }
}