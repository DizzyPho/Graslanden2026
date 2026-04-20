using GraslandenBL.Enums;
using GraslandenBL.FactoryResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain {
    public class Measurement {
        private Measurement(Species species, string coverage, Plot plot)
        {
            Species = species;
            Coverage = coverage;
            Plot = plot;
        }



        public Species Species { get; set; }
        public string Coverage { get; set; }
        public Plot Plot { get; set; }

        public static FactoryResult<Measurement> Create(Species species, string coverage, Plot plot)
        {
            List<String> errors = new List<String>();
            if (String.IsNullOrEmpty(coverage)) errors.Add("Bedekkingsklasse mag niet leeg zijn.");
     
            if (errors.Count > 0)
            {
                return new FactoryResult<Measurement>(errors);
            }
            else
            {
                return new FactoryResult<Measurement>(new Measurement(species, coverage, plot));
            }
        }
    }
}
