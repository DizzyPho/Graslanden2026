using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain
{
    public class Plot
    {
        public Plot(string code, double areaSqMeters, string campus, ManagementType managementType, string plotType, Dictionary<string, MessageType> errors = null)
        {
            Code = code;
            AreaSqMeters = areaSqMeters;
            Campus = campus;
            ManagementType = managementType;
            PlotType = plotType;
            if (errors != null)
                Errors = errors;
            else Errors = new Dictionary<string, MessageType>();
        }

        public string Code { get; set;  }
        public double AreaSqMeters { get; set; }
        public string Campus { get; set; }
        public ManagementType ManagementType { get; set; }
        public string PlotType { get; set; }
        public Dictionary<string, MessageType> Errors { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Plot plot &&
                   Code == plot.Code;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code);
        }
    }
}
