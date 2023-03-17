using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fandro2.lib.Controls.Folders {
    public partial class ToolBarRadioButton : ToolStripButton {
        private int radiogroupid = 0;
        private bool updategroup = true;
        private Color backgroundcolour = SystemColors.ButtonShadow;


        public ToolBarRadioButton() {
            InitializeComponent();
            this.CheckOnClick = true;
            this.updategroup = true;
        }

        [Category("Grouping")]
        public int RadioGroupId {
            get { return this.radiogroupid; }
            set {
                this.radiogroupid = value;
                this.updateGroupButtons();
            }
        }

        [Category("BackgroundColor")]
        public Color PressedBackgroundColor {
            get { return this.backgroundcolour; }
            set { 
                this.backgroundcolour= value;
                this.Invalidate();
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            this.Checked= true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCheckedChanged(EventArgs e) {
            base.OnCheckedChanged(e);
            if (this.Parent != null && this.updategroup) {
                foreach (ToolBarRadioButton radioButton in this.Parent.Items.OfType<ToolBarRadioButton>()) {
                    // Disable all other radio buttons with same group id
                    if (radioButton != this && radioButton.RadioGroupId == this.RadioGroupId) {
                        radioButton.setCheckValue(false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void setCheckValue(bool value) {
            this.updategroup = false;
            try {
                this.Checked = value;

            }
            finally { 
                this.updategroup=true;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void updateGroupButtons() {
            if (this.Parent != null) {
                // Anything checked?
                int checkedCount = this.Parent.Items.OfType<ToolBarRadioButton>().Count(x => x.RadioGroupId == RadioGroupId && x.Checked);

                if (checkedCount > 1) {
                    this.Checked = false;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe) {
            if (this.Checked) {
                pe.Graphics.FillRectangle(new SolidBrush(this.backgroundcolour), new Rectangle(new Point(0,0), this.Size));
            }
            base.OnPaint(pe);
        }
    }
}
