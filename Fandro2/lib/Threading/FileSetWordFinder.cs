using Fandro2.lib.Matching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Fandro2.lib.Threading {
    public class FileSetWordFinder {
        List<string> fileset = new List<string>();
        private ManualResetEvent stopThread;
        private ManualResetEvent threadHasStopped;
        private string pattern = null;
        private bool casesensitive = false;
        private DateTime duration = DateTime.MinValue;
        private FileFilters conditions;
        private Form targetform;
        private Thread nthread = null;
        private int count = 0;

        public FileSetWordFinder() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileset"></param>
        /// <param name="stopThread"></param>
        /// <param name="threadHasStopped"></param>
        public FileSetWordFinder(List<string> fileset, ManualResetEvent stopThread, ManualResetEvent threadHasStopped) { 
            this.fileset = fileset;
            this.stopThread = stopThread;
            this.threadHasStopped = threadHasStopped;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Pattern { 
            get { return pattern; } 
            set {  pattern = value; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CaseSensitive {
            get { return casesensitive; }
            set { casesensitive = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FileFilters Conditions {
            get { return this.conditions; }
            set { this.conditions = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Thread CurrentThread {
            get { return nthread; }
            set { this.nthread = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool isOKToContinue() {
            return targetform != null && pattern != null && this.fileset.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="accessor"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        unsafe private long boyerMooreHorspoolPointersLong(string text, MemoryMappedViewAccessor accessor, long size) {
            long ret = 0;

            if (!casesensitive) {
                pattern = pattern.ToUpper();
            }

            byte* ptrMemMap = (byte*)0;

            accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptrMemMap);


            int[] bad_shift = new int[char.MaxValue + 1];


            for (int i = 0; i < char.MaxValue + 1; i++) {
                bad_shift[i] = pattern.Length;
            }

            int last = pattern.Length - 1;

            for (int i = 0; i < last; i++) {
                bad_shift[pattern[i]] = last - i;
            }

            int patlength = pattern.Length;
            long pos = patlength - 1;
            char lastchar;
            int numskip = 0;
            bool found = false;

            if (pos != 0) {
                ret = -1;
                lastchar = pattern[patlength - 1];
                while (pos < size) {

                    if (stopThread.WaitOne(0, true)) {
                        ret = -666;
                        break;
                    }
                    char tmpchar = '0';

                    if (!casesensitive) {
                        tmpchar = char.ToUpper(Convert.ToChar(ptrMemMap[pos]));
                    }
                    else {
                        tmpchar = Convert.ToChar(ptrMemMap[pos]);
                    }



                    if (tmpchar != lastchar) {
                        numskip = bad_shift[tmpchar];
                    }
                    else {
                        int i = patlength - 1;
                        found = true;
                        numskip = patlength;
                        while (i > 0) {
                            pos--;

                            if (!casesensitive) {
                                tmpchar = char.ToUpper(Convert.ToChar(ptrMemMap[pos]));
                            }
                            else {
                                tmpchar = Convert.ToChar(ptrMemMap[pos]);
                            }

                            if (tmpchar != pattern[i - 1]) {
                                found = false;
                                numskip = patlength - i + bad_shift[lastchar];
                                break;
                            }
                            i--;
                        }
                    }

                    if (found) {
                        ret = pos;
                        return ret;
                    }
                    pos += numskip;

                }

            }


            return ret;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual long findTextPointersLong(string text, FileInfo file) {
            long ret = -1;

            try {
                using (MemoryMappedFile memfile = MemoryMappedFile.CreateFromFile(
                      file.FullName, FileMode.Open, null, 0, MemoryMappedFileAccess.Read)) {
                    try {
                        using (MemoryMappedViewAccessor accessor = memfile.CreateViewAccessor(
                            0, file.Length, MemoryMappedFileAccess.Read)) {
                            // do your fancy Boyer-Moore-Horspool search against the memory mapped file/pointers
                            ret = boyerMooreHorspoolPointersLong(text, accessor, file.Length);
                        }
                    }
                    catch (Exception ex) {
                        Trace.TraceError(ex.Message + " -- " + file.FullName + " - " + file.Length);
                    }
                }
            }
            catch (Exception ex) {
                Trace.TraceError(ex.Message + " -- " + file.FullName + " - " + file.Length);
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Execute() {
            if (this.isOKToContinue()) {
                this.nthread = new Thread(doWork);
                nthread.Start();
                nthread.Name = "Fandro2_fileset_" + Guid.NewGuid();
                duration = DateTime.Now;
                nthread.Start();
                this.doWork();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void doWork() {
            this.initializeDurationTime();
            foreach (String s in  this.fileset) {
                if (stopThread.WaitOne(0, true)) {
                    // AHTODO
                    break;
                }

                if (File.Exists(s)) {


                    FileInfo p = new FileInfo(s);
                    bool bconditions = true;
                    // do all the legwork...
                    // check pre conditions first and run against matchers.
                    if (this.conditions != null && this.conditions.Count > 0) {
                        this.conditions.FileInformation = p;
                        bconditions = this.conditions.DoMatch();
                    }

                    if (p.Length > 0 && bconditions == true) {
                        if (!String.IsNullOrEmpty(pattern)) {
                            long position = this.findTextPointersLong(pattern, p);
                            if (position > -1) {
                                //updateListView(e.FileInfo, position);
                                count++;
                            }
                            else {
                                if (position == -666) {
                                    // user signal..
                                    // need to test this...
                                    // AHTODO
                                }
                            }
                        }

                        if (stopThread.WaitOne(0, true)) {
                            // AHTODO
                            break;
                        }
                    }
                }

                if (stopThread.WaitOne(0, true)) {
                    // AHTODO
                    break;
                }

            }

            updateStatusBarCount();
            updateDurationTime();
            updateThreadIsRunning(false);
            threadHasStopped.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void updateThreadIsRunning(bool v) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void updateDurationTime() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void updateStatusBarCount() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void initializeDurationTime() {
            throw new NotImplementedException();
        }
    }
}
