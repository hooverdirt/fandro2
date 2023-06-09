using System;
using System.Runtime.InteropServices;

namespace libfandro2.lib.WinAPI {
    public class ShellAutoComplete {

        public static bool DoAutoComplete(IntPtr hwndEdit, AutoCompleteFlags flags) {
            int hRet;
            hRet = ShellAPI.SHAutoComplete(hwndEdit, (uint)flags);
            return hRet == 0;
        }

        [Flags]
        public enum AutoCompleteFlags : uint {
            /// <summary>
            /// The default setting, equivalent to FileSystem | UrlAll. Default cannot be
            ///  combined with any other flags.
            /// </summary>
            Default = 0x00000000,
            /// <summary>
            /// This includes the File System as well as the rest of the shell 
            /// (Desktop\My Computer\Control Panel\)
            /// </summary>
            FileSystem = 0x00000001,
            /// <summary>
            /// Include the URLs in the users History and Recently Used lists. Equivalent 
            /// to UrlHistory | UrlMRU.
            /// </summary>
            UrlAll = UrlHistory | UrlMRU,
            /// <summary>
            /// Include the URLs in the user's History list.
            /// </summary>
            UrlHistory = 0x00000002,
            /// <summary>
            /// Include the URLs in the user's Recently Used list.
            /// </summary>
            UrlMRU = 0x00000004,
            /// <summary>
            /// Allow the user to select from the autosuggest list by pressing the TAB 
            /// key. If this flag is not set, pressing the TAB key will shift focus to 
            /// the next control and close the autosuggest list. If UseTab is set, 
            /// pressing the TAB key will select the first item in the list. Pressing 
            /// TAB again will select the next item in the list, and so on. When the user 
            /// reaches the end of the list, the next TAB key press will cycle the focus 
            /// back to the edit control. This flag must be used in combination with one 
            /// or more of the FileSys* or Url* flags.
            /// </summary>
            UseTab = 0x00000008,
            /// <summary>
            /// This includes the File System
            /// </summary>
            FileSys_Only = 0x00000010,
            /// <summary>
            /// Same as FileSys_Only except it only includes directories, UNC servers, 
            /// and UNC server shares.
            /// </summary>
            FileSys_Dirs = 0x00000020,
            /// <summary>
            /// Ignore the registry value and force the autosuggest feature on. A 
            /// selection of possible completed strings will be displayed as a drop-down 
            /// list, below the edit box. This flag must be used in combination with one
            ///  or more of the FileSys* or Url* flags.
            /// </summary>
            AutoSuggest_Force_On = 0x10000000,
            /// <summary>
            /// Ignore the registry default and force the autosuggest feature off. This 
            /// flag must be used in combination with one or more of the FileSys* or 
            /// Url* flags.
            /// </summary>
            AutoSuggest_Force_Off = 0x20000000,
            /// <summary>
            /// Ignore the registry value and force the autoappend feature on. The 
            /// completed string will be displayed in the edit box with the added 
            /// characters highlighted. This flag must be used in combination with one 
            /// or more of the FileSys* or Url* flags.
            /// </summary>
            AutoAppend_Force_On = 0x40000000,
            /// <summary>
            /// Ignore the registry default and force the autoappend feature off. This 
            /// flag must be used in combination with one or more of the FileSys* or 
            /// Url* flags.
            /// </summary>
            AutoAppend_Force_Off = 0x80000000
        }


        [Flags]
        public enum AutoCompleteListOptions {
            None = 0,       // don't enumerate anything
            CurrentDir = 1,     // enumerate current directory
            MyComputer = 2,     // enumerate MyComputer
            Desktop = 4,        // enumerate Desktop Folder
            Favorites = 8,      // enumerate Favorites Folder
            FileSysOnly = 16,       // enumerate only the file system
            FileSysDirs = 32        // enumerate only the file system dirs, UNC shares, and UNC servers.
        }


        [Flags]
        public enum AutoCompleteOptions {
            None = 0,
            AutoSuggest = 0x1,
            AutoAppend = 0x2,
            Search = 0x4,
            FilterPreFixes = 0x8,
            UseTab = 0x10,
            UpDownKeyDropsList = 0x20,
            RtlReading = 0x40
        }


        public ShellAutoComplete() {
        }

        public IntPtr EditHandle = IntPtr.Zero;
        public object ListSource = null;
        public AutoCompleteOptions ACOptions = AutoCompleteOptions.AutoSuggest | AutoCompleteOptions.AutoAppend;

        private object GetAutoComplete() {
            Type typeAutoComplete = Type.GetTypeFromCLSID(ShellAPI.CLSID_AutoComplete);

            object obj;
            obj = Activator.CreateInstance(typeAutoComplete);
            return obj;
        }

        public static object GetACLHistory() {
            Type typeACLHistory = Type.GetTypeFromCLSID(ShellAPI.CLSID_ACLHistory);

            object obj;
            obj = Activator.CreateInstance(typeACLHistory);
            return obj;
        }

        public static object GetACLMRU() {
            Type typeACLMRU = Type.GetTypeFromCLSID(ShellAPI.CLSID_ACLMRU);

            object obj;
            obj = Activator.CreateInstance(typeACLMRU);
            return obj;
        }

        public static object GetACListISF() {
            Type typeACListISF = Type.GetTypeFromCLSID(ShellAPI.CLSID_ACListISF);

            object obj;
            obj = Activator.CreateInstance(typeACListISF);
            return obj;
        }

        public static object GetACLMulti() {
            Type typeACLMulti = Type.GetTypeFromCLSID(ShellAPI.CLSID_ACLMulti);

            object obj;
            obj = Activator.CreateInstance(typeACLMulti);
            return obj;
        }


        public void SetAutoComplete(bool enable) {
            int ret;
            IAutoComplete2 iac2 = (IAutoComplete2)GetAutoComplete();

            if (EditHandle == IntPtr.Zero)
                throw new Exception("EditHandle must not be zero!");

            if (ListSource == null)
                throw new Exception("ListSource must not be null!");


            ret = iac2.Init(EditHandle, ListSource, "", "");

            ret = iac2.SetOptions((uint)ACOptions);

            ret = iac2.Enable(enable ? 1 : 0);

            Marshal.ReleaseComObject(iac2);

        }





    }

}