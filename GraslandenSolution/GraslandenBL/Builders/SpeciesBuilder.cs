using GraslandenBL.Domain;
using GraslandenBL.Enums;
using GraslandenBL.Results;

namespace GraslandenBL.Builders
{
    public class SpeciesBuilder
    {
        private Species _species;
        private Dictionary<string, MessageType> _errors;

        public SpeciesBuilder(string name)
        {
            _errors = new Dictionary<string, MessageType>();
            _species = new Species();
            if (string.IsNullOrWhiteSpace(name)) _errors.Add($"Naam mag niet leeg zijn.", MessageType.Error);
            else _species.Name = name;
        }

        public SpeciesBuilder AddMoisture(string moistureString)
        {
            int? moisture = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(moistureString)) _species.Moisture = int.Parse(moistureString);
                if (moisture < 0) _errors.Add($"{_species.Name} | Foute waarde voor vochtigheid '{moisture}'. Moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Vochtgehalte: '{moistureString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddPh(string phString)
        {
            int? ph = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(phString)) _species.Ph = int.Parse(phString);
                if (ph < 0) _errors.Add($"{_species.Name} | Foute waarde voor zuurtegraad '{ph}'. Moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Zuurtegraad: '{phString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddNitrogen(string nitrogenString)
        {
            int? nitrogen = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(nitrogenString)) _species.Nitrogen = int.Parse(nitrogenString);
                if (nitrogen < 0) _errors.Add($"{_species.Name} | Foute stikstofwaarde '{nitrogen}'. Moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Stikstofgehalte: '{nitrogenString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddNectarValue(string nectarValueString)
        {
            int? nectarValue = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(nectarValueString)) _species.Nectarvalue = int.Parse(nectarValueString);
                if (nectarValue < 0) _errors.Add($"{_species.Name} | Foute nectarwaarde '{nectarValue}'. Moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Nectarwaarde: '{nectarValueString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddBiodiversity(string biodiversityString)
        {
            int? biodiversity = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(biodiversityString)) _species.Biodiversity = int.Parse(biodiversityString);
                if (biodiversity < 0) _errors.Add($"{_species.Name} | Foute biodiversiteitswaarde '{biodiversity}'. Moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Biodiversiteit: '{biodiversityString}' moet een getal zijn.", MessageType.Error);
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
                _errors.Add($"{_species.Name} | Rating: '{ratingString}' is ongeldig.", MessageType.Error);
            }
            return this;
        }

        public Species Build()
        {
            if (!string.IsNullOrWhiteSpace(_species.Name))
            {
                _species.Errors = _errors;
                return _species;
            }
            else return null;
        }
    }
}
