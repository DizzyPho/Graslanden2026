using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.DTOs
{
    public class InventoryDTO
    {
        private InventoryDTO(DateTime date, string name)
        {
            Date = date;
            Name = name;
        }

        public DateTime Date { get; set; }
        public String Name { get; set; }
    }
}
