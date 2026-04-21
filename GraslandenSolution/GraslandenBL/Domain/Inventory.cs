using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain
{
    public class Inventory
    {
        public Inventory(DateTime date, string name)
        {
            Date = date;
            Name = name;
            Measurements = new List<Measurement>();
        }

        public DateTime Date { get; set; }
        public String Name { get; set; }
        public List<Measurement> Measurements { get; set; }

        public List<Species> GetSpecies()
        {
            return Measurements.Select(m => m.Species).ToList();
        }

        public List<Plot> GetPlots()
        {
            return Measurements.Select(m => m.Plot).ToList();
        }
    }
}
