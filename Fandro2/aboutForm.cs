using Fandro2.lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fandro2 {
    public partial class aboutForm : Form {
        public aboutForm() {
            InitializeComponent();
        }

        private void aboutForm_Load(object sender, EventArgs e) {
            // get the file version
            this.label3.Text = String.Format("Version: {0}", Helpers.GetFileVersionInfo().FileVersion);
            this.label2.Text = String.Format("Copyrights © 2005-{0} Arthur Hoogervorst for PPF", DateTime.Now.Year);
        }
    }
}
