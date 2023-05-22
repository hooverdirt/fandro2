using System.Drawing;
using System.Windows.Forms;

namespace libfandro2.lib.Controls.Conditions {
    partial class NumericUpDownFile {
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
            btnState = new Button();
            txtValue = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)txtValue).BeginInit();
            SuspendLayout();
            // 
            // btnState
            // 
            btnState.Dock = DockStyle.Right;
            btnState.Location = new Point(286, 0);
            btnState.Name = "btnState";
            btnState.Size = new Size(34, 23);
            btnState.TabIndex = 0;
            btnState.Text = "KB";
            btnState.UseVisualStyleBackColor = true;
            btnState.Click += btnState_Click;
            // 
            // txtValue
            // 
            txtValue.Dock = DockStyle.Fill;
            txtValue.Location = new Point(0, 0);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(286, 23);
            txtValue.TabIndex = 1;
            txtValue.ThousandsSeparator = true;
            txtValue.ValueChanged += txtValue_ValueChanged;
            // 
            // NumericUpDownFile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(txtValue);
            Controls.Add(btnState);
            Name = "NumericUpDownFile";
            Size = new Size(320, 23);
            ((System.ComponentModel.ISupportInitialize)txtValue).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnState;
        private NumericUpDown txtValue;
    }
}
