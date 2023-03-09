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

namespace Trumpf.Coparoo.Desktop.Logging.DotTree
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Node class.
    /// </summary>
    internal class Node
    {
        private string id;

        /// <summary>
        /// Node types.
        /// </summary>
        public enum NodeTypes
        {
            /// <summary>
            /// Node represents a page object.
            /// </summary>
            [Description("PAGE OBJECT")]
            PageObject,

            /// <summary>
            /// Node represents a page test class.
            /// </summary>
            [Description("PAGE TEST CLASS")]
            PageTestClass,

            /// <summary>
            /// Node represents a page test method.
            /// </summary>
            [Description("PAGE TEST")]
            PageTest,

            /// <summary>
            /// Node represents a page test issue method.
            /// </summary>
            [Description("PAGE TEST ISSUE")]
            PageTestIssue
        }

        /// <summary>
        /// The frame colors.
        /// </summary>
        public enum Color
        {
            /// <summary>
            /// The color is black.
            /// </summary>
            [Description("white")]
            White,

            /// <summary>
            /// The color is grey.
            /// </summary>
            [Description("grey")]
            Grey,

            /// <summary>
            /// The color is orange.
            /// </summary>
            [Description("orange")]
            Orange,

            /// <summary>
            /// The color is green.
            /// </summary>
            [Description("green")]
            Green,

            /// <summary>
            /// The color is red.
            /// </summary>
            [Description("red")]
            Red,
        }

        /// <summary>
        /// Gets or sets the node type.
        /// </summary>
        public NodeTypes NodeType { get; set; }

        /// <summary>
        /// Gets or sets the frame color.
        /// </summary>
        public Color FrameColor { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public string Id
        {
            get { return Regex.Replace(id, "[^a-zA-Z]+", "_", RegexOptions.Compiled).Trim(); }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the node caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the picture.
        /// </summary>
        public Image Picture { get; set; }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Compare two nodes.
        /// </summary>
        /// <param name="obj">The other node.</param>
        /// <returns>Whether both nodes have the same ID.</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(GetType()))
            {
                return (obj as Node).Id.Equals(Id);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            string l;
            if (Picture == null)
            {
                l = '"' + GetDescription(NodeType) + "\n\n" + Caption.Replace(Environment.NewLine, "\\n") + '"';
            }
            else
            {
                // save image to working directory
                string imageFilename = Id + ".png";
                Picture.Save(imageFilename, System.Drawing.Imaging.ImageFormat.Png);

                l = "<<TABLE border=\"0\" cellborder=\"0\"><TR><TD><IMG SRC=\"" + imageFilename + "\" scale=\"true\"/></TD><TD>" + GetDescription(NodeType) + "<BR/><BR/>" + Caption.Replace(Environment.NewLine, "<BR/>") + "</TD></TR></TABLE>>";
            }

            return Id + "[ " + "color=black, margin=0 shape=box, style=filled, fillcolor=" + GetDescription(FrameColor) + ", fontsize=20, fontname=Arial" + " label = " + l + "];";
        }

        /// <summary>
        /// Gets the description of an enum.
        /// </summary>
        /// <param name="value">The enum.</param>
        /// <exception cref="InvalidOperationException">Thrown if the enum does not have a <see cref="System.ComponentModel.DescriptionAttribute"/> attribute.</exception>
        /// <returns>Returns the description.</returns>
        private static string GetDescription(Enum value)
        {
            System.Reflection.FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute == null)
            {
                throw new InvalidOperationException(string.Format(@"The enum does not have a {0} attribute.", typeof(DescriptionAttribute).FullName));
            }

            return attribute.Description;
        }
    }
}