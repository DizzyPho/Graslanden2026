using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenValidation
{
    public class InventoryValidation
    {
        public static bool Validate(DateTime date, String name, out List<string> errors)
        {
            errors = new List<string>();
            if (date > DateTime.Now) errors.Add("Datum mag niet in de toekomst zijn.");
            if (String.IsNullOrEmpty(name)) errors.Add("Naam mag niet leeg zijn.");

            return errors.Count == 0;
        }
    }
}
