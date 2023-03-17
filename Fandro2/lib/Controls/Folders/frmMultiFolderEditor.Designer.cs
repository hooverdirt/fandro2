using System.Drawing;
using System.Windows.Forms;

namespace Fandro2.lib.Controls.Folders {
    partial class frmMultiFolderEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMultiFolderEditor));
            txtFolderSelection = new TextBox();
            btnDown = new Button();
            btnUp = new Button();
            brnRemove = new Button();
            btnAdd = new Button();
            lboFolders = new ListBox();
            btnCancel = new Button();
            btnOK = new Button();
            SuspendLayout();
            // 
            // txtFolderSelection
            // 
            txtFolderSelection.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtFolderSelection.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtFolderSelection.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
            txtFolderSelection.Location = new Point(5, 14);
            txtFolderSelection.Name = "txtFolderSelection";
            txtFolderSelection.PlaceholderText = "Enter foldername and press RETURN";
            txtFolderSelection.Size = new Size(352, 23);
            txtFolderSelection.TabIndex = 1;
            txtFolderSelection.KeyUp += txtFolderSelection_KeyUp;
            // 
            // btnDown
            // 
            btnDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDown.Image = (Image)resources.GetObject("btnDown.Image");
            btnDown.Location = new Point(363, 150);
            btnDown.Name = "btnDown";
            btnDown.Size = new Size(26, 23);
            btnDown.TabIndex = 6;
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += btnDown_Click;
            // 
            // btnUp
            // 
            btnUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUp.Image = (Image)resources.GetObject("btnUp.Image");
            btnUp.Location = new Point(363, 121);
            btnUp.Name = "btnUp";
            btnUp.Size = new Size(26, 23);
            btnUp.TabIndex = 5;
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += btnUp_Click;
            // 
            // brnRemove
            // 
            brnRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            brnRemove.Image = (Image)resources.GetObject("brnRemove.Image");
            brnRemove.Location = new Point(363, 92);
            brnRemove.Name = "brnRemove";
            brnRemove.Size = new Size(26, 23);
            brnRemove.TabIndex = 4;
            brnRemove.UseVisualStyleBackColor = true;
            brnRemove.Click += brnRemove_Click;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Image = (Image)resources.GetObject("btnAdd.Image");
            btnAdd.Location = new Point(363, 63);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(26, 23);
            btnAdd.TabIndex = 3;
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // lboFolders
            // 
            lboFolders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lboFolders.FormattingEnabled = true;
            lboFolders.HorizontalScrollbar = true;
            lboFolders.ItemHeight = 15;
            lboFolders.Location = new Point(5, 43);
            lboFolders.Name = "lboFolders";
            lboFolders.SelectionMode = SelectionMode.MultiExtended;
            lboFolders.Size = new Size(352, 139);
            lboFolders.TabIndex = 2;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(200, 224);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(119, 224);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 7;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // frmMultiFolderEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(403, 263);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(txtFolderSelection);
            Controls.Add(btnDown);
            Controls.Add(btnUp);
            Controls.Add(brnRemove);
            Controls.Add(btnAdd);
            Controls.Add(lboFolders);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "frmMultiFolderEditor";
            Text = "Multi Folder Editor Box";
            Load += frmMultiFolderEditor_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnDown;
        private Button btnUp;
        private Button brnRemove;
        private Button btnAdd;
        private ListBox lboFolders;
        private Button btnCancel;
        private Button btnOK;
        private TextBox txtFolderSelection;
    }
}