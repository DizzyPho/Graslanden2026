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

        public static string CalculateMoistureString(double? avgMoisture)
        {
            string moistureString = avgMoisture switch
            {
                <= 1 => "Extreem droog",
                < 3 => "Tussen extreem droog en droog",
                3 => "Droog",
                < 5 => "Tussen droog en fris",
                5 => "Fris",
                < 7 => "Tussen fris en bijna altijd vochtig maar niet nat",
                7 => "Bijna altijd vochtig maar niet nat",
                < 9 => "Tussen bijna altijd vochtig en nat",
                < 10 => "Nat",
                < 11 => "Ondiep water",
                >= 11 => "Ondergedoken",
                _ => ""
            };
            return moistureString;
        }

        public static string CalculatePhString(double? avgPh)
        {
            string phString = avgPh switch
            {
                <= 1 => "Sterk zuur",
                < 3 => "Tussen sterk zuur en zuur",
                3 => "Zuur",
                < 5 => "Tussen zuur en matig zuur",
                5 => "Matig zuur",
                < 7 => "Tussen matig zuur en zwak basisch",
                7 => "Zwak basisch",
                < 9 => "Tussen zwak basisch en basisch",
                >= 9 => "Basisch",
                _ => ""
            };
            return phString;
        }

        public static string CalculateNitrogenString(double? avgNitrogen)
        {
            string nitrogenString = avgNitrogen switch
            {
                <= 1 => "Zeer stikstofarm",
                < 3 => "Tussen zeer stikstofarm en stikstofarm",
                  3 => "Stikstofarm",
                < 5 => "Tussen stikstofarm en matig stikstofrijk",
                  5 => "Matig stikstofrijk",
                < 7 => "Tussen matig stikstofrijk en vrij uitgesproken stikstofrijk",
                  7 => "Vrij uitgesproken stikstofrijk",
                < 9 => "Tussen vrij en zeer uitgesproken stikstofrijk",
                >= 9 => "Zeer uitgesproken stikstofrijk",
                _ => ""
            }; 
            return nitrogenString;
        }

        public static ManagementType StringToManagementType(string s)
        {
            ManagementType managementTypeEnum = s switch
            {
                "Netheidsboord" => ManagementType.Netheidsboord,
                "Schapenweide" => ManagementType.Schapenweide,
                "Intensief" => ManagementType.Intensief,
                "Extensief" => ManagementType.Extensief,
                _ => throw new Exception("Invalid management type")
            };
            return managementTypeEnum;
        }
    }
}
