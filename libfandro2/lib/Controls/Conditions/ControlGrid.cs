using libfandro2.lib.Matching;
using libfandro2.lib.Controls.Conditions.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace libfandro2.lib.Controls.Conditions {
    public class ControlGrid : TableLayoutPanel {
        bool resizable = false;
        bool resizing = false;
        TableLayoutRowStyleCollection rowStyles;
        TableLayoutColumnStyleCollection columnStyles;
        int colindex = -1;
        int rowindex = -1;



        public ControlGrid() : base() {
            this.rowStyles = this.RowStyles;
            this.columnStyles = this.ColumnStyles;
            this.AutoSize = false;
        }




        protected override void OnMouseMove(MouseEventArgs e) {
            if (!resizing && this.resizable) {
                float width = 0;
                float height = 0;
                //for rows
                for (int i = 0; i < rowStyles.Count; i++) {
                    height += rowStyles[i].Height;
                    if (e.Y > height - 3 && e.Y < height + 3) {
                        rowindex = i;
                        this.Cursor = Cursors.HSplit;
                        break;
                    }
                    else {
                        rowindex = -1;
                        this.Cursor = Cursors.Default;
                    }
                }
                //for columns
                for (int i = 0; i < columnStyles.Count; i++) {
                    width += columnStyles[i].Width;
                    if (e.X > width - 3 && e.X < width + 3) {
                        colindex = i;
                        if (rowindex > -1)
                            this.Cursor = Cursors.Cross;
                        else
                            this.Cursor = Cursors.VSplit;
                        break;
                    }
                    else {
                        colindex = -1;
                        if (rowindex == -1) {
                            this.Cursor = Cursors.Default;
                        }
                    }
                }
            }
            if (resizing && (colindex > -1 || rowindex > -1)) {
                float width = e.X;
                float height = e.Y;
                if (colindex > -1) {
                    for (int i = 0; i < colindex; i++) {
                        width -= columnStyles[i].Width;
                    }
                    columnStyles[colindex].SizeType = SizeType.Absolute;
                    columnStyles[colindex].Width = width;
                }
                if (rowindex > -1) {
                    for (int i = 0; i < rowindex; i++) {
                        height -= rowStyles[i].Height;
                    }

                    rowStyles[rowindex].SizeType = SizeType.Absolute;
                    rowStyles[rowindex].Height = height;
                }
            }
            else {

                int[] colwidths = this.GetColumnWidths();
                int[] rowheights = this.GetRowHeights();

                int top = this.Parent.PointToScreen(this.Location).Y;
                for (int y = 0; y < rowheights.Length; ++y) {
                    int left = this.Parent.PointToScreen(this.Location).X;
                    for (int x = 0; x < colwidths.Length; ++x) {
                        Rectangle t = new Rectangle(left, top, colwidths[x], rowheights[y]);
                        if (t.Contains(MousePosition)) {
                            Control c = this.GetControlFromPosition(x, y);
                            if (c != null && c is SelectableDataRow) {
                               (c as SelectableDataRow).BorderStyle = BorderStyle.None;                                
                            }
                        }
                        left += colwidths[x];
                    }
                    top += rowheights[y];
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);

            int[] colwidths = this.GetColumnWidths();
            int[] rowheights = this.GetRowHeights();

            int top = this.Parent.PointToScreen(this.Location).Y;
            for (int y = 0; y < rowheights.Length; ++y) {
                int left = this.Parent.PointToScreen(this.Location).X;
                for (int x = 0; x < colwidths.Length; ++x) {
                    Rectangle t = new Rectangle(left, top, colwidths[x], rowheights[y]);
                    if (t.Contains(MousePosition)) {
                        Control c = this.GetControlFromPosition(x, y);
                        if (c != null && c is SelectableDataRow) {
                            (c as SelectableDataRow).BorderStyle = BorderStyle.FixedSingle;
                            this.UnFocusOthers(y);
                        }
                    }
                    left += colwidths[x];
                }
                top += rowheights[y];
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ypos"></param>
        public virtual void UnFocusOthers(int ypos) {
            for (int i = 0; i < this.RowCount; i++) {
                Control d = this.GetControlFromPosition(0, i);
                if (d != null && d.HasChildren == true && i != ypos) {
                    (d as SelectableDataRow).BorderStyle = BorderStyle.None;                    
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual void UnfocusAllRows() {
            for (int i = 0; i < this.RowCount; i++) {
                Control d = this.GetControlFromPosition(0, i);
                if (d != null && d.HasChildren == true) {
                    (d as SelectableDataRow).BorderStyle = BorderStyle.None;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e) {
            if (this.resizable) {
                if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                    rowStyles = this.RowStyles;
                    columnStyles = this.ColumnStyles;
                    resizing = true;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                resizing = false;
                this.Cursor = Cursors.Default;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual int findFirstEmptyRow() {
            int r = -1;
            for (int i = 0; i < this.RowCount; i++) {

                if (this.GetControlFromPosition(0, i) == null) {
                    r = i;
                    break;
                }
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public SelectableDataRow this[int i]  {
            get {
                return this.GetControlFromPosition(0, i) as SelectableDataRow;
            }

            set {
                SelectableDataRow d = (this.GetControlFromPosition(0, i) as SelectableDataRow);
                d = value;                                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<SelectableDataRow> getDataRows() {
            List<SelectableDataRow> datarows = new List<SelectableDataRow>();

            for(int i = 0; i < this.RowCount + 1; i++) {
                Control t = this.GetControlFromPosition(0, i);
                if (t != null && t is SelectableDataRow) {
                    datarows.Add(t as SelectableDataRow);
                }
            }

            return datarows;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<SelectableDataRow> Items {
            get {
                return this.getDataRows();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public virtual void AddDataRow(SelectableDataRow row) {
            int i = findFirstEmptyRow();

            if (i == -1) {
                this.RowCount++;
                i = this.RowCount -1;
            }

            row.Parent = this;
            row.OwnerGrid = this;
            this.SetCellPosition(row, new TableLayoutPanelCellPosition { Column = 0, Row = i });
            this.SetColumnSpan(row, this.ColumnCount);
        }        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public virtual void RemoveDataRow(int row, bool shift = true) {
            if (row >= 0 && row < this.RowCount) {
                Control rowcontrol = this.GetControlFromPosition(0, row);
                if (rowcontrol != null) {
                    rowcontrol.Dispose();
                }

                // shift everything up
                try {
                    for(int i = row + 1; i < this.RowCount; i++ ) {
                        Control rowControl = this.GetControlFromPosition(0, i);
                        this.SetRow(rowControl, i - 1);
                    }                    
                }
                finally {
                    this.RowCount--;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ClearDataRows() {
            if (this.RowCount > 0) {
                for(int i= this.RowCount -1; i >= 0; i-- ) {
                    this.RemoveDataRow(i);
                }
            }
        }

        /// <summary>
        /// Determines if the gridcolumns and rows are resizable
        /// </summary>
        public bool Resizable {
            get {
                return this.resizable;
            }

            set { 
                this.resizable = value;
            }
        }
    }
}
