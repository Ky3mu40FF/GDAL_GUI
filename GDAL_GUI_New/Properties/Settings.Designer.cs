﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GDAL_GUI_New.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("en-US")]
        public global::System.Globalization.CultureInfo ProgramLanguage {
            get {
                return ((global::System.Globalization.CultureInfo)(this["ProgramLanguage"]));
            }
            set {
                this["ProgramLanguage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\GDAL_Bundle\\")]
        public string UtilitiesDirectory {
            get {
                return ((string)(this["UtilitiesDirectory"]));
            }
            set {
                this["UtilitiesDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Eng")]
        public string DescriptionsLanguage {
            get {
                return ((string)(this["DescriptionsLanguage"]));
            }
            set {
                this["DescriptionsLanguage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseTheBundledUtilities {
            get {
                return ((bool)(this["UseTheBundledUtilities"]));
            }
            set {
                this["UseTheBundledUtilities"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\Resources\\ImageNotAvailable.bmp")]
        public string ImageNotAvailableRelativePath {
            get {
                return ((string)(this["ImageNotAvailableRelativePath"]));
            }
            set {
                this["ImageNotAvailableRelativePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TempDirectory {
            get {
                return ((string)(this["TempDirectory"]));
            }
            set {
                this["TempDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool GenerateThumbnails {
            get {
                return ((bool)(this["GenerateThumbnails"]));
            }
            set {
                this["GenerateThumbnails"] = value;
            }
        }
    }
}
