using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Fandro2.lib;
using System.Threading;
using System.IO;
using Fandro2.lib.WinAPI;
using Fandro2.lib.Finding;
using Fandro2.lib.Threading;
using System.Linq.Expressions;
using lib.Controls.Conditions;
using lib.Controls.Conditions.Classes;
using System.Collections;
using Fandro2.lib.Controls.Folders;
using Fandro2.lib.Interfaces;

namespace Fandro2 {
    public partial class mainForm : Form, IFandroFindForm {
        private ManualResetEvent userstoppedevent = new ManualResetEvent(false);
        private ManualResetEvent processstoppedevent = new ManualResetEvent(false);
        FoldersWordFinder finder = null;
        FindOptions findoptions = null;
        HelpProvider helpProvider = null;

        /// <summary>
        /// 
        /// </summary>
        public mainForm() {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public mainForm(FindOptions options) {
            InitializeComponent();
            this.findoptions = options;
        }


        /// <summary>
        /// 
        /// </summary>
        private void initializeMainForm() {
            loadGeneralSettings();
            this.enableSearchButtons();
            this.lvwSearchResults.DoubleBuffering(true);
            this.ftextFolders.FolderTextBox.TextChanged += FolderTextBox_TextChanged;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void FolderTextBox_TextChanged(object sender, EventArgs e) {
            enableSearchButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        private void setupFindOptions() {
            if (this.findoptions.TargetFolder != null) {
                if (this.findoptions.TargetFolder.Contains(";")) {
                    this.ftextFolders.SelectedFolders = this.findoptions.TargetFolder.Split(';').ToList();
                }
                else {
                    this.ftextFolders.SelectedFolder = this.findoptions.TargetFolder;
                }
            }

            this.cboFileMask.Text = this.findoptions.FileMask;
            this.cboPattern.Text = this.findoptions.Pattern;

            if (this.findoptions.SearchOptions.HasFlag(FileSearchOptions.CaseSensitive)) {
                this.chkCaseSensitive.Checked = true;
            }

            if (this.findoptions.SearchOptions.HasFlag(FileSearchOptions.Recursive)) {
                this.chkSubfolders.Checked = true;
            }

            if (this.findoptions.Execute) {
                this.startSearch();
            }
        }


        private void mainForm_Load(object sender, EventArgs e) {
            this.initializeMainForm();

            if (this.findoptions != null) {
                setupFindOptions();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ListView FilesView {
            get { return this.lvwSearchResults; }
            set { this.lvwSearchResults = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public StatusStrip StatusBar {
            get { return this.mainStatusStrip; }
            set { this.mainStatusStrip = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public ComboBox ComboBoxFileMask {
            get { return this.cboFileMask; }
            set { this.cboFileMask = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public ComboBox ComboBoxSearchPattern {
            get { return this.cboPattern; }
            set { this.cboPattern = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextBox EditBoxFolders {
            get { return null; }
            set {
                // AHTODO - we should hide some of the usercontrol properties...
                this.ftextFolders.FolderTextBox = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<ListViewItem> FoundFileItems {
            get {
                return this.lvwSearchResults.Items.Cast<ListViewItem>().ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<ListViewItem> SelectedFileItems {
            get {
                return this.lvwSearchResults.SelectedItems.Cast<ListViewItem>().ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startToolStripMenuItem_Click(object sender, EventArgs e) {
            this.startSearch();
        }


        /// <summary>
        /// 
        /// </summary>
        private void clearScreen() {
            this.cboFileMask.Text = "";
            // this should clear everything...
            this.ftextFolders.SelectionMode = lib.Controls.Folders.FileTextSelectionBox.FolderSelectionMode.SingleFolder;
            this.cboPattern.Text = "";
            this.lvwSearchResults.Items.Clear();
            this.chkCaseSensitive.Checked = false;
            this.chkSubfolders.Checked = true;

            // clear the grid...
            this.gridControls.ClearDataRows();
            this.rdBtnOr.Checked = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void startSearch() {

            if (cboFileMask.Text.Length > 0) {
                Helpers.AddItemToHistoryList(cboFileMask.Text, cboFileMask.Items, Config.Settings.MaskMaxItems);
            }

            if (cboPattern.Text.Length > 0) {
                Helpers.AddItemToHistoryList(cboPattern.Text, cboPattern.Items, Config.Settings.PatternMaxItems);
            }

            if (Control.ModifierKeys != Keys.Shift) {
                lvwSearchResults.Items.Clear();
            }

            if (this.cboFileMask.Text == "") {
                this.cboFileMask.Text = "*.*";
            }


            processstoppedevent.Reset();
            userstoppedevent.Reset();

            this.finder = new FoldersWordFinder(userstoppedevent, processstoppedevent);


            finder.Mask = this.cboFileMask.Text;
            finder.Recursive = this.chkSubfolders.Checked;

            finder.StartFolder = this.ftextFolders.SelectionMode == lib.Controls.Folders.FileTextSelectionBox.FolderSelectionMode.SingleFolder ?
                this.ftextFolders.SelectedFolder : String.Join(";", this.ftextFolders.SelectedFolders);
            finder.TargetForm = this;
            finder.Pattern = cboPattern.Text;
            finder.CaseSensitive = chkCaseSensitive.Checked;

            // set conditions
            finder.Conditions = gridControls.Items.ToMatcher(
                rdBtnOr.Checked ? lib.Matching.MatcherEnums.MatcherOperator.Or :
                    lib.Matching.MatcherEnums.MatcherOperator.And);

            // run finder thread
            finder.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        private void stopSearch() {
            if (this.finder != null) {
                if (this.finder.CurrentThread != null && this.finder.CurrentThread.IsAlive == true) {

                    userstoppedevent.Set();

                    while (this.finder.CurrentThread.IsAlive) {
                        if (WaitHandle.WaitAll(new ManualResetEvent[] { processstoppedevent }, 100, true)) {
                            break;
                        }
                        Application.DoEvents();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="combobox"></param>
        /// <param name="strings"></param>
        private void loadHistoryLists(ComboBox combobox, String strings) {
            string[] arraystring = strings.Split(',');
            combobox.Items.AddRange(arraystring);
        }


        /// <summary>
        /// 
        /// 
        /// </summary>
        private void loadGeneralSettings() {
            loadHistoryLists(cboFileMask, Config.Settings.MasksHistory);
            loadHistoryLists(cboPattern, Config.Settings.PatternHistory);

            if (Config.Settings.DirectoryHistory != null && Config.Settings.DirectoryHistory.Length > 0) {
                this.ftextFolders.RecentlyUsedFolders = Config.Settings.DirectoryHistory.Split(",").ToList();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void saveGeneralSettings() {
            Config.Settings.MasksHistory = Helpers.HistoryListToString(cboFileMask.Items);
            Config.Settings.PatternHistory = Helpers.HistoryListToString(cboPattern.Items);

            List<String> list = ftextFolders.RecentlyUsedFolders;

            if (list != null && list.Count > 0) {
                Config.Settings.DirectoryHistory = String.Join(',', list);
            }

            Config.Settings.Save();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void SetControlsWhileThreading(bool stopping) {
            startToolStripMenuItem.Enabled = (!stopping);
            startSearchToolStripButton.Enabled = (!stopping);
            stopToolStripMenuItem.Enabled = (stopping);
            stopSearchToolStripButton.Enabled = (stopping);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopToolStripMenuItem_Click(object sender, EventArgs e) {
            this.stopSearch();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startSearchToolStripButton_Click(object sender, EventArgs e) {
            this.startSearch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopSearchToolStripButton_Click(object sender, EventArgs e) {
            this.stopSearch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwSearchResults_MouseUp(object sender, MouseEventArgs e) {
            // only when selected...
            if (lvwSearchResults.SelectedItems.Count > 0) {
                if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                    if (Control.ModifierKeys == Keys.Shift) {
                        ///    ShellContextMenu scm = new ShellContextMenu();
                        ///    FileInfo[] files = new FileInfo[1];
                        ///    files[0] = new FileInfo(@"c:\windows\notepad.exe");
                        ///    scm.ShowContextMenu(this.Handle, files, Cursor.Position);

                        ShellContextMenu scm = new ShellContextMenu();
                        try {
                            ListViewItem item = lvwSearchResults.SelectedItems[0];
                            String filepath = Path.Combine(item.SubItems[1].Text, item.Text);
                            FileInfo[] files = new FileInfo[1];
                            files[0] = new FileInfo(filepath);

                            scm.ShowContextMenu(files, lvwSearchResults.PointToScreen(e.Location));

                        }
                        finally {
                        }


                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainForm_FormClosed(object sender, FormClosedEventArgs e) {
            this.saveGeneralSettings();

            if (this.helpProvider != null) {
                this.helpProvider.Dispose();
            }

            if (this.finder != null && this.finder.CurrentThread != null
                && this.finder.CurrentThread.IsAlive) {

                this.stopSearch();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerShellObjectToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void unregisterShellObjectToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearToolStripMenuItem_Click(object sender, EventArgs e) {
            this.clearScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool canStartSearch() {
            return (ftextFolders.HasFolderText && this.cboFileMask.Text != "");
        }

        /// <summary>
        /// 
        /// </summary>
        private void enableSearchButtons() {
            this.startSearchToolStripButton.Enabled = canStartSearch();
            this.startToolStripMenuItem.Enabled = canStartSearch();
            this.stopSearchToolStripButton.Enabled = !canStartSearch() && this.startSearchToolStripButton.Enabled;
            this.stopToolStripMenuItem.Enabled = !canStartSearch() && this.startSearchToolStripButton.Enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboFileMask_TextChanged(object sender, EventArgs e) {
            enableSearchButtons();
        }

        private void txtFolders_TextChanged(object sender, EventArgs e) {
            enableSearchButtons();
        }

        private void cboPattern_TextChanged(object sender, EventArgs e) {
            enableSearchButtons();
        }


        private List<FilePropertyItem> setupFileProperties() {
            List<FilePropertyItem> items = new List<FilePropertyItem>();

            items.Add(new FilePropertyItem {
                Name = "Creation date",
                MatcherType = lib.Matching.MatcherEnums.MatcherType.FileCreateTime,
                ValueType = typeof(DateTime)
            });

            items.Add(new FilePropertyItem {
                Name = "Modification date",
                MatcherType = lib.Matching.MatcherEnums.MatcherType.FileModTime,
                ValueType = typeof(DateTime)
            });

            items.Add(new FilePropertyItem {
                Name = "Access date",
                MatcherType = lib.Matching.MatcherEnums.MatcherType.FileAccessTime,
                ValueType = typeof(DateTime)
            });

            items.Add(new FilePropertyItem {
                Name = "Size",
                MatcherType = lib.Matching.MatcherEnums.MatcherType.FileSize,
                ValueType = typeof(Int32)
            });

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<OperatorItem> setupOperatorItems() {
            List<OperatorItem> items = new List<OperatorItem>();

            items.Add(new OperatorItem {
                Name = "equals to",
                Operator = lib.Matching.MatcherEnums.MatcherAction.Equals
            });

            items.Add(new OperatorItem {
                Name = "not equals to",
                Operator = lib.Matching.MatcherEnums.MatcherAction.NotEquals
            });

            items.Add(new OperatorItem {
                Name = "greater than",
                Operator = lib.Matching.MatcherEnums.MatcherAction.Greater

            });

            items.Add(new OperatorItem {
                Name = "less than",
                Operator = lib.Matching.MatcherEnums.MatcherAction.Less
            });

            return items;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e) {
            SelectableDataRow dataRow = new SelectableDataRow();
            dataRow.FilePropertyItems = setupFileProperties();
            dataRow.OperatorItems = setupOperatorItems();
            this.gridControls.AddDataRow(dataRow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e) {
            // get the last row
            this.gridControls.RemoveDataRow(gridControls.RowCount - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearAll_Click(object sender, EventArgs e) {
            this.gridControls.ClearDataRows();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            aboutForm n = new aboutForm();
            try {
                n.StartPosition = FormStartPosition.CenterParent;
                n.ShowDialog();
            }
            finally {
                if (n != null) {
                    n.Dispose();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextListViewMenu_Opening(object sender, CancelEventArgs e) {
            if (this.lvwSearchResults.SelectedItems.Count == 0) {
                e.Cancel = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileToolStripMenuItem1_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in this.lvwSearchResults.SelectedItems) {
                string s = Path.Combine(item.SubItems[1].Text, item.SubItems[0].Text);

                if (File.Exists(s)) {
                    Helpers.StartApplication(s);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void folderToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem i in this.lvwSearchResults.SelectedItems) {
                string folder = i.SubItems[1].Text;

                Hashtable table = new Hashtable();
                try {
                    if (!table.ContainsKey(folder)) {
                        table.Add(folder, folder);
                        if (Directory.Exists(folder)) {
                            Helpers.StartApplication(folder);
                        }
                    }
                }
                catch {
                    ///
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void explorerMenuToolStripMenuItem_Click(object sender, EventArgs e) {

            if (this.lvwSearchResults.SelectedItems.Count == 1) {
                ShellContextMenu scm = new ShellContextMenu();
                try {
                    ListViewItem item = lvwSearchResults.SelectedItems[0];
                    String filepath = Path.Combine(item.SubItems[1].Text, item.Text);
                    FileInfo[] files = new FileInfo[1];
                    files[0] = new FileInfo(filepath);

                    scm.ShowContextMenu(files, Cursor.Position);
                }
                catch {

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.lvwSearchResults.SelectedItems.Count == 1) {
                string s = Path.Combine(this.lvwSearchResults.SelectedItems[0].SubItems[1].Text,
                    this.lvwSearchResults.SelectedItems[0].SubItems[0].Text);

                if (File.Exists(s)) {
                    Helpers.ShowPropertyDialogBox(s);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clipboardToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.lvwSearchResults.Items.Count > 0) {
                Clipboard.SetText(this.lvwSearchResults.ListViewDataToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileToolStripMenuItem3_Click(object sender, EventArgs e) {
            if (this.lvwSearchResults.Items.Count > 0) {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                try {
                    saveFileDialog.Filter = "*.csv|CSV files (*.csv)|*.*|All files (*.*)";
                    DialogResult res = saveFileDialog.ShowDialog();

                    if (res == DialogResult.OK) {
                        System.IO.File.WriteAllText(saveFileDialog.FileName,
                            this.lvwSearchResults.ListViewDataToString());
                    }
                }
                catch (Exception ex) {
                    // AHTODO
                }
                finally {
                    if (saveFileDialog != null) {
                        saveFileDialog.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileClearToolStripButton_Click(object sender, EventArgs e) {
            this.clearScreen();
        }


        /// <summary>
        /// 
        /// </summary>
        private void activeControlSelectAll() {
            if (this.ActiveControl is SplitContainer) {
                SplitContainer p = this.ActiveControl as SplitContainer;
                if (p.ActiveControl is TextBox) {
                    (p.ActiveControl as TextBox).SelectAll();
                }
                else if (p.ActiveControl is FileTextSelectionBox) {
                    (p.ActiveControl as FileTextSelectionBox).FolderTextBox.SelectAll();
                }
                else if (p.ActiveControl is ComboBox) {
                    (p.ActiveControl as ComboBox).SelectAll();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void activeControlCut() {
            if (this.ActiveControl is SplitContainer) {
                SplitContainer p = this.ActiveControl as SplitContainer;
                if (p.ActiveControl is TextBox) {
                    (p.ActiveControl as TextBox).Cut();
                }
                else if (p.ActiveControl is FileTextSelectionBox) {
                    (p.ActiveControl as FileTextSelectionBox).FolderTextBox.Cut();
                }
                else if (p.ActiveControl is ComboBox) {
                    ComboBox pcomb = p.ActiveControl as ComboBox;
                    int i = pcomb.SelectionStart;
                    if (pcomb.SelectionLength > 0) {
                        string st = pcomb.Text;
                        if (st.Length > 0) {
                            string removed = st.Substring(pcomb.SelectionStart, pcomb.SelectionLength);
                            Clipboard.SetText(removed);
                            pcomb.Text = pcomb.Text.Replace(removed, "");
                            pcomb.SelectionStart = i;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void activeControlCopy() {
            if (this.ActiveControl is SplitContainer) {
                SplitContainer p = this.ActiveControl as SplitContainer;
                if (p.ActiveControl is TextBox) {
                    (p.ActiveControl as TextBox).Copy();
                }
                else if (p.ActiveControl is FileTextSelectionBox) {
                    (p.ActiveControl as FileTextSelectionBox).FolderTextBox.Copy();
                }
                else if (p.ActiveControl is ComboBox) {
                    ComboBox pcomb = p.ActiveControl as ComboBox;
                    int i = pcomb.SelectionStart;
                    if (pcomb.SelectionLength > 0) {
                        string st = pcomb.Text;
                        if (st.Length > 0) {
                            string copied = st.Substring(pcomb.SelectionStart, pcomb.SelectionLength);
                            Clipboard.SetText(copied);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void activeControlPaste() {
            if (this.ActiveControl is SplitContainer) {
                SplitContainer p = this.ActiveControl as SplitContainer;
                if (p.ActiveControl is TextBox) {
                    (p.ActiveControl as TextBox).Paste();
                }
                else if (p.ActiveControl is FileTextSelectionBox) {
                    (p.ActiveControl as FileTextSelectionBox).FolderTextBox.Paste();
                }
                else if (p.ActiveControl is ComboBox) {
                    ComboBox pcomb = p.ActiveControl as ComboBox;
                    int i = pcomb.SelectionStart;
                    string pastetext = Clipboard.GetText();
                    if (pcomb.SelectionLength > 0) {
                        string selectionstart = pcomb.Text.Substring(pcomb.SelectionStart, pcomb.SelectionLength);
                        pcomb.Text = pcomb.Text.Replace(selectionstart, pastetext);
                        pcomb.SelectionStart = i;
                        pcomb.SelectionLength = pastetext.Length;
                    }
                    else {
                        pcomb.Text = pcomb.Text.Substring(0, i - 1) + pastetext
                            + pcomb.Text.Substring(pcomb.SelectionStart);

                        pcomb.SelectionStart = 0;
                        pcomb.SelectionLength = pcomb.Text.Length;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            this.activeControlSelectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            this.activeControlCut();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editCutToolStripButton_Click(object sender, EventArgs e) {
            this.activeControlCut();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            this.activeControlCopy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editCopyToolStripButton_Click(object sender, EventArgs e) {
            this.activeControlCopy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            this.activeControlPaste();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editPasteToolStripButton_Click(object sender, EventArgs e) {
            this.activeControlPaste();
        }
    }
}
