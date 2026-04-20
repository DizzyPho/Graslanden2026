using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenValidation
{
    public static class SpeciesValidation
    {
        public static bool Validate(string name, string moistureString, string phString, string nitrogenString, string nectarValueString, string biodiversityString, string ratingString, out List<string> errors)
        {
            errors = new List<string>();
            int? id;
            int moisture;
            int ph;
            int nitrogen;
            int? nectarValue = null;
            int? biodiversity = null;
            Rating? rating = null;

            try
            {
                moisture = int.Parse(moistureString);
                if (moisture < 0) errors.Add($"Foute waarde voor vochtigheid '{moisture}'. Moet een positief getal zijn.");
            }
            catch
            {
                errors.Add($"Vochtgehalte: '{moistureString}' moet een getal zijn.");
            }

            try
            {
                ph = int.Parse(phString);
                if (ph < 0) errors.Add($"Foute waarde voor zuurtegraad '{ph}'. Moet een positief getal zijn.");
            }
            catch
            {
                errors.Add($"Zuurtegraad: '{phString}' moet een getal zijn.");
            }

            try
            {
                nitrogen = int.Parse(nitrogenString);
                if (nitrogen < 0) errors.Add($"Foute stikstofwaarde '{nitrogen}'. Moet een positief getal zijn.");
            }
            catch
            {
                errors.Add($"Stikstofgehalte: '{nitrogenString}' moet een getal zijn.");
            }

            try
            {
                if (nectarValueString != null) nectarValue = int.Parse(nectarValueString);
                if (nectarValue < 0) errors.Add($"Foute nectarwaarde '{nectarValue}'. Moet een positief getal zijn.");
            }
            catch
            {
                errors.Add($"Nectarwaarde: '{nectarValueString}' moet een getal zijn.");
            }

            try
            {
                if (biodiversityString != null) biodiversity = int.Parse(biodiversityString);
                if (biodiversity < 0) errors.Add($"Foute biodiversiteitswaarde '{biodiversity}'. Moet een positief getal zijn.");
            }
            catch
            {
                errors.Add($"Biodiversiteit: '{biodiversityString}' moet een getal zijn.");
            }

            if (String.IsNullOrEmpty(name)) errors.Add("Naam mag niet leeg zijn.");

            return errors.Count == 0;
        }
    }
}
