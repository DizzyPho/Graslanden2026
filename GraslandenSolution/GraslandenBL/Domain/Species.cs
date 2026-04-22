using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain {
    public class Species {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Moisture { get; set; }
        public int? Ph { get; set; }
        public int? Nitrogen { get; set; }
        public int? Nectarvalue { get; set; }
        public int? Biodiversity { get; set; }
        public Rating? Rating { get; set; }
        public Dictionary<string, MessageType> Errors { get; set; }
        public Species(int? id, string name, int? moisture, int? ph, int? nitrogen, int? nectarvalue, int? biodiversity, Rating? rating, Dictionary<string, MessageType> errors = null) {
            Id = id;
            Name = name;
            Moisture = moisture;
            Ph = ph;
            Nitrogen = nitrogen;
            Nectarvalue = nectarvalue;
            Biodiversity = biodiversity;
            Rating = rating;
            if (errors != null)
                Errors = errors;
            else Errors = new Dictionary<string, MessageType>();
        }

        public Species() { }

        public static Rating ParseRating(string ratingString)
        {
            Rating rating = ratingString switch
            {
                "+++" =>  Enums.Rating.Begeleidend,
                "++" => Enums.Rating.Begeleidend,
                "+" => Enums.Rating.Algemeen,
                "0" => Enums.Rating.Ruderaal,
                "-" => Enums.Rating.Invasief,
                _ => throw new ArgumentException()
            };
            return rating;
        }
    }
}
