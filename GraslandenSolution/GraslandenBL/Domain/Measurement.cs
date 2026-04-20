using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain {
    public class Measurement {
        public Measurement(Species species, string coverage, Plot plot)
        {
            Species = species;
            Coverage = coverage;
            Plot = plot;
        }

        public Species Species { get; set; }
        public string Coverage { get; set; }
        public Plot Plot { get; set; }

    }
}
