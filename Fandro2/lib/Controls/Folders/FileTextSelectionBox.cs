using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fandro2.lib.Controls.Folders {
    public partial class FileTextSelectionBox : UserControl {
        /// <summary>
        ///
        /// </summary>
        public enum FolderSelectionMode {
            SingleFolder,
            MultipleFolders
        }


        private int maxmruitems = 20;
        private List<string> multifiles = new List<string>();

        public FileTextSelectionBox() {
            InitializeComponent();
            this.txtFolder.Top = 2;
            this.toolStripFileModes.Top = 2;
        }

        /// <summary>
        /// 
        /// </summary>
        private void setupMRUItems(List<string> list) {
            tbbRecentFolders.DropDownItems.Clear();

            if (list.Count > 0) {
                foreach (string folder in list) {
                    // Add shell image in near future?
                    if (folder != "" && this.tbbRecentFolders.DropDownItems[folder] == null) {
                        this.addMRUItem(folder);
                    }
                }
            }
        }

        private void Tiem_Click(object sender, EventArgs e) {
            if (sender is not null && sender is ToolStripItem) {
                if (this.SelectionMode == FolderSelectionMode.SingleFolder) {
                    this.txtFolder.Text = (sender as ToolStripItem).Text;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void removeMaxMRUItems() {
            while (this.tbbRecentFolders.DropDownItems.Count > this.maxmruitems) {
                this.tbbRecentFolders.DropDownItems.RemoveAt(0);
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private bool hasFolder(string folder) {
            bool ret = false;
            foreach (ToolStripMenuItem i in this.tbbRecentFolders.DropDownItems) {
                if (i.Text == folder) {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        private void addMRUItem(string folder) {
            if (folder != "" && !this.hasFolder(folder)) {
                ToolStripMenuItem tiem = new ToolStripMenuItem();
                tiem.Text = folder;
                tiem.Tag = folder;
                tiem.Click += Tiem_Click;
                this.tbbRecentFolders.DropDownItems.Add(tiem);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FolderSelectionMode getFolderSelectionMode() {
            return (this.tbbSingleFileMode.Checked == true ? FolderSelectionMode.SingleFolder : FolderSelectionMode.MultipleFolders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        private void setFolderSelectionMode(FolderSelectionMode mode) {
            this.txtFolder.Text = "";
            if (mode == FolderSelectionMode.SingleFolder) {
                this.tbbSingleFileMode.Checked = true;
            }
            else {
                this.tbbMultiFileMode.Checked = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void selectSingleFolder() {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            try {
                dialog.Description = "Select Folder";
                dialog.ShowNewFolderButton = false;
                dialog.UseDescriptionForTitle = true;

                DialogResult res = dialog.ShowDialog();
                if (res == DialogResult.OK) {
                    switch (this.getFolderSelectionMode()) {
                        case FolderSelectionMode.SingleFolder:
                            this.txtFolder.Text = dialog.SelectedPath;
                            break;
                        case FolderSelectionMode.MultipleFolders:
                            // concatenate (for now).
                            this.txtFolder.Text = this.txtFolder.Text
                                + (this.txtFolder.Text.Last() == ';' ? "" : ";") + dialog.SelectedPath + ";";
                            break;
                    }
                }
            }
            finally {
                if (dialog != null) {
                    dialog.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<string> selectMultipleFolders() {
            List<string> result = new List<string>();
            frmMultiFolderEditor newform = new frmMultiFolderEditor();
            try {
                newform.StartPosition = FormStartPosition.CenterParent;
                newform.MaximizeBox = false;
                newform.MinimizeBox = false;
                newform.SelectedFolders = this.multifiles;

                DialogResult res = newform.ShowDialog();


                if (res == DialogResult.OK) {
                    result = newform.SelectedFolders;
                }
            }
            catch (Exception ex) {

            }
            finally {
                if (newform != null) {
                    newform.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbbSelectFolder_Click(object sender, EventArgs e) {
            switch (this.getFolderSelectionMode()) {
                case FolderSelectionMode.SingleFolder:
                    this.selectSingleFolder();
                    break;
                case FolderSelectionMode.MultipleFolders:
                    this.multifiles = this.selectMultipleFolders();
                    if (this.multifiles.Count > 0) {
                        this.txtFolder.Text = String.Join(";", this.multifiles);
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<String> getSelectedTextFolders() {
            List<String> result = new List<String>();

            if (this.txtFolder.Text.Length > 0) {
                if (this.getFolderSelectionMode() == FolderSelectionMode.SingleFolder) {
                    result.Add(this.txtFolder.Text);
                }
                else {
                    result = this.txtFolder.Text.Split(';').ToList();

                }
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private void setSingleModeClick() {
            this.txtFolder.ReadOnly = false;
            this.txtFolder.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.txtFolder.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
            this.txtFolder.Text = "";

            this.multifiles = new List<string>();

        }

        /// <summary>
        /// 
        /// </summary>
        private void setMultiFolderModeClick() {
            this.txtFolder.ReadOnly = true;
            this.txtFolder.AutoCompleteMode = AutoCompleteMode.None;
            this.txtFolder.AutoCompleteSource = AutoCompleteSource.None;
            this.txtFolder.Text = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbbSingleFileMode_Click(object sender, EventArgs e) {
            this.setSingleModeClick();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbbMultiFileMode_Click(object sender, EventArgs e) {
            this.setMultiFolderModeClick();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool checkforFolderData() {
            bool ret = false;
            switch (this.getFolderSelectionMode()) {
                case FolderSelectionMode.SingleFolder:
                    ret = this.txtFolder.Text.Length > 0;
                    break;
                case FolderSelectionMode.MultipleFolders:
                    ret = this.multifiles.Count > 0;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasFolderText {
            get {
                return this.checkforFolderData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<String> RecentlyUsedFolders {
            get {
                List<string> list = new List<string>();

                foreach (ToolStripItem r in tbbRecentFolders.DropDownItems) {
                    list.Add(r.Text);
                }

                return list;
            }
            set {
                this.setupMRUItems(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxRecentlyUsedFolders {
            get { return this.maxmruitems; }
            set {
                this.maxmruitems = value;
            }
        }


        /// <summary>
        /// Expose Textbox
        /// </summary>
        public TextBox FolderTextBox {
            get { return this.txtFolder; }
            set { this.txtFolder = value; }
        }



        /// <summary>
        /// 
        /// </summary>
        public String SelectedFolder {
            get {
                string s = null;
                if (this.SelectionMode == FolderSelectionMode.SingleFolder) {
                    s = this.txtFolder.Text;
                }

                addMRUItem(s);

                return s;
            }

            set {
                this.setSingleModeClick();
                this.setFolderSelectionMode(FolderSelectionMode.SingleFolder);
                this.txtFolder.Text = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<String> SelectedFolders {
            get {
                List<String> list = this.getSelectedTextFolders();

                foreach (String s in list) {
                    this.addMRUItem(s);
                }

                return list;
            }

            set {
                if (this.Created) {
                    this.setMultiFolderModeClick();
                    this.setFolderSelectionMode(FolderSelectionMode.MultipleFolders);
                    this.multifiles = value;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public String SelectedFolderRawText {
            get {
                return this.txtFolder.Text;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<String> SelectedMultipleFolders {
            get {
                List<String> list = null;

                if (this.SelectionMode == FolderSelectionMode.MultipleFolders) {
                    list = this.multifiles;
                }

                return list;
            }

            set {
                if (this.SelectionMode == FolderSelectionMode.MultipleFolders) {
                    this.multifiles = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FolderSelectionMode SelectionMode {
            get {
                return getFolderSelectionMode();
            }
            set {
                this.setFolderSelectionMode(value);
            }
        }



    }
}
