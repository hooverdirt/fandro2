using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fandro2.lib.Controls.Folders {
    public partial class frmMultiFolderEditor : Form {
        private string startFolder = "";
        public frmMultiFolderEditor() {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            try {

                dialog.SelectedPath = String.IsNullOrEmpty(startFolder) ?
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : this.startFolder;

                DialogResult res = dialog.ShowDialog();
                if (res == DialogResult.OK) {
                    this.lboFolders.Items.Add(dialog.SelectedPath);
                }
            }
            catch (Exception ex) {

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
        public string StartFolder {
            get { return startFolder; }
            set { startFolder = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<String> getSelectedFolders() {
            List<String> selectedFolders = new List<String>();

            foreach (string s in lboFolders.Items) {
                selectedFolders.Add(s);
            }

            return selectedFolders;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void setSelectedFolders(List<String> value) {
            // who cares about DataSource?
            if (value != null) {
                lboFolders.Items.AddRange(value.ToArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFolderSelection_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
                if (this.txtFolderSelection.Text != "") {
                    if (Directory.Exists(this.txtFolderSelection.Text)) {
                        if (this.lboFolders.Items.Contains(this.txtFolderSelection.Text) == false) {
                            lboFolders.Items.Add(this.txtFolderSelection.Text);
                            this.txtFolderSelection.Text = ""; // clear it!
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
        private void brnRemove_Click(object sender, EventArgs e) {
            if (this.lboFolders.SelectedIndex != -1) {
                for (int i = this.lboFolders.SelectedItems.Count - 1; i >= 0; i--) {
                    lboFolders.Items.Remove(this.lboFolders.SelectedItems[i]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e) {
            if (this.lboFolders.SelectedItems.Count == 1 && this.lboFolders.SelectedIndex > 0) {
                string sabove = this.lboFolders.Items[this.lboFolders.SelectedIndex - 1].ToString();
                int posidx = this.lboFolders.SelectedIndex - 1;
                int selectedidx = this.lboFolders.SelectedIndex;

                string current = this.lboFolders.Items[this.lboFolders.SelectedIndex].ToString();

                this.lboFolders.Items.RemoveAt(posidx);
                this.lboFolders.Items.Insert(posidx, current);
                this.lboFolders.Items.RemoveAt(posidx + 1);
                this.lboFolders.Items.Insert(selectedidx, sabove);
                this.lboFolders.SelectedIndex = posidx;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e) {
            if (this.lboFolders.SelectedItems.Count == 1 && this.lboFolders.SelectedIndex < this.lboFolders.Items.Count - 1) {
                int posidx = this.lboFolders.SelectedIndex + 1;
                int selectedidx = this.lboFolders.SelectedIndex;

                string sdown = this.lboFolders.Items[posidx].ToString();
                string current = this.lboFolders.Items[selectedidx].ToString();

                this.lboFolders.Items.RemoveAt(posidx);
                this.lboFolders.Items.Insert(posidx, current);
                this.lboFolders.Items.RemoveAt(posidx - 1);
                this.lboFolders.Items.Insert(selectedidx, sdown);
                // set selection index..
                this.lboFolders.SelectedIndex = posidx;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMultiFolderEditor_Load(object sender, EventArgs e) {
            this.ActiveControl = this.lboFolders;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<String> SelectedFolders {
            get {
                return this.getSelectedFolders();
            }
            set {
                this.setSelectedFolders(value);
            }
        }
    }
}
