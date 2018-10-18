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

namespace Trumpf.Coparoo.Desktop.WPF
{
    using SmartBear.TestLeft.TestObjects;

    /// <summary>
    /// Extension methods for TestLeft controls.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the value of the WPF control ordinal number.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The property value.</returns>
        public static int GetWPFControlOrdinalNo(this IObject node)
        {
            return node.GetProperty<int>("WPFControlOrdinalNo");
        }

        /// <summary>
        /// Gets the value of the object text. In most cases, this is the object's caption.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The control text.</returns>
        public static string GetWpfControlText(this IObject node)
        {
            return node.GetProperty<string>("WpfControlText");
        }

        /// <summary>
        /// Gets the value of the full class name property.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The property value.</returns>
        public static string GetClrFullClassName(this IObject node)
        {
            return node.GetProperty<string>("ClrFullClassName");
        }

        /// <summary>
        /// Gets the value of the text property.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The property value.</returns>
        public static string GetText(this IObject node)
        {
            return node.GetProperty<string>("Text");
        }

        /// <summary>
        /// Get a data context property.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="propertyName">The property name.</param>
        /// <typeparam name="T">The property type.</typeparam>
        /// <returns>The value of the property in the data context.</returns>
        public static T GetDataContextProperty<T>(this IObject node, string propertyName)
        {
            return node.GetDataContextProperty<T>(new string[] { propertyName });
        }

        /// <summary>
        /// Get a data context property.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="propertyNames">The paths to the property.</param>
        /// <typeparam name="T">The property type.</typeparam>
        /// <returns>The value of the property in the data context.</returns>
        public static T GetDataContextProperty<T>(this IObject node, params string[] propertyNames)
        {
            return node.GetProperty<IObject>("DataContext").GetProperty<T>(string.Join(".", propertyNames));
        }
    }
}