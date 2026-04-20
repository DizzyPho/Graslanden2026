using GraslandenBL.Enums;
using GraslandenBL.FactoryResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain {
    public class Species {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Moisture { get; set; }
        public int Ph { get; set; }
        public int Nitrogen { get; set; }
        public int? Nectarvalue { get; set; }
        public int? Biodiversity { get; set; }
        public Rating? Rating { get; set; }

        private Species(int? id, string name, int moisture, int ph, int nitrogen, int? nectarvalue, int? biodiversity, Rating? rating) {
            Id = id;
            Name = name;
            Moisture = moisture;
            Ph = ph;
            Nitrogen = nitrogen;
            Nectarvalue = nectarvalue;
            Biodiversity = biodiversity;
            Rating = rating;
        }

        public static FactoryResult<Species> Create(int? id, string name, int moisture, int ph, int nitrogen, int? nectarvalue, int? biodiversity, Rating? rating)
        {
            List<String> errors = new List<String>();
            if (String.IsNullOrEmpty(name)) errors.Add("Naam mag niet leeg zijn.");
            if (moisture < 0) errors.Add($"Foute waarde voor vochtigheid '{moisture}'. Moet een positief getal zijn.");
            if (ph < 0) errors.Add($"Foute waarde voor zuurtegraad '{ph}'. Moet een positief getal zijn.");
            if (nitrogen < 0) errors.Add($"Foute stikstofwaarde '{nitrogen}'. Moet een positief getal zijn.");
            if (nectarvalue < 0) errors.Add($"Foute nectarwaarde '{nectarvalue}'. Moet een positief getal zijn.");
            if (biodiversity < 0) errors.Add($"Foute biodiversiteitswaarde '{moisture}'. Moet een positief getal zijn.");


            if (errors.Count > 0)
            {
                return new FactoryResult<Species>(errors);
            }
            else
            {
                return new FactoryResult<Species>(new Species(id, name, moisture, ph, nitrogen, nectarvalue, biodiversity, rating));
            }
        }



    }
}
