using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Fandro2.lib.WinAPI {
    public class ShellAPI {
        public const int SW_SHOW = 5;
        public const uint SEE_MASK_INVOKEIDLIST = 12;
        public struct SHFILEINFO {
            // Handle to the icon representing the file
            public IntPtr hIcon;
            // Index of the icon within the image list
            public int iIcon;
            // Various attributes of the file
            public uint dwAttributes;
            // Path to the file
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szDisplayName;
            // File type
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        public const int SHGFI_ICON = 0x100;
        public const int SHGFI_SMALLICON = 0x1;
        public const int SHGFI_LARGEICON = 0x0;

        public static Guid IID_IMalloc =
    new Guid("{00000002-0000-0000-C000-000000000046}");
        public static Guid IID_IShellFolder =
            new Guid("{000214E6-0000-0000-C000-000000000046}");
        public static Guid IID_IFolderFilterSite =
            new Guid("{C0A651F5-B48B-11d2-B5ED-006097C686F6}");
        public static Guid IID_IFolderFilter =
            new Guid("{9CC22886-DC8E-11d2-B1D0-00C04F8EEB3E}");

        // AutoComplete Guids
        public static Guid IID_IAutoCompList =
            new Guid("{00BB2760-6A77-11D0-A535-00C04FD7D062}");

        public static Guid IID_IObjMgr =
            new Guid("{00BB2761-6A77-11D0-A535-00C04FD7D062}");

        public static Guid IID_IACList =
            new Guid("{77A130B0-94FD-11D0-A544-00C04FD7D062}");

        public static Guid IID_IACList2 =
            new Guid("{470141A0-5186-11D2-BBB6-0060977B464C}");

        public static Guid IID_ICurrentWorkingDirectory =
            new Guid("{91956D21-9276-11D1-921A-006097DF5BD4}");

        public static Guid CLSID_AutoComplete =
            new Guid("{00BB2763-6A77-11D0-A535-00C04FD7D062}");

        public static Guid CLSID_ACLHistory =
            new Guid("{00BB2764-6A77-11D0-A535-00C04FD7D062}");

        public static Guid CLSID_ACListISF =
            new Guid("{03C036F1-A186-11D0-824A-00AA005B4383}");

        public static Guid CLSID_ACLMRU =
            new Guid("{6756A641-dE71-11D0-831B-00AA005B4383}");

        public static Guid CLSID_ACLMulti =
            new Guid("{00BB2765-6A77-11D0-A535-00C04FD7D062}");

        public static Guid CLSID_ACLCustomMRU =
            new Guid("{6935DB93-21E8-4CCC-BEB9-9fE3C77A297A}");

        // Instructs system edit controls to use AutoComplete to help complete URLs or 
        // file system paths. 
        [DllImport("shlwapi.dll")]
        public static extern int SHAutoComplete(
            IntPtr hwndEdit,
            uint dwFlags);

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

    }
}
