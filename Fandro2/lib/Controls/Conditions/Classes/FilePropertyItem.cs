using Fandro2.lib.Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.Controls.Conditions.Classes {
    /// <summary>
    ///
    /// </summary>
    public class FilePropertyItem {
        public string Name { get; set; }
        public Type ValueType { get; set; }

        public MatcherEnums.MatcherType MatcherType 
            {get; set;}

        public override string ToString() {
            return this.Name;
        }
    }
}
