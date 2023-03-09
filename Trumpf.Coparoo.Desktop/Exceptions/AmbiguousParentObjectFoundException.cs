﻿// Copyright 2016 - 2023 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System.Collections.Generic;

    /// <summary>
    /// Ambiguous parent object found exception class.
    /// </summary>
    public class AmbiguousParentObjectFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousParentObjectFoundException"/> class.
        /// </summary>
        public AmbiguousParentObjectFoundException(IPageObject parentToFind, Type similarParent, Type childOfSimilarParent)
            : base($"Error when searching for page object '{parentToFind.GetType().Name}' in location '{parentToFind.GetType().Assembly.Location}' in the page object tree: Found the same class in location {similarParent.Assembly.Location}' with child '{childOfSimilarParent}'. Ensure that page objects are only loaded once. Check https://stackoverflow.com/a/3624044 for ideas on how to solve this issue.")
        {
        }
    }


}