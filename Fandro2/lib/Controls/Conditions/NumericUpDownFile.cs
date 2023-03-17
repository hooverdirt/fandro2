using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fandro2.lib.Finding;
using lib.Controls.Conditions.Classes;

namespace lib.Controls.Conditions {
    public partial class NumericUpDownFile : UserControl {

        private SizeState sizestate = SizeState.B;

        public NumericUpDownFile() {
            InitializeComponent();
            this.setSizeState(SizeState.B);
            this.txtValue.Minimum = 0;
            this.txtValue.Maximum = Decimal.MaxValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void setSizeState(SizeState value) {
            if (value > SizeState.TB) {
                value = SizeState.B;
            }

            this.sizestate = value;

            this.btnState.Text = Enum.GetName(typeof(SizeState), this.sizestate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtValue_ValueChanged(object sender, EventArgs e) {
            if (ChangeEvent != null) {
                ChangeEvent(this, new NumericUpDownFileValueChangedEventArgs(true, false));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnState_Click(object sender, EventArgs e) {

            this.sizestate = this.sizestate + 1;
            this.setSizeState(this.sizestate);

            if (ChangeEvent != null) {
                ChangeEvent(this, new NumericUpDownFileValueChangedEventArgs(false, true));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SizeState Units {
            get {
                return this.sizestate;
            }

            set {
                this.setSizeState(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Value {
            get {
                return Convert.ToInt32(this.txtValue.Value);
            }

            set {
                this.txtValue.Value = Convert.ToDecimal(value);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<NumericUpDownFileValueChangedEventArgs> ChangeEvent;

        /// <summary>
        /// 
        /// </summary>
        public HorizontalAlignment TextAlign {
            get {
                return this.txtValue.TextAlign;
            }

            set {
                this.txtValue.TextAlign = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean ThousandsSeparator {
            get {
                return this.txtValue.ThousandsSeparator;
            }

            set {
                this.txtValue.ThousandsSeparator = value;
            }
        }
    }
}
