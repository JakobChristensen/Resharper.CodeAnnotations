// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="">
//   
// </copyright>
// <summary>
//   AssemblyInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using System.Reflection;
using JetBrains.Application.PluginSupport;
using JetBrains.ReSharper.Daemon;

[assembly: AssemblyTitle("CodeAnnotationPack")]
[assembly: AssemblyDescription("Additional support for code annotations. Highlight method parameters, returns values and properties without code annotations. Quick Fix for applying code annotation attributes and assertions.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Jakob Christensen")]
[assembly: AssemblyProduct("CodeAnnotationPack")]
[assembly: AssemblyCopyright("Copyright © Jakob Christensen")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.4.0")]

// The following information is displayed by ReSharper in the Plugins dialog
[assembly: PluginTitle("Code Annotation Pack")]
[assembly: PluginDescription("Additional support for code annotations. Highlight method parameters, returns values and properties without code annotations. Quick Fix for applying code annotation attributes and assertions.")]
[assembly: PluginVendor("Jakob Christensen")]
[assembly: RegisterConfigurableSeverity("CodeAnnotationAnalyzer", null, "CodeSmell", "Add NotNull or CanBeNull attributes [Code Annotation Pack]", "\n          Add either [NotNull] or [CanBeNull] attributes to parameters and possible return value.\n        ", Severity.SUGGESTION, false, Internal = false)]