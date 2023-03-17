using System.Drawing;
using System.Windows.Forms;

namespace lib.Controls.Conditions {
    partial class SelectableDataRow {
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
            grabHandle = new PictureBox();
            splitContainer1 = new SplitContainer();
            cboFindFields = new ComboBox();
            splitContainer2 = new SplitContainer();
            cboOperators = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)grabHandle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // grabHandle
            // 
            grabHandle.Dock = DockStyle.Left;
            grabHandle.Location = new Point(0, 0);
            grabHandle.Name = "grabHandle";
            grabHandle.Size = new Size(19, 25);
            grabHandle.TabIndex = 0;
            grabHandle.TabStop = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(19, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(cboFindFields);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(520, 25);
            splitContainer1.SplitterDistance = 167;
            splitContainer1.TabIndex = 1;
            // 
            // cboFindFields
            // 
            cboFindFields.Dock = DockStyle.Fill;
            cboFindFields.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFindFields.FormattingEnabled = true;
            cboFindFields.Location = new Point(0, 0);
            cboFindFields.Name = "cboFindFields";
            cboFindFields.Size = new Size(167, 23);
            cboFindFields.TabIndex = 0;
            cboFindFields.SelectedIndexChanged += cboFindFields_SelectedIndexChanged;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(cboOperators);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.BackColor = SystemColors.Control;
            splitContainer2.Size = new Size(349, 25);
            splitContainer2.SplitterDistance = 116;
            splitContainer2.TabIndex = 0;
            // 
            // cboOperators
            // 
            cboOperators.Dock = DockStyle.Top;
            cboOperators.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOperators.FormattingEnabled = true;
            cboOperators.Location = new Point(0, 0);
            cboOperators.Name = "cboOperators";
            cboOperators.Size = new Size(116, 23);
            cboOperators.TabIndex = 0;
            // 
            // SelectableDataRow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Controls.Add(grabHandle);
            Name = "SelectableDataRow";
            Size = new Size(539, 25);
            ((System.ComponentModel.ISupportInitialize)grabHandle).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox grabHandle;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private ComboBox cboFindFields;
        private ComboBox cboOperators;
    }
}
