using GraslandenBL.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.DTOs {
    public class MeasurementDTO {
        public int Id { get; set; }
        public Species Species { get; set; }
        public string Coverage { get; set; }
        public MeasurementDTO(int id, Species species, string coverage) {
            Id = id;
            Species = species;
            Coverage = coverage;
        }
    }
}
