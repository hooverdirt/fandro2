using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fandro2.lib.Interfaces {
    public interface IFandroFindForm {
        public StatusStrip StatusStrip { get; set; }
        public ListView FilesListView { get; set; }  
        public void SetControlsWhileThreading(bool stopping);
        IList<ListViewItem> FoundFileItems { get; }
        IList<ListViewItem> SelectedFileItems { get; }
    }
}
