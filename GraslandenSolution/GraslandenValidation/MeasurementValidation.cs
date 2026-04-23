using GraslandenBL.Enums;

namespace GraslandenValidation
{
    public static class MeasurementValidation
    {
        public static bool Validate(string coverage, out Dictionary<string, MessageType> errors)
        {
            errors = new Dictionary<string, MessageType>();
            if (String.IsNullOrWhiteSpace(coverage)) errors.Add("Bedekkingsklasse mag niet leeg zijn.", MessageType.Error);

            return errors.Where(e => e.Value == MessageType.Error).Count() == 0;
        }
    }
}