using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain {
    public class Measurement {
        public Measurement(Species species, string coverage, Plot plot, Dictionary<string, MessageType> errors = null)
        {
            Species = species;
            Coverage = coverage;
            Plot = plot;
            if (errors != null)
                Errors = errors;
            else Errors = new Dictionary<string, MessageType>();
        }

        public Species Species { get; set; }
        public string Coverage { get; set; }
        public Plot Plot { get; set; }
        public Dictionary<string, MessageType> Errors { get; set; }

    }
}
