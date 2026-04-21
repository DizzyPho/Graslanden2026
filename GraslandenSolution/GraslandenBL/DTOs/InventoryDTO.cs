using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.DTOs
{
    public class InventoryDTO
    {
        public InventoryDTO(DateTime date, string name)
        {
            Date = date;
            Name = name;
        }

        public InventoryDTO(int id, DateTime date, string name) : this(date, name)
        {
            Id = id;
        }

        public DateTime Date { get; set; }
        public String Name { get; set; }

        public int? Id { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Date.ToString("dd/MM/yyyy")}";
        }
    }
}
