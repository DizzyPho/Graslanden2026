using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain {
    public class Measurement {
        public Species Species { get; set; }
        public string Coverage { get; set; }
        public Plot Plot { get; set; }
    }
}
