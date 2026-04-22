using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenValidation
{
    public static class PlotValidation
    {
        public static bool Validate(string code, string areaString, string campus, string managementTypeString, out List<string> errors)
        {
            errors = new List<string>();

            if (String.IsNullOrEmpty(code)) errors.Add("Graslandcode mag niet leeg zijn.");
            if (double.TryParse(areaString, out double areaSqMetersDouble))
            {                
                if (areaSqMetersDouble <= 0) errors.Add($"Ongeldige oppervlakte '${areaString}'. Moet een strikt positief getal zijn.");
            }
            else
            {
                errors.Add($"Ongeldige oppervlakte '{areaString}'. Moet een getal zijn");
            }
            if (String.IsNullOrEmpty(campus)) errors.Add("Campusnaam mag niet leeg zijn.");

            List<String> validTypes = ["EXTENSIEF", "INTENSIEF", "NETHEIDSBOORD", "SCHAPENWEIDE"];
            if (!validTypes.Contains(managementTypeString.ToUpper().Trim()))
            {
                errors.Add($"Onbekend beheertype: '{managementTypeString}'.");
            }
            
            return errors.Count == 0;
        }
    }
}
