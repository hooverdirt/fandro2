using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace libfandro2.lib.Finding {
    public class FileIO {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="searchfortext"></param>
        /// <param name="startpos"></param>
        /// <param name="maxchars"></param>
        /// <returns></returns>
        public string GetPreviewLines(string filename, string searchfortext, int startpos, int maxchars) {
            string ret = null;

            maxchars = maxchars == 0 ? 100 : maxchars;

            long top = startpos - maxchars > 0 ? startpos - maxchars : 0;
            long bottom = maxchars + searchfortext.Length + 1;

            StreamReader res = new StreamReader(filename);
            try {
                // correct bottom.
                bottom = bottom < res.BaseStream.Length ? bottom : res.BaseStream.Length;



            }
            catch (Exception ex) {

            }
            finally {
                if (res != null) {
                    res.Dispose();
                }
            }

            return ret;
        }
    }
}
