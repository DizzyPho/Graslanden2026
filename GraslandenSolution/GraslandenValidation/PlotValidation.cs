using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenValidation
{
    public static class PlotValidation
    {
        public static bool Validate(string code, string areaString, string campus, string managementTypeString, out Dictionary<string, MessageType> errors)
        {
            errors = new Dictionary<string, MessageType>();

            if (String.IsNullOrWhiteSpace(code)) errors.Add("Graslandcode mag niet leeg zijn.", MessageType.Error);
            if (double.TryParse(areaString, out double areaSqMetersDouble))
            {                
                if (areaSqMetersDouble <= 0) errors.Add($"Ongeldige oppervlakte '{areaString}'. Moet een strikt positief getal zijn.", MessageType.Remark);
            }
            else
            {
                errors.Add($"Ongeldige oppervlakte '{areaString}'. Moet een getal zijn", MessageType.Error);
            }
            if (String.IsNullOrWhiteSpace(campus)) errors.Add("Campusnaam mag niet leeg zijn.", MessageType.Error);

            List<String> validTypes = ["EXTENSIEF", "INTENSIEF", "NETHEIDSBOORD", "SCHAPENWEIDE"];
            if (!validTypes.Contains(managementTypeString.ToUpper().Trim()))
            {
                errors.Add($"Onbekend beheertype: '{managementTypeString}'.", MessageType.Error);
            }
            
            return errors.Where(e => e.Value == MessageType.Error).Count() == 0;
        }
    }
}
