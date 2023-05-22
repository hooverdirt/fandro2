using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Security.Principal;
using libfandro2.lib.WinAPI;
using libfandro2.lib.Matching;
using libfandro2.lib.Controls.Conditions;
using libfandro2.lib.Controls.Conditions.Classes;
using static libfandro2.lib.Matching.MatcherEnums;
using System.Diagnostics;
using static libfandro2.lib.WinAPI.ShellAPI;
using static System.Windows.Forms.ListViewItem;

namespace libfandro2.lib
{
    public static class Helpers {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentlist"></param>
        /// <param name="items"></param>
        public static void AddUnique<T>(this IList<T> currentlist, IEnumerable<T> items) {
            foreach (T? item in items)
                if (!currentlist.Contains(item)) { 
                    currentlist.Add(item); 
                }
        }
    } 
}
