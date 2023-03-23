using Fandro2.lib.Interfaces;
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
    public class FileSetWordFinder : BaseFileWordFinder {
        List<string> fileset = new List<string>();
        private IFandroFindForm targetform;

        public FileSetWordFinder() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileset"></param>
        /// <param name="stopThread"></param>
        /// <param name="threadHasStopped"></param>
        public FileSetWordFinder(List<string> fileset, ManualResetEvent stopThread, ManualResetEvent threadHasStopped) { 
            this.fileset = fileset;
            this.StopWorkResetEvent = stopThread;
            this.FinishedWorkResetEvent = threadHasStopped;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool IsOKToContinue() {
            return this.targetform != null && this.Pattern != null && this.fileset.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="accessor"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        unsafe virtual protected long boyerMooreHorspoolPointersLong(string text, MemoryMappedViewAccessor accessor, long size) {
            long ret = 0;

            if (!this.CaseSensitive) {
                this.Pattern = this.Pattern.ToUpper();
            }

            byte* ptrMemMap = (byte*)0;

            accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptrMemMap);


            int[] bad_shift = new int[char.MaxValue + 1];


            for (int i = 0; i < char.MaxValue + 1; i++) {
                bad_shift[i] = this.Pattern.Length;
            }

            int last = this.Pattern.Length - 1;

            for (int i = 0; i < last; i++) {
                bad_shift[this.Pattern[i]] = last - i;
            }

            int patlength = this.Pattern.Length;
            long pos = patlength - 1;
            char lastchar;
            int numskip = 0;
            bool found = false;

            if (pos != 0) {
                ret = -1;
                lastchar = this.Pattern[patlength - 1];
                while (pos < size) {

                    if (this.StopWorkResetEvent.WaitOne(0, true)) {
                        ret = -666;
                        break;
                    }
                    char tmpchar = '0';

                    if (!this.CaseSensitive) {
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

                            if (!this.CaseSensitive) {
                                tmpchar = char.ToUpper(Convert.ToChar(ptrMemMap[pos]));
                            }
                            else {
                                tmpchar = Convert.ToChar(ptrMemMap[pos]);
                            }

                            if (tmpchar != this.Pattern[i - 1]) {
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
        public override void Execute() {
            if (this.IsOKToContinue()) {
                this.CurrentThread = new Thread(this.DoWork);
                this.CurrentThread.Name = "Fandro2_fileset_" + Guid.NewGuid();
                this.Duration = DateTime.Now;
                this.CurrentThread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void DoWork() {
            this.initializeDurationTime();
            foreach (String s in  this.fileset) {
                if (this.StopWorkResetEvent.WaitOne(0, true)) {
                    // AHTODO
                    break;
                }

                if (File.Exists(s)) {


                    FileInfo p = new FileInfo(s);
                    bool bconditions = true;
                    // do all the legwork...
                    // check pre conditions first and run against matchers.
                    if (this.Conditions != null && this.Conditions.Count > 0) {
                        this.Conditions.FileInformation = p;
                        bconditions = this.Conditions.DoMatch();
                    }

                    if (p.Length > 0 && bconditions == true) {
                        if (!String.IsNullOrEmpty(this.Pattern)) {
                            long position = this.findTextPointersLong(this.Pattern, p);
                            if (position > -1) {
                                //updateListView(e.FileInfo, position);
                                this.Count++;
                            }
                            else {
                                if (position == -666) {
                                    // user signal..
                                    // need to test this...
                                    // AHTODO
                                }
                            }
                        }

                        if (this.StopWorkResetEvent.WaitOne(0, true)) {
                            // AHTODO
                            break;
                        }
                    }
                }

                if (this.StopWorkResetEvent.WaitOne(0, true)) {
                    // AHTODO
                    break;
                }

            }

            updateStatusBarCount();
            updateDurationTime();
            updateThreadIsRunning(false);
            this.FinishedWorkResetEvent.Set();
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
