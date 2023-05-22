using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace libfandro2.lib.Matching {
    public class FileFilters {
        private List<Matcher> matchers = new List<Matcher>();
        private FileInfo currentFileInfo = null;


        /// <summary>
        /// 
        /// </summary>
        public FileFilters() { }

        /// <summary>
        /// 
        /// </summary>
        private bool doAndMatch() {
            bool b = true;

            foreach (Matcher m in matchers) {
                b = b && m.DoMatch();

                /// don't bother keep looking... false is false in an and...
                if (b == false) {
                    break;
                }
            }

            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool doOrMatch() {
            bool b = false;

            foreach (Matcher m in matchers) {
                b = b || m.DoMatch();
            }

            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Matcher> Matchers {
            get { return matchers; }
            set { matchers = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MatcherEnums.MatcherOperator ValidateType {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matcher"></param>
        public void AddMatcher(Matcher matcher) {
            matchers.Add(matcher);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matcher"></param>
        public void RemoveMatcher(Matcher matcher) {
            matchers.Remove(matcher);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveMatcherAt(int index) {
            matchers.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count {
            get { return this.matchers.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            matchers.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void setFileInfoMatchers(FileInfo value) { 
            if (value != null) { 
                foreach(Matcher m in this.matchers) {
                    switch(m.MatcherType) {
                        case MatcherEnums.MatcherType.FileCreateTime:
                            (m as DateTimeMatcher).CurrentValue = value.CreationTime;
                            break;
                        case MatcherEnums.MatcherType.FileAccessTime:
                            (m as DateTimeMatcher).CurrentValue = value.LastAccessTime;
                            break;
                        case MatcherEnums.MatcherType.FileModTime:
                            (m as DateTimeMatcher).CurrentValue = value.LastWriteTime;
                            break;
                        case MatcherEnums.MatcherType.FileSize:
                            (m as FileSizeMatcher).CurrentValue = value.Length;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FileInfo FileInformation {
            get { return this.currentFileInfo; }
            set { setFileInfoMatchers(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DoMatch() {
            bool res = false;
            switch (ValidateType) {
                case MatcherEnums.MatcherOperator.Or:
                    res = doOrMatch();
                    break;
                case MatcherEnums.MatcherOperator.And:
                    res = doAndMatch();
                    break;

            }

            return res;
        }

    }
}
