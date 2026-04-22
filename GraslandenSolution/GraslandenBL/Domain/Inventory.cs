using GraslandenBL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain
{
    public class Inventory
    {
        public Inventory(DateTime date, string name, Dictionary<string, MessageType> errors = null)
        {
            Date = date;
            Name = name;
            Measurements = new List<Measurement>();
            if (errors != null)
                Errors = errors;
            else Errors = new Dictionary<string, MessageType>();
        }

        public DateTime Date { get; set; }
        public String Name { get; set; }
        public List<Measurement> Measurements { get; set; }
        public Dictionary<string, MessageType> Errors { get; set; }

        public List<Species> GetSpecies()
        {
            return Measurements.Select(m => m.Species).Distinct().ToList();
        }

        public List<Plot> GetPlots()
        {
            return Measurements.Select(m => m.Plot).Distinct().ToList();
        }
    }
}
