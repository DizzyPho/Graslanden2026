using GraslandenBL.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.DTOs {
    public class MeasurementDTO {
        public Species Species { get; set; }
        public string Coverage { get; set; }
        public MeasurementDTO(Species species, string coverage) {
            Species = species;
            Coverage = coverage;
        }


    }
}
