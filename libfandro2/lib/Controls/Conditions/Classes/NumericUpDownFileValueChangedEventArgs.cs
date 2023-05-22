using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libfandro2.lib.Controls.Conditions.Classes {
    public class NumericUpDownFileValueChangedEventArgs {
        private bool numberChanged = false;
        private bool unitsChanged = false;

        /// <summary>
        /// 
        /// </summary>
        public NumericUpDownFileValueChangedEventArgs() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numbervaluechanged"></param>
        /// <param name="sizestatechanged"></param>
        public NumericUpDownFileValueChangedEventArgs(bool numbervaluechanged, bool sizestatechanged) {
            this.numberChanged = numbervaluechanged;
            this.unitsChanged = sizestatechanged;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NumberValueChanged { 
            get { return this.numberChanged; } 
            set {  this.numberChanged = value; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UnitsValueChanged {
            get { return this.unitsChanged; }
            set { this.unitsChanged = value; }
        }
    }
}
