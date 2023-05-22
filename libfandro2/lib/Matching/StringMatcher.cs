using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libfandro2.lib.Matching {
    public class StringMatcher : Matcher {

        // fileinfo data
        public String CurrentValue { get; set; }

        /// <summary>
        /// user value
        /// </summary>
        public String CompareValue { get; set; }

        public override bool DoMatch() {
            bool b = false;

            switch (this.MatcherAction) {
                case MatcherEnums.MatcherAction.Equals:
                    b = CurrentValue == CompareValue;
                    break;
                case MatcherEnums.MatcherAction.NotEquals:
                    b = CurrentValue != CompareValue;
                    break;
                case MatcherEnums.MatcherAction.Less:
                    // this is irrelevant
                    b = String.Compare(CurrentValue, CompareValue) < 0;
                    break;
                case MatcherEnums.MatcherAction.Greater:
                    // this is irrelevant
                    b = String.Compare(CurrentValue, CompareValue) > 0;
                    break;
            }

            return b;
        }
    }
}
