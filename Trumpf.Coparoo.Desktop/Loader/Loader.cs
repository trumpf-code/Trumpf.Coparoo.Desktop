// Copyright 2016, 2017, 2018, 2019, 2020 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Assembly loader class.
    /// </summary>
    public static class Loader
    {
        private static string[] Dirs;

        private static void LoadAssemblyPrinter(object sender, AssemblyLoadEventArgs args) => Logger("Assembly loaded: " + args.LoadedAssembly.FullName + " from " + args.LoadedAssembly.Location);

        private static Assembly LoadFromUserFolder(object sender, ResolveEventArgs args)
        {
            var candidates = Dirs.Select(folderPath => Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll"));
            var existing = candidates.Where(path => File.Exists(path));

            Logger($"Assembly load candidates: {string.Join(", ", candidates)}");
            if (existing.Any())
            {
                Logger($"Loading the first out of the following existing candidates: {string.Join(", ", existing)}");

                var winner = existing.First();
                if (Path.GetFileNameWithoutExtension(winner).Equals("SmartBear.TestLeft", StringComparison.InvariantCultureIgnoreCase))
                {
                    var restAssemblyPath = Path.GetDirectoryName(winner) + @"\RestSharp.dll";
                    Logger($"Loading {restAssemblyPath}...");
                    Assembly.LoadFrom(restAssemblyPath);
                }

                Logger($"Redirecting load to {winner}...");
                return Assembly.LoadFrom(winner);
            }
            else
            {
                Logger("No or only non-existing candidates.");
                return null;
            }
        }

        private static readonly List<string> appConfigContents = new List<string>()
        {
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
            "<configuration>",
            "  <runtime>",
            "    <!-- Redirect TestLeft assemblies to a larger version so they get resolved at runtime -->",
            "    <assemblyBinding xmlns=\"urn:schemas-microsoft-com:asm.v1\">",
            "      <dependentAssembly>",
            "        <assemblyIdentity name=\"SmartBear.TestLeft.WebApiWrapper\"",
            "                          publicKeyToken=\"483cb98dc5760d9e\"",
            "                          culture=\"neutral\" />",
            "        <bindingRedirect oldVersion=\"12.31.1833.11\" newVersion=\"99.99.99.99\" />",
            "      </dependentAssembly>",
            "      <dependentAssembly>",
            "        <assemblyIdentity name=\"SmartBear.TestLeft\"",
            "                          publicKeyToken=\"483cb98dc5760d9e\"",
            "                          culture=\"neutral\" />",
            "        <bindingRedirect oldVersion=\"12.31.1833.11\" newVersion=\"99.99.99.99\" />",
            "      </dependentAssembly>",
            "    </assemblyBinding>",
            "  </runtime>",
            "</configuration>"
        };

        /// <summary>
        /// Gets the logger action.
        /// </summary>
        private static Action<string> Logger;

        /// <summary>
        /// Gets the default assembly load folders.
        /// </summary>
        public static string[] DefaultLoadFolders => new string[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"SmartBear\TestLeft 2\API\dotNET"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"SmartBear\TestLeft 2\API\dotNET"),
            Environment.CurrentDirectory,
        };

        /// <summary>
        /// Register assembly load redirection folders.
        /// </summary>
        /// <param name="logger">The logger to use. Passing <code>null</code> deactivates logging.</param>
        /// <param name="folders">The list of folders. If no directory is passed, the default is used.</param>
        public static void RegisterRedirections(Action<string> logger, params string[] folders)
        {
            Logger = logger ?? (_ => { });
            var currentAppDomain = AppDomain.CurrentDomain;

            // no SmartBear assembly should be loaded yet
            var alreadyLoaded = currentAppDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("SmartBear", StringComparison.InvariantCultureIgnoreCase));
            if (alreadyLoaded.Any())
            {
                throw new TypeLoadException($"The following SmartBear assemblies must not yet be loaded before a call to '{nameof(RegisterRedirections)}': {string.Join(", ", alreadyLoaded)}. Make sure that (1) method {nameof(RegisterRedirections)} called before any method call that uses any SmartBear functionality. These steps may resolve this problem: (1) ensure that these assemblies are not present in the working directory, or (2) deploy a 'App.config' with a content like this: {Environment.NewLine}{string.Join(Environment.NewLine, appConfigContents)}");
            }

            var defaults = DefaultLoadFolders.Where(e => Directory.Exists(e)).Distinct().ToArray();
            Dirs = folders.Any() ? folders : DefaultLoadFolders;

            currentAppDomain.AssemblyResolve += new ResolveEventHandler(LoadFromUserFolder);
            currentAppDomain.AssemblyLoad += new AssemblyLoadEventHandler(LoadAssemblyPrinter);

            Logger($"Currently loaded assemblies: {Environment.NewLine}{string.Join(Environment.NewLine, currentAppDomain.GetAssemblies().Select(a => "- " + a.GetName().Name))}");
        }

        /// <summary>
        /// Register assembly load redirection folders.
        /// Discard logging.
        /// </summary>
        /// <param name="folders">The list of folders.</param>
        public static void RegisterRedirections(params string[] folders) => RegisterRedirections(null, folders);
    }
}