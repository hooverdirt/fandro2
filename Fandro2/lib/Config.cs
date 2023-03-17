using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fandro2.Properties;
using System.IO;

namespace Fandro2.lib {
    public static class Config {
        /// <summary>
        /// 
        /// </summary>
        
        public static Settings Settings {
            get { return Fandro2.Properties.Settings.Default; }
        }

        /// <summary>
        /// 
        /// </summary>
        private static string generateLocalAppFolder() {
            DirectoryInfo directory = Directory.CreateDirectory(Helpers.GetCurrentLocalAppPath());
            return directory.FullName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        public static void LoadAutoCompleteSettings(mainForm form) {
            
        }

    }
}
