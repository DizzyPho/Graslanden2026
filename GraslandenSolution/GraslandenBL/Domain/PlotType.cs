using System;
using System.Collections.Generic;

namespace GraslandenBL.Domain
{
    public class PlotType
    {
        public PlotType(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; set; }
        public string Description { get; set; }

    }
}
