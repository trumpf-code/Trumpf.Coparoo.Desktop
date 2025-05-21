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

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.

using System.Runtime.CompilerServices;

[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: System.CLSCompliant(true)]

#if KEY_FOUND
[assembly: InternalsVisibleTo("Trumpf.Coparoo.Desktop.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b99a8799dd15ee2f4091d5a8dc742455c776a11860aa7cd17c2da2e14a1225a5a503c5451012285add1c557ec3f931dbaa3aecb3384ec9028ed931aeddfcc9c98dda98734a90c5eac60c585e998464ccac47bb2c6c6437bdd4d6af874c2e30e526839e13e7537a938a9224b6b91f4377fbeede9f5b60ca36412f0761f0319db1")]
[assembly: InternalsVisibleTo("Trumpf.Coparoo.Desktop.DemoApp.Interf, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b99a8799dd15ee2f4091d5a8dc742455c776a11860aa7cd17c2da2e14a1225a5a503c5451012285add1c557ec3f931dbaa3aecb3384ec9028ed931aeddfcc9c98dda98734a90c5eac60c585e998464ccac47bb2c6c6437bdd4d6af874c2e30e526839e13e7537a938a9224b6b91f4377fbeede9f5b60ca36412f0761f0319db1")]
[assembly: InternalsVisibleTo("Trumpf.Coparoo.Desktop.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b99a8799dd15ee2f4091d5a8dc742455c776a11860aa7cd17c2da2e14a1225a5a503c5451012285add1c557ec3f931dbaa3aecb3384ec9028ed931aeddfcc9c98dda98734a90c5eac60c585e998464ccac47bb2c6c6437bdd4d6af874c2e30e526839e13e7537a938a9224b6b91f4377fbeede9f5b60ca36412f0761f0319db1")]
#else
[assembly: InternalsVisibleTo("Trumpf.Coparoo.Desktop.Tests")]
[assembly: InternalsVisibleTo("Trumpf.Coparoo.Desktop.DemoApp.Interf")]
#endif