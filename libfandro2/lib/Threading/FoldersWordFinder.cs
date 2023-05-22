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
using libfandro2.lib.WinAPI;
using static System.Net.Mime.MediaTypeNames;
using libfandro2.lib.Finding;
using libfandro2.lib.Matching;
using System.ComponentModel.Design;
using libfandro2.lib.Interfaces;

namespace libfandro2.lib.Threading {
    public class FoldersWordFinder : BaseFileWordFinder {
        private string startingfolder = null;
        private bool recursive = false;
        private Form targetform = null;
        private FileFinder findfiler = null;
        private int count = 0;
        private string mask = null;


        /// <summary>
        /// 
        /// </summary>
        public bool Recursive {
            get { return this.recursive; }
            set { this.recursive = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Mask {
            get { return this.mask; }
            set { this.mask = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Form TargetForm {
            get { return this.targetform; }
            set { this.targetform = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string StartFolder {
            get { return this.startingfolder; }
            set { this.startingfolder = value; }
        }


        public FoldersWordFinder() {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopThread"></param>
        /// <param name="hasStoppedThread"></param>
        public FoldersWordFinder(ManualResetEvent stopthread, ManualResetEvent hasstoppedthread) : this() {
            // signals if thread is going to be stopped from UI
            this.StopWorkResetEvent = stopthread;
            // signals if thread has stopped internally
            this.FinishedWorkResetEvent = hasstoppedthread;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool IsOKToContinue() {
            return this.targetform != null && this.startingfolder != null &&  this.mask != null;
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
            this.FinishedWorkResetEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Findfiler_FolderProcessingEvent(object sender, FolderProcessingEventArgs e) {
            // pre entry check for reset event
            if (this.StopWorkResetEvent.WaitOne(0, true)) {
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
            if (this.StopWorkResetEvent.WaitOne(0, true)) {
                if (findfiler != null) {
                    e.Cancelled = true;
                }
            }

            // check pre conditions first and run against matchers.
            if (this.Conditions != null && this.Conditions.Count > 0) {
                this.Conditions.FileInformation = e.FileInfo;
                bconditions = this.Conditions.DoMatch();
            }
          

            // che
            // ^^^ Wait what was that comment for?
            if (this.StopWorkResetEvent.WaitOne(0, true)) {
                if (findfiler != null) {
                    e.Cancelled = true;
                }
            }

            updateStatusBarFilesFound(e.FileInfo.FullName + " " + (bconditions == true? "(match)" : "(no match)"));
            // we'll need at least data in the file...
            if (e.FileInfo.Length > 0 && bconditions == true) {
                if (!String.IsNullOrEmpty(this.Pattern)) {
                    count++;
                    long position = this.FindTextPointersLong(this.Pattern, e.FileInfo);
                    if (position > -1) {
                        updateListView(e.FileInfo, position);
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

            if (this.StopWorkResetEvent.WaitOne(0, true)) {
                if (findfiler != null) {
                    e.Cancelled = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute() {
            if (this.IsOKToContinue()) {

                // disable main controls
                updateThreadIsRunning(true);
                this.CurrentThread = new Thread(getAllFiles);
                this.CurrentThread.Name = "Fandro2_" + Guid.NewGuid();
                this.Duration = DateTime.Now;
                this.CurrentThread.Start();
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
                (targetform as IFandroFindForm).SetControlsWhileThreading(stopping);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        private void updateStatusBarFilesFound(string text) {
            if ((targetform as IFandroFindForm).StatusBar.InvokeRequired) {
                (targetform as IFandroFindForm).StatusBar.Invoke(new StringInvoker(updateStatusBarFilesFound), new object[] { text });
            }
            else {
                (targetform as IFandroFindForm).StatusBar.Items[0].Text = String.Format("{0}/{1}", 
                    (targetform as IFandroFindForm).FilesView.Items.Count, this.count);
                (targetform as IFandroFindForm).StatusBar.Items[2].Text = text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateStatusBarCount() {
            if ((targetform as IFandroFindForm).StatusBar.InvokeRequired) {
                (targetform as IFandroFindForm).StatusBar.Invoke(new MethodInvoker(updateStatusBarCount));
            }
            else {
                (targetform as IFandroFindForm).StatusBar.Items[0].Text = String.Format("{0}/{1}", 
                    (targetform as IFandroFindForm).FilesView.Items.Count, this.count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void initializeDurationTime() {
            if ((targetform as IFandroFindForm).StatusBar.InvokeRequired) {
                (targetform as IFandroFindForm).StatusBar.Invoke(new MethodInvoker(initializeDurationTime), new object[] { });

            }
            else {
                (targetform as IFandroFindForm).StatusBar.Items[1].Text = this.Duration.ToString("G");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void updateDurationTime() {
            if ((targetform as IFandroFindForm).StatusBar.InvokeRequired) {
                (targetform as IFandroFindForm).StatusBar.Invoke(new MethodInvoker(updateDurationTime), new object[] { });

            }
            else {
                TimeSpan p = DateTime.Now.Subtract(this.Duration);
                (targetform as IFandroFindForm).StatusBar.Items[1].Text = p.ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        private void updateListView(FileInfo info, long position) {
            if ((targetform as IFandroFindForm).FilesView.InvokeRequired) {
                (targetform as IFandroFindForm).FilesView.Invoke(new FileFindSystemInfoInvoker(updateListView), new object[] { info, position });
            }
            else {
                ListViewItem item = new ListViewItem();
                item.Text = info.Name;


                // get extension file icon
                string extension = info.Extension;


                // Image img = targetform.FilesView.SmallImageList.Images[extension];
                int i = -1;

                try {
                    bool b = (targetform as IFandroFindForm).FilesView.SmallImageList.Images.Keys.Contains(extension);
                    if (!b) {
                        Icon icon = Helpers.GetSmallIcon(info.FullName);
                        (targetform as IFandroFindForm).FilesView.SmallImageList.Images.Add(extension, icon);
                        i = (targetform as IFandroFindForm).FilesView.SmallImageList.Images.Count - 1;
                    }
                    else {
                        i = (targetform as IFandroFindForm).FilesView.SmallImageList.Images.IndexOfKey(extension);
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
                (targetform as IFandroFindForm).FilesView.Items.Add(item);
            }
        }

    }
}
