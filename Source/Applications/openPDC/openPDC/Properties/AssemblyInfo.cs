﻿using System.Reflection;
using System.Runtime.InteropServices;

// Assembly identity attributes.
[assembly: AssemblyVersion("1.0.46.38625")]

// Informational attributes.
[assembly: AssemblyCompany("TVA")]
[assembly: AssemblyCopyright("No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.")]
[assembly: AssemblyProduct("openPDC")]

// Assembly manifest attributes.
#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif
[assembly: AssemblyDescription("Windows service that hosts input, action and output adapters.")]
[assembly: AssemblyTitle("openPDC Iaon Host")]

// Other configuration attributes.
[assembly: ComVisible(false)]
[assembly: Guid("c8a353e9-9383-46c5-8c19-8da53d307cde")]
