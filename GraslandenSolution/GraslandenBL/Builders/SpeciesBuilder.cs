using GraslandenBL.Domain;
using GraslandenBL.Enums;
using GraslandenBL.Results;

namespace GraslandenBL.Builders
{
    public class SpeciesBuilder
    {
        private Species _species;
        private List<string> _errors;

        public SpeciesBuilder(string name)
        {
            _errors = new List<string>();
            _species = new Species();
            if (string.IsNullOrEmpty(name)) _errors.Add($"Naam mag niet leeg zijn.");
            else _species.Name = name;
        }

        public SpeciesBuilder AddMoisture(string moistureString)
        {
            int? moisture = null;

            try
            {
                if (!string.IsNullOrEmpty(moistureString)) _species.Moisture = int.Parse(moistureString);
                if (moisture < 0) _errors.Add($"{_species.Name} | Foute waarde voor vochtigheid '{moisture}'. Moet een positief getal zijn.");
            }
            catch
            {
                _errors.Add($"{_species.Name} | Vochtgehalte: '{moistureString}' moet een getal zijn.");
            }
            return this;
        }

        public SpeciesBuilder AddPh(string phString)
        {
            int? ph = null;

            try
            {
                if (!string.IsNullOrEmpty(phString)) _species.Ph = int.Parse(phString);
                if (ph < 0) _errors.Add($"{_species.Name} | Foute waarde voor zuurtegraad '{ph}'. Moet een positief getal zijn.");
            }
            catch
            {
                _errors.Add($"{_species.Name} | Zuurtegraad: '{phString}' moet een getal zijn.");
            }
            return this;
        }

        public SpeciesBuilder AddNitrogen(string nitrogenString)
        {
            int? nitrogen = null;

            try
            {
                if (!string.IsNullOrEmpty(nitrogenString)) _species.Nitrogen = int.Parse(nitrogenString);
                if (nitrogen < 0) _errors.Add($"{_species.Name} | Foute stikstofwaarde '{nitrogen}'. Moet een positief getal zijn.");
            }
            catch
            {
                _errors.Add($"{_species.Name} | Stikstofgehalte: '{nitrogenString}' moet een getal zijn.");
            }
            return this;
        }

        public SpeciesBuilder AddNectarValue(string nectarValueString)
        {
            int? nectarValue = null;

            try
            {
                if (!string.IsNullOrEmpty(nectarValueString)) _species.Nectarvalue = int.Parse(nectarValueString);
                if (nectarValue < 0) _errors.Add($"{_species.Name} | Foute nectarwaarde '{nectarValue}'. Moet een positief getal zijn.");
            }
            catch
            {
                _errors.Add($"{_species.Name} | Nectarwaarde: '{nectarValueString}' moet een getal zijn.");
            }
            return this;
        }

        public SpeciesBuilder AddBiodiversity(string biodiversityString)
        {
            int? biodiversity = null;

            try
            {
                if (!string.IsNullOrEmpty(biodiversityString)) _species.Biodiversity = int.Parse(biodiversityString);
                if (biodiversity < 0) _errors.Add($"{_species.Name} | Foute biodiversiteitswaarde '{biodiversity}'. Moet een positief getal zijn.");
            }
            catch
            {
                _errors.Add($"{_species.Name} | Biodiversiteit: '{biodiversityString}' moet een getal zijn.");
            }
            return this;
        }

        public SpeciesBuilder AddRating(string ratingString)
        {
            Rating? rating = null;
            try
            {
                _species.Rating = ratingString.Trim() switch
                {
                    "+++" => Rating.Sleutel,
                    "++" => Rating.Begeleidend,
                    "+" => Rating.Algemeen,
                    "0" => Rating.Ruderaal,
                    "-" => Rating.Invasief
                };
            }
            catch
            {
                _errors.Add($"{_species.Name} | Rating: '{ratingString}' is ongeldig.");
            }
            return this;
        }

        public SpeciesResult Build()
        {
            if (!string.IsNullOrEmpty(_species.Name)) return new SpeciesResult(_species, _errors);
            else return new SpeciesResult(null, _errors);
        }
    }
}
