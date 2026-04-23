using GraslandenBL.Domain;
using GraslandenBL.Enums;

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
            try
            {
                if (!string.IsNullOrWhiteSpace(moistureString)) _species.Moisture = int.Parse(moistureString);
                if (_species.Moisture < 0) _errors.Add($"{_species.Name} | Fout vochtgehalte: '{_species.Moisture}' moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Fout vochtgehalte: '{moistureString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddPh(string phString)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(phString)) _species.Ph = int.Parse(phString);
                if (_species.Ph < 0) _errors.Add($"{_species.Name} | Foute zuurtegraad: '{_species.Ph}' moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Foute zuurtegraad: '{phString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddNitrogen(string nitrogenString)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(nitrogenString)) _species.Nitrogen = int.Parse(nitrogenString);
                if (_species.Nitrogen < 0) _errors.Add($"{_species.Name} | Fout stikstofgehalte: '{_species.Nitrogen}' moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Fout stikstofgehalte: '{nitrogenString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddNectarValue(string nectarValueString)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(nectarValueString)) _species.Nectarvalue = int.Parse(nectarValueString);
                if (_species.Nectarvalue < 0) _errors.Add($"{_species.Name} | Foute nectarwaarde: '{_species.Nectarvalue}' moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Foute nectarwaarde: '{nectarValueString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddBiodiversity(string biodiversityString)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(biodiversityString)) _species.Biodiversity = int.Parse(biodiversityString);
                if (_species.Biodiversity < 0) _errors.Add($"{_species.Name} | Foute biodiversiteit: '{_species.Biodiversity}' moet een positief getal zijn.", MessageType.Error);
            }
            catch
            {
                _errors.Add($"{_species.Name} | Foute biodiversiteit: '{biodiversityString}' moet een getal zijn.", MessageType.Error);
            }
            return this;
        }

        public SpeciesBuilder AddRating(string ratingString)
        {
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
