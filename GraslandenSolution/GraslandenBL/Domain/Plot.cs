using GraslandenBL.Enums;
using GraslandenBL.FactoryResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain
{
    public class Plot
    {
        private Plot(string code, double areaSqMeters, string campus, ManagementType managementType, PlotType plotType)
        {
            Code = code;
            AreaSqMeters = areaSqMeters;
            Campus = campus;
            ManagementType = managementType;
            PlotType = plotType;
        }

        public static FactoryResult<Plot> Create(string code, double areaSqMeters, string campus, ManagementType managementType, string plotTypeCode, string plotTypeDesc)
        {
            List<String> errors = new List<String>();
            FactoryResult<PlotType> plotTypeFactory = PlotType.Create(plotTypeCode, plotTypeDesc);
            errors.Concat(plotTypeFactory.Errors);
            if (String.IsNullOrEmpty(code)) errors.Add("Graslandcode mag niet leeg zijn.");
            if (areaSqMeters <= 0) errors.Add($"Ongeldige oppervlakte '${areaSqMeters}'. Moet een strikt positief getal zijn.");
            if (String.IsNullOrEmpty(campus)) errors.Add("Campusnaam mag niet leeg zijn.");

            if (errors.Count > 0)
            {
                return new FactoryResult<Plot>(errors);
            }
            else
            {
                return new FactoryResult<Plot>(new Plot(code, areaSqMeters, campus, managementType, plotTypeFactory.Result));
            }
        }

        public string Code { get; set;  }
        public double AreaSqMeters { get; set; }
        public string Campus { get; set; }
        ManagementType ManagementType { get; set; }
        PlotType PlotType { get; set; }
    }
}
