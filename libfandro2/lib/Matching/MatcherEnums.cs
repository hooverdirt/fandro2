﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libfandro2.lib.Matching {
    public class MatcherEnums {
        public enum MatcherType {
            None,
            FileSize,
            FileModTime,
            FileCreateTime,
            FileAccessTime
        }

        public enum MatcherAction {
            Equals,
            NotEquals,
            Greater,
            Less,
            DoesContain,
            DoesNotContain
        }

        public enum MatcherOperator {
            Or,
            And
        }
    }
}
