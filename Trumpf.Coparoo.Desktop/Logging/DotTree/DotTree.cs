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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Dot tree class.
    /// </summary>
    internal class DotTree
    {
        private List<Node> nodes = new List<Node>();

        private List<Edge> edges = new List<Edge>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DotTree"/> class.
        /// </summary>
        /// <param name="root">The tree root.</param>
        public DotTree(Node root)
        {
            nodes.Add(root);
            Root = root;
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        public Node Root { get; private set; }

        /// <summary>
        /// Gets the node count.
        /// </summary>
        public int NodeCount
        {
            get { return nodes.Count(); }
        }

        /// <summary>
        /// Gets the edge count.
        /// </summary>
        public int EdgeCount
        {
            get { return edges.Count(); }
        }

        /// <summary>
        /// Create the union of two trees.
        /// </summary>
        /// <param name="d1">First tree (extended).</param>
        /// <param name="d2">Second tree.</param>
        /// <returns>The union of the trees.</returns>
        public static DotTree operator +(DotTree d1, DotTree d2)
        {
            d1.nodes = d1.nodes.Union(d2.nodes).ToList();
            d1.edges.AddRange(d2.edges);
            return d1;
        }

        /// <summary>
        /// Add nodes.
        /// </summary>
        /// <param name="d1">The tree to extend.</param>
        /// <param name="n">The new nodes.</param>
        /// <returns>The extended tree.</returns>
        public static DotTree operator +(DotTree d1, Node n)
        {
            d1.nodes.Add(n);
            return d1;
        }

        /// <summary>
        /// Add edges.
        /// </summary>
        /// <param name="d1">The tree to extend.</param>
        /// <param name="e">The new edges.</param>
        /// <returns>The extended tree.</returns>
        public static DotTree operator +(DotTree d1, Edge e)
        {
            d1.edges.Add(e);
            return d1;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return
                "digraph graphname " + Environment.NewLine +
                "{" + Environment.NewLine +
                "size=\"1,1\";" + Environment.NewLine +
                "rankdir = LR;" + Environment.NewLine +
                "splines = line;" + Environment.NewLine +
                 string.Join(Environment.NewLine, nodes) + Environment.NewLine +
                 string.Join(Environment.NewLine, edges) + Environment.NewLine +
                 "{rank=same; " + string.Join(" ", nodes.Where(f => f.NodeType == Node.NodeTypes.PageTest).Select(e => e.Id)) + "}" + Environment.NewLine +
                 "{rank=same; " + string.Join(" ", nodes.Where(f => f.NodeType == Node.NodeTypes.PageTestClass).Select(e => e.Id)) + "}" + Environment.NewLine +
                 "}";
        }
    }
}
