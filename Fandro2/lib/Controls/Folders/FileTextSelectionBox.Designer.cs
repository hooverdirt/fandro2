using System.Drawing;
using System.Windows.Forms;

namespace Fandro2.lib.Controls.Folders {
    partial class FileTextSelectionBox {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileTextSelectionBox));
            toolStripFileModes = new ToolStrip();
            tbbSingleFileMode = new ToolBarRadioButton();
            tbbMultiFileMode = new ToolBarRadioButton();
            toolStripSeparator1 = new ToolStripSeparator();
            tbbSelectFolder = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            tbbRecentFolders = new ToolStripDropDownButton();
            txtFolder = new TextBox();
            toolStripFileModes.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripFileModes
            // 
            toolStripFileModes.Dock = DockStyle.None;
            toolStripFileModes.GripStyle = ToolStripGripStyle.Hidden;
            toolStripFileModes.Items.AddRange(new ToolStripItem[] { tbbSingleFileMode, tbbMultiFileMode, toolStripSeparator1, tbbSelectFolder, toolStripSeparator2, tbbRecentFolders });
            toolStripFileModes.Location = new Point(314, 0);
            toolStripFileModes.Name = "toolStripFileModes";
            toolStripFileModes.Size = new Size(144, 25);
            toolStripFileModes.TabIndex = 1;
            toolStripFileModes.TabStop = true;
            toolStripFileModes.Text = "toolStrip1";
            // 
            // tbbSingleFileMode
            // 
            tbbSingleFileMode.Checked = true;
            tbbSingleFileMode.CheckOnClick = true;
            tbbSingleFileMode.CheckState = CheckState.Checked;
            tbbSingleFileMode.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tbbSingleFileMode.Image = (Image)resources.GetObject("tbbSingleFileMode.Image");
            tbbSingleFileMode.ImageTransparentColor = Color.Magenta;
            tbbSingleFileMode.Name = "tbbSingleFileMode";
            tbbSingleFileMode.PressedBackgroundColor = SystemColors.ButtonShadow;
            tbbSingleFileMode.RadioGroupId = 1;
            tbbSingleFileMode.Size = new Size(23, 22);
            tbbSingleFileMode.Text = "Single Folder Mode";
            tbbSingleFileMode.Click += tbbSingleFileMode_Click;
            // 
            // tbbMultiFileMode
            // 
            tbbMultiFileMode.CheckOnClick = true;
            tbbMultiFileMode.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tbbMultiFileMode.Image = (Image)resources.GetObject("tbbMultiFileMode.Image");
            tbbMultiFileMode.ImageTransparentColor = Color.Magenta;
            tbbMultiFileMode.Name = "tbbMultiFileMode";
            tbbMultiFileMode.PressedBackgroundColor = SystemColors.ButtonShadow;
            tbbMultiFileMode.RadioGroupId = 1;
            tbbMultiFileMode.Size = new Size(23, 22);
            tbbMultiFileMode.Text = "Multi Folder Mode";
            tbbMultiFileMode.Click += tbbMultiFileMode_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // tbbSelectFolder
            // 
            tbbSelectFolder.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tbbSelectFolder.Image = (Image)resources.GetObject("tbbSelectFolder.Image");
            tbbSelectFolder.ImageTransparentColor = Color.Magenta;
            tbbSelectFolder.Name = "tbbSelectFolder";
            tbbSelectFolder.Size = new Size(23, 22);
            tbbSelectFolder.Text = "Select Folder";
            tbbSelectFolder.Click += tbbSelectFolder_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // tbbRecentFolders
            // 
            tbbRecentFolders.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tbbRecentFolders.Image = (Image)resources.GetObject("tbbRecentFolders.Image");
            tbbRecentFolders.ImageTransparentColor = Color.Magenta;
            tbbRecentFolders.Name = "tbbRecentFolders";
            tbbRecentFolders.Size = new Size(29, 22);
            tbbRecentFolders.Text = "Recently Used";
            // 
            // txtFolder
            // 
            txtFolder.Location = new Point(0, 0);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(311, 23);
            txtFolder.TabIndex = 0;
            // 
            // FileTextSelectionBox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(txtFolder);
            Controls.Add(toolStripFileModes);
            Name = "FileTextSelectionBox";
            Size = new Size(458, 26);
            toolStripFileModes.ResumeLayout(false);
            toolStripFileModes.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolStrip toolStripFileModes;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripDropDownButton tbbRecentFolders;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton tbbSelectFolder;
        private ToolBarRadioButton tbbSingleFileMode;
        private ToolBarRadioButton tbbMultiFileMode;
        private TextBox txtFolder;
    }
}
