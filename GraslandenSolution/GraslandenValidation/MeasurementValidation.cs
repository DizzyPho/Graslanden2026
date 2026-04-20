using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenValidation
{
    public static class MeasurementValidation
    {
        public static bool Validate(string coverage, out List<string> errors)
        {
            errors = new List<string>();
            if (String.IsNullOrEmpty(coverage)) errors.Add("Bedekkingsklasse mag niet leeg zijn.");

            return errors.Count == 0;
        }
    }
}
