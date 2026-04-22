using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain {
    public class Measurement {
        public Measurement(Species species, string coverage, Plot plot, List<string> errors)
        {
            Species = species;
            Coverage = coverage;
            Plot = plot;
            Errors = errors;
        }

        public Species Species { get; set; }
        public string Coverage { get; set; }
        public Plot Plot { get; set; }
        public List<string> Errors { get; set; }

    }
}
