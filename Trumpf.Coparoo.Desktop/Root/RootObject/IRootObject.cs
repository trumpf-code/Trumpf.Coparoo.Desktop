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

namespace Trumpf.Coparoo.Desktop
{
    using SmartBear.TestLeft;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Interface for root objects.
    /// </summary>
    public interface IRootObject : IPageObject
    {
        /// <summary>
        /// Gets or sets the TestExecute driver.
        /// </summary>
        IDriver Driver { get; set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        Configuration Configuration { get; }

        /// <summary>
        /// Gets the statistics.
        /// </summary>
        Statistics Statistics { get; }

        /// <summary>
        /// Register the page object type with the given object.
        /// </summary>
        /// <typeparam name="TParentPageObject">The parent page object type.</typeparam>
        /// <typeparam name="TChildPageObject">The child page object type.</typeparam>
        /// <returns>Whether the value not yet registered.</returns>
        bool Register<TParentPageObject, TChildPageObject>();

        /// <summary>
        /// Write the page object tree in the DOT format.
        /// </summary>
        /// <param name="filename">The filename to write to with extension.</param>
        /// <returns>The absolute file name that was written.</returns>
        string WriteDotTree(string filename = "PageObjectTree.dot");

        /// <summary>
        /// Write the page object tree in the PDF format.
        /// </summary>
        /// <param name="filename">The filename to write to with extension.</param>
        /// <param name="dotBinaryPath">The hint path to the dot.exe binary file.</param>
        /// <returns>The absolute file name that was written.</returns>
        string WritePdfTree(string filename = "PageObjectTree.pdf", string dotBinaryPath = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe");

        /// <summary>
        /// Optionally sets the assemblies which should be used to check for PageObjects and ControlObjects and searched Types
        /// </summary>
        /// <param name="assemblies"></param>
        void AddAssembliesForTypeResolving(IEnumerable<Assembly> assemblies);
    }
}