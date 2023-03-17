using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fandro2.lib.Finding {


    public enum SizeState {
        B = 0,
        KB = 1,
        MB = 2,
        GB = 3,
        TB = 4
    }

    public enum WordFind {
        Any,
        All,
        Exact
    }

    [Flags]
    public enum FileSearchOptions {
        None = 0,
        Recursive,
        CaseSensitive
    }


}
