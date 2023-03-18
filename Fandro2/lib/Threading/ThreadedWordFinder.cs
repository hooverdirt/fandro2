using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Fandro2.lib.WinAPI;
using static System.Net.Mime.MediaTypeNames;
using Fandro2.lib.Finding;
using Fandro2.lib.Matching;
using System.ComponentModel.Design;

namespace Fandro2.lib.Threading {
    public class ThreadedWordFinder {
        private ManualResetEvent stopThread;
        private ManualResetEvent threadHasStopped;
        private string startingfolder = null;
        private string mask = null;
        private bool recursive = false;
        private mainForm targetform = null;
        private Thread nthread = null;
        private string pattern = null;
        private bool casesensitive = true;
        private DateTime duration = DateTime.MinValue;
        // private FindFile findfiler = null;
        private FileFinder findfiler = null;
        private FileFilters conditions = null;

        private int count = 0;


        /// <summary>
        /// 
        /// </summary>
        public string StartFolder {
            get { return startingfolder; }
            set { startingfolder = value; }
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
        public string Mask {
            get { return mask; }
            set { mask = value; }
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
        public bool Recursive {
            get { return recursive; }
            set { recursive = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public mainForm TargetForm {
            get { return targetform; }
            set { targetform = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FileFilters Conditions {
            get { return this.conditions; }
            set { this.conditions = value; }
        }

        public ThreadedWordFinder() {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopThread"></param>
        /// <param name="hasStoppedThread"></param>
        public ThreadedWordFinder(ManualResetEvent stopthread, ManualResetEvent hasstoppedthread) : this() {
            // signals if thread is going to be stopped from UI
            stopThread = stopthread;
            // signals if thread has stopped internally
            threadHasStopped = hasstoppedthread;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool okToContinue() {
            return targetform != null && startingfolder != null && mask != null;
        }

        /// <summary>
        /// Long variant...
        /// </summary>
        /// <param name="text"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private long findTextPointersLong(string text, FileInfo file) {
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
        /// This is the real deal!
        /// </summary>
        private void getAllFiles() {
            initializeDurationTime();
            count = 0;
            findfiler = new FileFinder();
            try {
                findfiler.TargetFolder = startingfolder;
                findfiler.FileMask = mask;
                findfiler.Recursive = recursive;
                findfiler.FileFoundEvent += Findfiler_FileFoundEvent;
                findfiler.FolderProcessingEvent += Findfiler_FolderProcessingEvent;
                findfiler.Execute();
            }
            finally {

                if (findfiler != null) {
                    findfiler.FileFoundEvent -= Findfiler_FileFoundEvent;
                    findfiler.FolderProcessingEvent -= Findfiler_FolderProcessingEvent;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Findfiler_FolderProcessingEvent(object sender, FolderProcessingEventArgs e) {
            // pre entry check for reset event
            if (stopThread.WaitOne(0, true)) {
                if (findfiler != null) {
                    e.Cancelled = true;
                }
            }

            this.updateStatusBarFilesFound("Processing: " + e.DirectoryInfo.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Findfiler_FileFoundEvent(object sender, FileFoundEventArgs e) {
            bool bconditions = true;

            // pre entry check for reset event
            if (stopThread.WaitOne(0, true)) {
                if (findfiler != null) {
                    e.Cancelled = true;
                }
            }

            // check pre conditions first and run against matchers.
            if (this.conditions != null && this.conditions.Count > 0) {
                this.conditions.FileInformation = e.FileInfo;
                bconditions = this.conditions.DoMatch();
            }
          

            // che
            // ^^^ Wait what was that comment for?
            if (stopThread.WaitOne(0, true)) {
                if (findfiler != null) {
                    e.Cancelled = true;
                }
            }

            updateStatusBarFilesFound(e.FileInfo.FullName + " " + (bconditions == true? "(match)" : "(no match)"));
            // we'll need at least data in the file...
            if (e.FileInfo.Length > 0 && bconditions == true) {
                if (!String.IsNullOrEmpty(pattern)) {
                    long position = findTextPointersLong(pattern, e.FileInfo);
                    if (position > -1) {
                        updateListView(e.FileInfo, position);
                        count++;
                    }
                    else {
                        if (position == -666) {
                            // user signal..
                            // need to test this...
                            e.Cancelled = true;
                        }
                    }
                }
                else {
                    updateListView(e.FileInfo, -1);
                }
            }
            // otherwise ignore...

            if (stopThread.WaitOne(0, true)) {
                if (findfiler != null) {
                    e.Cancelled = true;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Thread CurrentThread {
            get { return nthread; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Execute() {
            if (okToContinue()) {

                // disable main controls
                updateThreadIsRunning(true);
                nthread = new Thread(getAllFiles);
                nthread.Name = "Fandro2_" + Guid.NewGuid();
                duration = DateTime.Now;
                nthread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateThreadIsRunning(bool stopping) {
            if (targetform.InvokeRequired) {
                targetform.Invoke(new BooleanInvoker(updateThreadIsRunning), new object[] { stopping });
            }
            else {
                targetform.SetControlsWhileThreading(stopping);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        private void updateStatusBarFilesFound(string text) {
            if (targetform.StatusBar.InvokeRequired) {
                targetform.StatusBar.Invoke(new StringInvoker(updateStatusBarFilesFound), new object[] { text });
            }
            else {
                targetform.StatusBar.Items[0].Text = targetform.FilesView.Items.Count.ToString();
                targetform.StatusBar.Items[2].Text = text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateStatusBarCount() {
            if (targetform.StatusBar.InvokeRequired) {
                targetform.StatusBar.Invoke(new MethodInvoker(updateStatusBarCount));
            }
            else {
                targetform.StatusBar.Items[0].Text = targetform.FilesView.Items.Count.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void initializeDurationTime() {
            if (targetform.StatusBar.InvokeRequired) {
                targetform.StatusBar.Invoke(new MethodInvoker(initializeDurationTime), new object[] { });

            }
            else {
                targetform.StatusBar.Items[1].Text = duration.ToString("G");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void updateDurationTime() {
            if (targetform.StatusBar.InvokeRequired) {
                targetform.StatusBar.Invoke(new MethodInvoker(updateDurationTime), new object[] { });

            }
            else {
                TimeSpan p = DateTime.Now.Subtract(duration);
                targetform.StatusBar.Items[1].Text = p.ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        private void updateListView(FileInfo info, long position) {
            if (targetform.FilesView.InvokeRequired) {
                targetform.FilesView.Invoke(new FileFindSystemInfoInvoker(updateListView), new object[] { info, position });
            }
            else {
                ListViewItem item = new ListViewItem();
                item.Text = info.Name;


                // get extension file icon
                string extension = info.Extension;


                // Image img = targetform.FilesView.SmallImageList.Images[extension];
                int i = -1;

                try {
                    bool b = targetform.FilesView.SmallImageList.Images.Keys.Contains(extension);
                    if (!b) {
                        Icon icon = Helpers.GetSmallIcon(info.FullName);
                        targetform.FilesView.SmallImageList.Images.Add(extension, icon);
                        i = targetform.FilesView.SmallImageList.Images.Count - 1;
                    }
                    else {
                        i = targetform.FilesView.SmallImageList.Images.IndexOfKey(extension);
                    }
                }
                catch (Exception ex) {

                }
                item.ImageIndex = i;

                long filesize = info.Length;

                item.SubItems.Add(Path.GetDirectoryName(info.FullName));
                item.SubItems.Add(position > -1 ? position.ToString() : "N/A");
                item.SubItems.Add(filesize.ToString());
                item.SubItems.Add(info.LastWriteTimeUtc.ToLocalTime().ToString());
                item.SubItems.Add(info.CreationTimeUtc.ToLocalTime().ToString());
                item.SubItems.Add(info.LastAccessTimeUtc.ToLocalTime().ToString());
                targetform.FilesView.Items.Add(item);
            }
        }

    }
}
