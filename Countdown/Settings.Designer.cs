﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Countdown {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public long Version {
            get {
                return ((long)(this["Version"]));
            }
            set {
                this["Version"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UpdateRequired {
            get {
                return ((bool)(this["UpdateRequired"]));
            }
            set {
                this["UpdateRequired"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("B 2,C 3,D 6,F 2,G 3,H 2,J 1,K 1,L 5,M 4,N 8,P 4,Q 1,R 9,S 9,T 9,V 1,W 1,X 1,Y 1,Z" +
            " 1,")]
        public global::Countdown.Models.LetterList ConsonantFrequency {
            get {
                return ((global::Countdown.Models.LetterList)(this["ConsonantFrequency"]));
            }
            set {
                this["ConsonantFrequency"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("A 15,E 21,I 13,O 13,U 5,")]
        public global::Countdown.Models.LetterList VowelFrequency {
            get {
                return ((global::Countdown.Models.LetterList)(this["VowelFrequency"]));
            }
            set {
                this["VowelFrequency"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int PickNumberOption {
            get {
                return ((int)(this["PickNumberOption"]));
            }
            set {
                this["PickNumberOption"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int PickLetterOption {
            get {
                return ((int)(this["PickLetterOption"]));
            }
            set {
                this["PickLetterOption"] = value;
            }
        }
    }
}
