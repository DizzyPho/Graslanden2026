using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenValidation
{
    public static class PlotTypeValidation
    {
        public static bool Validate(string code, string description, out List<string> errors)
        {
            errors = new List<string>();
            if (String.IsNullOrEmpty(code)) errors.Add("Type grasland mag niet leeg zijn.");
            if (String.IsNullOrEmpty(description)) errors.Add("Beschrijving mag niet leeg zijn.");

            return errors.Count == 0;
        }
    }
}
