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
using libfandro2.lib.Finding;
using libfandro2.lib.Controls.Conditions.Classes;

namespace libfandro2.lib.Controls.Conditions {
    public partial class SelectableDataRow : UserControl {
        private ControlGrid ownerGrid = null;
        private Control valueControl = null;

        /// <summary>
        /// 
        /// </summary>
        public SelectableDataRow() {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownergrid"></param>
        public SelectableDataRow(ControlGrid ownergrid) : this() {
            this.ownerGrid = ownergrid;
            this.Parent = ownergrid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownergrid"></param>
        /// <param name="properties"></param>
        /// <param name="operators"></param>
        public SelectableDataRow(ControlGrid ownergrid, List<FilePropertyItem> properties, List<OperatorItem> operators)
            : this(ownergrid) {
            this.FilePropertyItems = properties;
            this.OperatorItems = operators;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<FilePropertyItem> FilePropertyItems {
            get {
                return this.cboFindFields.Items.Cast<FilePropertyItem>().ToList();
            }

            set {
                this.cboFindFields.DataSource = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<OperatorItem> OperatorItems {
            get {
                return this.cboOperators.Items.Cast<OperatorItem>().ToList();
            }

            set {
                this.cboOperators.DataSource = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private TableLayoutPanelCellPosition getGridLocation() {
            return this.ownerGrid.GetCellPosition(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboFindFields_SelectedIndexChanged(object sender, EventArgs e) {

            if (this.cboFindFields.SelectedItem != null) {

                if (this.splitContainer2.Panel2.HasChildren) {
                    foreach (Control t in this.splitContainer2.Panel2.Controls) {
                        t.Dispose();
                    }
                }
                FilePropertyItem selitem = (FilePropertyItem)this.cboFindFields.SelectedItem;


                if (selitem != null) {
                    if (selitem.ValueType == typeof(DateTime)) {
                        this.valueControl = new DateTimePicker();

                    }
                    else if (selitem.ValueType == typeof(Int32)) {
                        this.valueControl = new NumericUpDownFile();
                    }
                    else {
                        this.valueControl = new TextBox();
                    }

                    this.valueControl.Parent = this.splitContainer2.Panel2;
                    this.valueControl.Dock = DockStyle.Top;
                    this.valueControl.Visible = true;

                }


            }
        }


        /// <summary>
        /// 
        /// </summary>
        public ControlGrid OwnerGrid {
            get {
                return this.ownerGrid;
            }

            set {
                this.ownerGrid = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private object getValue() {
            object val = null;
            if (this.valueControl != null) {
                if (this.valueControl is DateTimePicker) {
                    val = (this.valueControl as DateTimePicker).Value.Date;
                }

                if (this.valueControl is NumericUpDownFile) {
                    val = (this.valueControl as NumericUpDownFile).Value;
                }

                if (this.valueControl is TextBox) {
                    val = (this.valueControl as TextBox).Text;
                }
            }

            return val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        private void setValue(object t) {
            if (this.valueControl != null) {
                // a bit dangerous!
                if (this.valueControl is DateTimePicker && t is DateTime) {
                    (this.valueControl as DateTimePicker).Value = (DateTime)t;
                }

                if (this.valueControl is NumericUpDownFile && t is int) {
                    (this.valueControl as NumericUpDownFile).Value = (int)t;
                }

                if (this.valueControl is TextBox && t is string) {
                    (this.valueControl as TextBox).Text = (String)t;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public FilePropertyItem SelectedFilePropertyItem {
            get {
                return this.cboFindFields.SelectedItem as FilePropertyItem;
            }
            set {
                this.cboFindFields.SelectedItem = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SelectedFilePropertyItemIndex {
            get {
                return this.cboFindFields.SelectedIndex;
            }

            set {
                this.cboFindFields.SelectedIndex = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public OperatorItem SelectedOperatorItem {
            get {
                return this.cboOperators.SelectedItem as OperatorItem;
            }

            set {
                this.cboOperators.SelectedItem = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SelectedOperatorItemIndex {
            get {
                return this.cboOperators.SelectedIndex;
            }

            set {
                this.cboOperators.SelectedIndex = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Control SelectedValueControl {
            get {
                return this.valueControl;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasUnits {
            get {
                return this.valueControl is NumericUpDownFile ? true : false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SizeState Units {
            get {
                SizeState state = SizeState.B;

                // if valueControl is null - it would never have units...
                if (this.HasUnits) {
                    state = (this.valueControl as NumericUpDownFile).Units;
                }

                return state;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value {
            get { return this.getValue(); }
            set { this.setValue(value); }
        }
    }
}
