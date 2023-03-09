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

namespace Trumpf.Coparoo.Desktop.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Exceptions;
    using SmartBear.TestLeft;
    using Trumpf.Coparoo.Desktop.Waiting;
    using Trumpf.Coparoo.Desktop.Core.Waiting;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for root page objects.
    /// </summary>
    /// <typeparam name="TNode">The node type of the root page object.</typeparam>
    public abstract class RootObject<TNode> : PageObject<TNode>, IRootObjectInternal where TNode : class, IRootObjectNode, new()
    {
        private PageObjectLocator pageObjectLocator;
        private UIObjectInterfaceResolver objectInterfaceResolver;
        private const string DEFAULT_DOT_TREE_FILE = "PageObjectTree.dot";
        private const string DEFAULT_PDF_TREE_FILE = "PageObjectTree.pdf";
        private const string DEFAULT_DOT_PATH = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
        private Dictionary<Type, HashSet<Type>> dyanamicParentToChildMap = new Dictionary<Type, HashSet<Type>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RootObject{TNode}"/> class.
        /// </summary>
        public RootObject()
        {
            Init(null, new TNode());
            pageObjectLocator = new PageObjectLocator(this);
            objectInterfaceResolver = new UIObjectInterfaceResolver(this);
        }

        /// <summary>
        /// Gets or sets the (static) TestExecute driver.
        /// </summary>
        public IDriver Driver
        {
            get { return new TNode().Driver; }
            set { new TNode().Driver = value; }
        }

        /// <summary>
        /// Gets the node locator.
        /// </summary>
        INodeLocator IRootObjectInternal.NodeLocator
        {
            get { return ((IRootObjectNodeInternal)Node).NodeLocator; }
        }

        /// <summary>
        /// Gets the page object locator.
        /// </summary>
        IPageObjectLocator IRootObjectInternal.PageObjectLocator
        {
            get { return pageObjectLocator; }
        }

        /// <summary>
        /// Gets the UI object interface resolver.
        /// </summary>
        IUIObjectInterfaceResolver IRootObjectInternal.UIObjectInterfaceResolver
        {
            get { return objectInterfaceResolver; }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Configuration Configuration
        {
            get { return Node.Configuration; }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Statistics Statistics
        {
            get { return Node.Statistics; }
        }

        /// <summary>
        /// Resolve the root object interface to a loaded process object in the current app domain.
        /// </summary>
        /// <typeparam name="TRootObject">The interface to resolve.</typeparam>
        /// <returns>The root object.</returns>
        public static TRootObject Resolve<TRootObject>() where TRootObject : IRootObject
        {
            var matches = PageTests
                .Locate
                .Types
                .Where(e => !e.IsAbstract && typeof(TRootObject).IsAssignableFrom(e) && IsDefaultConstructible(e))
                .ToArray();

            if (!matches.Any())
            {
                throw new ProcessObjectNotFoundException<TRootObject>();
            }
            else
            {
                var type = matches.First();
                return (TRootObject)Activator.CreateInstance(type);
            }
        }

        /// <inheritdoc/>
        public string WriteDotTree(string filename = DEFAULT_DOT_TREE_FILE)
        {
            var outFile = Path.GetFullPath(filename);
            var dotTree = ((IPageObjectInternal)this).DotTree.ToString();
            File.WriteAllText(outFile, dotTree);
            return outFile;
        }

        /// <summary>
        /// Register the page object type with the given object.
        /// </summary>
        /// <typeparam name="TChildPageObject">The child page object type.</typeparam>
        /// <typeparam name="TParentPageObject">The parent page object type.</typeparam>
        /// <returns>Whether the value not yet registered.</returns>
        public bool Register<TChildPageObject, TParentPageObject>()
        {
            if (typeof(TChildPageObject).IsAbstract || typeof(TChildPageObject).IsInterface)
            {
                throw new InvalidOperationException($"{typeof(TChildPageObject).FullName} must not be abstract nor an interface.");
            }

            if (typeof(TParentPageObject).IsAbstract || typeof(TParentPageObject).IsInterface)
            {
                throw new InvalidOperationException($"{typeof(TParentPageObject).FullName} must not be abstract nor an interface.");
            }

            HashSet<Type> o;
            if (dyanamicParentToChildMap.TryGetValue(typeof(TParentPageObject), out o))
            {
                return o.Add(typeof(TChildPageObject));
            }
            else
            {
                dyanamicParentToChildMap.Add(typeof(TParentPageObject), new HashSet<Type>() { typeof(TChildPageObject) });
                return true;
            }
        }

        /// <inheritdoc/>
        IEnumerable<Type> IRootObjectInternal.DynamicChildren(Type pageObjectType)
        {
            HashSet<Type> value;
            return dyanamicParentToChildMap.TryGetValue(pageObjectType, out value) ? value : new HashSet<Type>();
        }

        /// <inheritdoc/>
        public string WritePdfTree(string filename = DEFAULT_PDF_TREE_FILE, string dotBinaryPath = DEFAULT_DOT_PATH)
        {
            string outPdf = Path.GetFullPath(filename);
            string outDot = Path.ChangeExtension(outPdf, ".dot");
            Process s = new Process
            {
                StartInfo =
                {
                    Arguments = " -o " + '"' + outPdf + '"' + " -Tpdf " + '"' + outDot + '"',
                    FileName = dotBinaryPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            // write tree as DOT file
            WriteDotTree(outDot);

            // try to convert it to PDF
            try
            {
                // assumes dot is installed if not no output is generated
                s.Start();

                string stdOut = s.StandardOutput.ReadToEnd();
                string stdErr = s.StandardError.ReadToEnd();

                s.WaitForExit();
                Trace.WriteLine("dot exit code: " + s.ExitCode);
                Trace.WriteLine("dot output (StandardOutput): " + stdOut);
                Trace.WriteLine("dot output (StandardError): " + stdErr);

                if (s.ExitCode == 0)
                {
                    Trace.WriteLine("Rendered tree written to <" + outPdf + ">");
                    return outPdf;
                }
                else
                {
                    Trace.WriteLine("Got non-zero return code. Rendering failed");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("No pdf graph generated; requires executable " + dotBinaryPath + "; available at http://graphviz.org/; Error text: " + e.Message);
            }

            Trace.WriteLine("Could not render tree");
            return outDot;
        }

        /// <summary>
        /// Gets an await-object.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="function">The function to wrap.</param>
        /// <param name="name">The display name used in timeout exceptions.</param>
        /// <returns>The wrapped object.</returns>
        IAwait<T> IRootObjectInternal.Await<T>(Func<T> function, string name)
        {
            return new Await<T>(function, name, GetType(), () => Configuration.WaitTimeout, () => Configuration.PositiveWaitTimeout, () => Configuration.ShowWaitingDialog);
        }

        private static bool IsDefaultConstructible(Type e)
        {
            try
            {
                return e.GetConstructor(Type.EmptyTypes) != null;
            }
            catch (FileNotFoundException exception)
            {
                Trace.WriteLine($"Could not check type '{e.GetType().FullName}' for a default constructor: {exception.Message}");
                return false;
            }
        }
    }
}
