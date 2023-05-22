using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libfandro2.lib.Finding {


    public class FindOptions {
        private string fileMask = null;
        private string pattern = null;
        private string targetFolder = null;
        private FileSearchOptions searchOptions = FileSearchOptions.Recursive;
        private bool execute = false;

        /// <summary>
        /// 
        /// </summary>
        public string FileMask {
            get { return fileMask; }
            set { fileMask = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Pattern {
            get { return pattern; }
            set { pattern = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TargetFolder {
            get { return targetFolder; }
            set { targetFolder = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FileSearchOptions SearchOptions {
            get { return searchOptions; }
            set { searchOptions = value; }
        }

        public bool Execute {
            get { return execute; }
            set { execute = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FindOptions() {
            fileMask = "";
            pattern = "";
            targetFolder = "";
            searchOptions = FileSearchOptions.None;
            execute = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="afilemask"></param>
        /// <param name="apattern"></param>
        /// <param name="afolder"></param>
        /// <param name="fileoptions"></param>
        /// <param name="doexecute"></param>
        public FindOptions(string afilemask, string apattern, string afolder, FileSearchOptions fileoptions, bool doexecute) : this() {
            fileMask = afilemask;
            pattern = apattern;
            targetFolder = afolder;
            searchOptions = fileoptions;
            execute = doexecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private string findArgumentValue(string argument, string[] arguments) {
            string ret = "";

            foreach (string s in arguments) {
                if (s.Contains(argument)) {
                    // try splitting it
                    string[] arr = s.Split('=');

                    if (arr.Length > 1) {
                        ret = arr[1];
                        break;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="astring"></param>
        /// <returns></returns>
        private FileSearchOptions convertStringToOptions(string astring) {
            FileSearchOptions option = FileSearchOptions.None;

            if (astring.ToLower().Contains("r")) {
                option |= FileSearchOptions.Recursive;
            }

            if (astring.ToLower().Contains("c")) {
                option |= FileSearchOptions.CaseSensitive;
            }

            return option;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        public FindOptions(string[] arguments) {
            // --mask
            // --dir
            // --pattern
            // -- options
            // -- exec

            fileMask = findArgumentValue("--mask", arguments);
            targetFolder = findArgumentValue("--dir", arguments);
            pattern = findArgumentValue("--pattern", arguments);

            bool p = false;
            bool pvalue = false;

            p = bool.TryParse(findArgumentValue("--exec", arguments), out pvalue);
            execute = pvalue;

            searchOptions = convertStringToOptions(findArgumentValue("--options", arguments));

        }
    }
}
