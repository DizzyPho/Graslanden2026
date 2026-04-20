using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.DTOs {
    public class MeasurementDTO {
        public int SpeciesId { get; set; }
        public string Coverage { get; set; }
        public MeasurementDTO(int speciesid, string coverage) {
            SpeciesId = speciesid;
            Coverage = coverage;
        }


    }
}
