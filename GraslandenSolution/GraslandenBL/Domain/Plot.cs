using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain
{
    public class Plot
    {
        public Plot(string code, double areaSqMeters, string campus, ManagementType managementType, string plotType, List<string> errors = null)
        {
            Code = code;
            AreaSqMeters = areaSqMeters;
            Campus = campus;
            ManagementType = managementType;
            PlotType = plotType;
            Errors = errors;
        }

        public string Code { get; set;  }
        public double AreaSqMeters { get; set; }
        public string Campus { get; set; }
        public ManagementType ManagementType { get; set; }
        public string PlotType { get; set; }
        public List<string> Errors { get; set; }
    }
}
