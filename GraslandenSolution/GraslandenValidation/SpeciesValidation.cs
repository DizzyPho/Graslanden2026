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
            int? id = -1;
            int moisture = -1;
            int ph = -1;
            int nitrogen = -1;
            int? nectarValue = null;
            int? biodiversity = null;
            Rating? rating = null;

            try
            {
                moisture = int.Parse(moistureString);
            }
            catch
            {
                errors.Add($"Vochtgehalte: '{moistureString}' moet een getal zijn.");
            }

            try
            {
                ph = int.Parse(phString);
            }
            catch
            {
                errors.Add($"Zuurtegraad: '{phString}' moet een getal zijn.");
            }

            try
            {
                nitrogen = int.Parse(nitrogenString);
            }
            catch
            {
                errors.Add($"Stikstofgehalte: '{nitrogenString}' moet een getal zijn.");
            }

            try
            {
                if (nectarValueString != null) nectarValue = int.Parse(nectarValueString);
            }
            catch
            {
                errors.Add($"Nectarwaarde: '{nectarValueString}' moet een getal zijn.");
            }

            try
            {
                if (biodiversityString != null) biodiversity = int.Parse(biodiversityString);
            }
            catch
            {
                errors.Add($"Biodiversiteit: '{biodiversityString}' moet een getal zijn.");
            }

            if (String.IsNullOrEmpty(name)) errors.Add("Naam mag niet leeg zijn.");
            if (moisture < 0) errors.Add($"Foute waarde voor vochtigheid '{moisture}'. Moet een positief getal zijn.");
            if (ph < 0) errors.Add($"Foute waarde voor zuurtegraad '{ph}'. Moet een positief getal zijn.");
            if (nitrogen < 0) errors.Add($"Foute stikstofwaarde '{nitrogen}'. Moet een positief getal zijn.");
            if (nectarValue < 0) errors.Add($"Foute nectarwaarde '{nectarValue}'. Moet een positief getal zijn.");
            if (biodiversity < 0) errors.Add($"Foute biodiversiteitswaarde '{moisture}'. Moet een positief getal zijn.");

            return errors.Count == 0;
        }
    }
}
