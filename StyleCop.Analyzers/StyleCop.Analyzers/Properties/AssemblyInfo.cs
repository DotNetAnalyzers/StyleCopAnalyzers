// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("StyleCop.Analyzers")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tunnel Vision Laboratories, LLC")]
[assembly: AssemblyProduct("StyleCop.Analyzers")]
[assembly: AssemblyCopyright("Copyright © Sam Harwell 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]
[assembly: NeutralResourcesLanguage("en-US")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.26")]
[assembly: AssemblyFileVersion("1.0.0.26")]
[assembly: AssemblyInformationalVersion("1.0.0-dev")]

#if DEVELOPMENT_KEY
[assembly: InternalsVisibleTo("StyleCop.Analyzers.CodeFixes, PublicKey=0024000004800000940000000602000000240000525341310004000001000100fbe8bc154d11de9907c4e19600890ebef9cf9c8456c9a8ee05f0a8c9e69bd4f66c038ff4ea769c8864e6c5a38a1aad538876d6b2549962856f1b39e10d33bb13940c6538d2e863542ae15bc6e251946ca18094b6902690866d514f1fcd9395756732f927f9aeab1d6e1af5190816a251ad29db9c5b4cb86de7d909fc6c3d18a0")]
#else
[assembly: InternalsVisibleTo("StyleCop.Analyzers.CodeFixes, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ad62a4e5529344c07fe1455f270d61b205bdc8b0a94bcbe80b8506f28061073e4ed750b7e3d344f23213f671397a05e8c59b1434555f78edc091c0cf7b603011cf126aaa10116d890354f97f369ff56e24df17ee7f22cc3dd4d4b841d027d6d3d3b52a9a4462b8acf0f4bb9f400256ae18eed71070692e4cdd051498d04a66ed")]
#endif
[assembly: InternalsVisibleTo("StyleCop.Analyzers.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c36d40d996fcc95fb6a89754728616758f459026e31478ce93633b3e27a4af416f103aa3d7a9e7998f829f8715cc1240d30724fd662042550fa71357b19562622424267e9e4640c403edbe64709a9ca5918128a9b9020b0db6e770d0dd1eac888869c23a835b74bde00e171984b1d1c24636cf030f0b23106e73035a2be145a6")]
[assembly: InternalsVisibleTo("StyleCopTester, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c36d40d996fcc95fb6a89754728616758f459026e31478ce93633b3e27a4af416f103aa3d7a9e7998f829f8715cc1240d30724fd662042550fa71357b19562622424267e9e4640c403edbe64709a9ca5918128a9b9020b0db6e770d0dd1eac888869c23a835b74bde00e171984b1d1c24636cf030f0b23106e73035a2be145a6")]
