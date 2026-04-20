using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain {
    public class Species {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Moisture { get; set; }
        public int Ph { get; set; }
        public int Nitrogen { get; set; }
        public int Nectarvalue { get; set; }
        public int Biodiversity { get; set; }
        public Rating Rating { get; set; }

        public Species(int id, string name, int moisture,int ph, int biodiversity, Rating rating) {
            Id = id;
            Name = name;
            Moisture = moisture;
            Ph = ph;
            Biodiversity = biodiversity;
            Rating = rating;
        }

        



    }
}
