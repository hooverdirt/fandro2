using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Fandro2.lib.Threading {
    public class FileSetWordFinder {
        List<string> fileset = new List<string>();
        private ManualResetEvent stopThread;
        private ManualResetEvent threadHasStopped;
        private string pattern = null;
        private bool casesensitive = false;

        public FileSetWordFinder() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileset"></param>
        /// <param name="stopThread"></param>
        /// <param name="threadHasStopped"></param>
        public FileSetWordFinder(List<string> fileset, ManualResetEvent stopThread, ManualResetEvent threadHasStopped) { 
            this.fileset = fileset;
            this.stopThread = stopThread;
            this.threadHasStopped = threadHasStopped;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Pattern { 
            get { return pattern; } 
            set {  pattern = value; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CaseSensitive {
            get { return casesensitive; }
            set { casesensitive = value; }
        }
    }
}
