using GraslandenBL.FactoryResults;
using System;
using System.Collections.Generic;

namespace GraslandenBL.Domain
{
    public class PlotType
    {
        private PlotType(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; set; }
        public string Description { get; set; }


        public static FactoryResult<PlotType> Create(string code, string description)
        {
            List<String> errors = new List<String>();
            if (String.IsNullOrEmpty(code)) errors.Add("Type grasland mag niet leeg zijn.");
            if (String.IsNullOrEmpty(description)) errors.Add("Beschrijving mag niet leeg zijn.");

            if (errors.Count > 0)
            {
                return new FactoryResult<PlotType>(errors);
            }
            else
            {
                return new FactoryResult<PlotType>(new PlotType(code, description));
            }
        }
    }
}
