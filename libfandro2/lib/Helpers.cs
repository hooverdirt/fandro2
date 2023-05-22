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
        public const string FANDRO2OBJECTFILE = "fandro2obj.dll";
        public const string FANDRO2OBJECTKEY = @"Folder\shellex\ContextMenuHandlers\Fandro";

        /// <summary>
        /// Set double buffer property for protected DoubleBuffered property.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="enable"></param>
        public static void DoubleBuffering(this Control control, bool enable) {
            PropertyInfo doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentLocalAppPath() {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Assembly.GetExecutingAssembly().GetName().Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Icon GetSmallIcon(string filename) {
            Icon icon = null;

            ShellAPI.SHFILEINFO shinfo = new ShellAPI.SHFILEINFO();

            IntPtr hsmallimage = WinAPI.ShellAPI.SHGetFileInfo(filename, 0, ref shinfo, Marshal.SizeOf(shinfo),
                ShellAPI.SHGFI_ICON | ShellAPI.SHGFI_SMALLICON);

            icon = Icon.FromHandle(shinfo.hIcon);

            return icon;
        }


        /// <summary>
        /// For some reason, I can't cast an object collection to an IList
        /// which is obnoxious as ObjectCollection indeed does implement
        /// an Ilist.
        /// </summary>
        /// <param name="astring"></param>
        /// <param name="list"></param>
        /// <param name="maxcount"></param>
        public static void AddItemToHistoryList(String astring, ComboBox.ObjectCollection col, int maxitems) {
            int i = col.IndexOf(astring);

            if (i == -1) {
                col.Insert(0, astring);

                // delete items...
                while (col.Count > maxitems) {
                    col.RemoveAt(col.Count - 1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String HistoryListToString(ComboBox.ObjectCollection list) {
            String ret = "";

            foreach (String i in list) {
                ret += i + ",";
            }

            ret = ret.TrimEnd(',');

            return ret;
        }
        


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Boolean AreObjectsRegistered() {
            Boolean b = false;

            RegistryKey r = Registry.ClassesRoot.OpenSubKey(FANDRO2OBJECTKEY);

            if (r != null) {
                b = true;
            }

            return b;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        public static void RegisterFandroObjects() {

            String fandropath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), FANDRO2OBJECTFILE);
           

            if (File.Exists(fandropath)) {
                RegistrationServices regservice = new RegistrationServices();
                try {
                    Assembly objects = Assembly.LoadFrom(fandropath);
                    regservice.RegisterAssembly(objects, AssemblyRegistrationFlags.SetCodeBase);

                    // register the current path to HKCR/Software/Fandro/
                    //      Key MainPath=
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                    if (key != null) {
                        RegistryKey fandro = key.CreateSubKey("Fandro2");
                        try {
                            if (fandro != null) {
                                fandro.SetValue("MainPath", Path.GetDirectoryName(Application.ExecutablePath));
                                fandro.Flush();
                            }
                        } finally {
                            if (fandro != null) {
                                fandro.Close();
                            }
                        }
                        key.Flush();
                        key.Close();
                    }
                } catch (Exception ex) {
                    throw ex;
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnregisterFandroObjects() {
            String fandropath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), FANDRO2OBJECTFILE);

            if (File.Exists(fandropath)) {
                RegistrationServices regservice = new RegistrationServices();
                try {
                    Assembly objects = Assembly.LoadFrom(fandropath);
                    RegistrationServices regService = new RegistrationServices();
                    regService.UnregisterAssembly(objects);

                    Registry.CurrentUser.DeleteSubKey(@"Software\Fandro2");

                } catch (Exception ex) {
                    throw ex;
                }

            }

        }
        */

        public static bool IsUserAdministrator() {
            bool isAdmin;
            try {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            } catch (UnauthorizedAccessException ex) {
                isAdmin = false;
            } catch (Exception ex) {
                isAdmin = false;
            }
            return isAdmin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static FileFilters ToMatcher(this List<SelectableDataRow> list,
            MatcherOperator op) {

            FileFilters matchconditions = new FileFilters();
            matchconditions.ValidateType = op;

            foreach (SelectableDataRow row in list) {
                FilePropertyItem t = row.SelectedFilePropertyItem;
                OperatorItem o = row.SelectedOperatorItem;

                if (t != null) {
                    if (t.ValueType == typeof(DateTime)) {
                        matchconditions.AddMatcher(new DateTimeMatcher { 
                            CompareValue = (DateTime) row.Value,
                            MatcherType = t.MatcherType, 
                            MatcherAction = o.Operator
                        });;
                    }
                    else if (t.ValueType == typeof(int)) {
                        if (row.HasUnits) {
                            matchconditions.AddMatcher(new FileSizeMatcher {
                                CompareValue = (int)row.Value,
                                MatcherType =t.MatcherType,
                                MatcherAction = o.Operator,
                                Units = row.Units
                            });
                        }
                        else {
                            matchconditions.AddMatcher(new IntegerMatcher {
                                CompareValue = (int)row.Value,
                                MatcherType = t.MatcherType,
                                MatcherAction = o.Operator
                            });
                        }
                    }
                    else {
                        matchconditions.AddMatcher(new StringMatcher {
                            CompareValue = (String) row.Value,
                            MatcherType = t.MatcherType,
                            MatcherAction = o.Operator
                        });
                    }                   
                }
            }

            return matchconditions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void StartApplication(string path) {
            try {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(path);
                processStartInfo.UseShellExecute = true;
                Process.Start(processStartInfo);
            }
            catch { 
                // AHTODO
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static bool ShowPropertyDialogBox(string path) {
            bool b = false;
            try {
                SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
                info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
                info.lpVerb = "properties";
                info.lpFile = path;
                info.nShow = SW_SHOW;
                info.fMask = SEE_MASK_INVOKEIDLIST;
                b = ShellExecuteEx(ref info);
            }
            catch { 
                // AHTODO
            }

            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        /// <returns></returns>
        public static string ListViewDataToString(this ListView listView) {
            string s = "";

            foreach(ColumnHeader c in listView.Columns) {
                s += string.Format("\"{0}\",", c.Text);
            }

            s = s.TrimEnd(',');
            s += Environment.NewLine;

            foreach(ListViewItem item in listView.Items) {
                foreach(ListViewSubItem sp in item.SubItems) {
                    s += string.Format("\"{0}\",", sp.Text);
                }
                s = s.TrimEnd(',');
                s += Environment.NewLine;
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FileVersionInfo GetFileVersionInfo() {
            Assembly ap = Assembly.GetExecutingAssembly();
            return FileVersionInfo.GetVersionInfo(ap.Location);
        }

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
