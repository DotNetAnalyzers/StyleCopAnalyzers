﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StyleCop.Analyzers.NamingRules {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class NamingResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal NamingResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("StyleCop.Analyzers.NamingRules.NamingResources", typeof(NamingResources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Rename To &apos;{0}&apos;.
        /// </summary>
        internal static string RenameToCodeFix {
            get {
                return ResourceManager.GetString("RenameToCodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Prefix interface name with &apos;I&apos;.
        /// </summary>
        internal static string SA1302CodeFix {
            get {
                return ResourceManager.GetString("SA1302CodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of a variable in C# does not begin with a lower-case letter..
        /// </summary>
        internal static string SA1312Description {
            get {
                return ResourceManager.GetString("SA1312Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Variable &apos;{0}&apos; must begin with lower-case letter.
        /// </summary>
        internal static string SA1312MessageFormat {
            get {
                return ResourceManager.GetString("SA1312MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Variable names must begin with lower-case letter.
        /// </summary>
        internal static string SA1312Title {
            get {
                return ResourceManager.GetString("SA1312Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of a parameter in C# does not begin with a lower-case letter..
        /// </summary>
        internal static string SA1313Description {
            get {
                return ResourceManager.GetString("SA1313Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;{0}&apos; must begin with lower-case letter.
        /// </summary>
        internal static string SA1313MessageFormat {
            get {
                return ResourceManager.GetString("SA1313MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter names must begin with lower-case letter.
        /// </summary>
        internal static string SA1313Title {
            get {
                return ResourceManager.GetString("SA1313Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When a method overrides a method from a base class, or implements an interface method, the parameter names of the overriding method should match the names in the base definition..
        /// </summary>
        internal static string SA1315Description {
            get {
                return ResourceManager.GetString("SA1315Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameters should match inherited names.
        /// </summary>
        internal static string SA1315MessageFormat {
            get {
                return ResourceManager.GetString("SA1315MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameters should match inherited names.
        /// </summary>
        internal static string SA1315Title {
            get {
                return ResourceManager.GetString("SA1315Title", resourceCulture);
            }
        }
    }
}
