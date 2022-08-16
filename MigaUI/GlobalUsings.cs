global using System;
global using System.Windows;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.Windows.Controls;
global using System.Windows.Input;
global using System.Windows.Media;
global using System.Windows.Markup;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using DryIoc;
global using Acorisoft.Miga.Utils;
global using Acorisoft.Miga.UI;
global using Acorisoft.Miga.UI.Core;
global using Acorisoft.Miga.UI.Mvvm;
global using Acorisoft.Miga.UI.Commands;
global using Acorisoft.Miga.UI.Markup;


using System.Runtime.CompilerServices;

// TODO: Builtin Dialog -> Notify Query Confirm
// TODO: Button

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: AssemblyCompanyAttribute("MigaUI")]
[assembly: AssemblyConfigurationAttribute("Debug")]
[assembly: AssemblyFileVersionAttribute("0.3.0")]
[assembly: AssemblyInformationalVersionAttribute("0.3.0")]
[assembly: AssemblyProductAttribute("MigaUI")]
[assembly: AssemblyTitleAttribute("MigaUI")]
[assembly: AssemblyVersionAttribute("0.3.0")]
[assembly: System.Runtime.Versioning.TargetPlatformAttribute("Windows7.0")]
[assembly: System.Runtime.Versioning.SupportedOSPlatformAttribute("Windows7.0")]
[assembly: XmlnsDefinition("https://www.acorisoft.cn/miga/ui", "Acorisoft.Miga.UI")]
[assembly: XmlnsDefinition("https://www.acorisoft.cn/miga/ui", "Acorisoft.Miga.UI.Commands")]
[assembly: XmlnsDefinition("https://www.acorisoft.cn/miga/ui", "Acorisoft.Miga.UI.Markup")]
[assembly: InternalsVisibleTo("MigaUI.Test")]