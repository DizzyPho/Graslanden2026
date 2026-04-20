using GraslandenBL.FactoryResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.Domain
{
    public class Inventory
    {
        private Inventory(DateTime date, string name)
        {
            Date = date;
            Name = name;
            Measurements = new List<Measurement>();
        }

        public static FactoryResult<Inventory> Create(DateTime date, String name) 
        {
            List<String> errors = new List<String>();
            if (date > DateTime.Now) errors.Add("Datum mag niet in de toekomst zijn.");
            if (String.IsNullOrEmpty(name)) errors.Add("Naam mag niet leeg zijn.");

            if(errors.Count > 0)
            {
                return new FactoryResult<Inventory>(errors);
            }
            else
            {
                return new FactoryResult<Inventory>(new Inventory(date, name));
            }
        }
        public DateTime Date { get; set; }
        public String Name { get; set; }
        public List<Measurement> Measurements { get; set; }


    }
}
